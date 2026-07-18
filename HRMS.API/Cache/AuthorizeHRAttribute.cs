using HR.Identity.infrastructure.Data;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Security;
using HRMS.API.Controllers.IdentityManager.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HRMS.API.Cache;

/// <summary>
/// Authorization filter that enforces the hierarchical permission model.
/// It runs on every request (applied on <c>AppBaseController</c>), so it is optimized to:
///  - validate authentication, super-admin claim and security stamp (token-replay protection),
///  - resolve the caller's accessible permission keys with a minimal, fully asynchronous set of queries,
///  - evaluate access against an <see cref="IMemoryCache"/>-backed permission graph instead of rebuilding
///    the whole permission tree on each call.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthorizeHRAttribute : Attribute, IAsyncAuthorizationFilter
{
    private static readonly TimeSpan PermissionGraphCacheTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan UserAccessCacheTtl = TimeSpan.FromMinutes(5);
    private const long AdminRoleId = 1;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Skip authorization for [AllowAnonymous] endpoints.
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            return;
        if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            return;

        var services = context.HttpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<AuthorizeHRAttribute>>();
        var user = context.HttpContext.User;
        var request = GetRequestInfo(context);

        if (user.Identity?.IsAuthenticated != true)
        {
            var (failureReason, failureDetail) = ResolveAuthFailureReason(context.HttpContext);
            logger.LogWarning("SECURITY FAILURE - Unauthenticated access attempt to {Controller}.{Action} - Reason: {Reason}, Detail: {Detail}, from IP: {IP}, UserAgent: {UserAgent}, Referer: {Referer}",
                request.Controller, request.Action, failureReason, failureDetail, request.IP, request.UserAgent, request.Referer);
            context.Result = new UnauthorizedResult();
            return;
        }

        // Super-admin short-circuit.
        if (user.HasClaim(c => c is { Type: "isAdmin", Value: "true" }))
            return;

        var userIdClaim = user.FindFirst("userId");
        var tokenStampClaim = user.FindFirst("security_stamp");
        if (string.IsNullOrWhiteSpace(userIdClaim?.Value) || tokenStampClaim is null)
        {
            logger.LogWarning("SECURITY FAILURE - Invalid token claims - HasUserId: {HasUserId}, HasSecurityStamp: {HasStamp} from IP: {IP}, UserAgent: {UserAgent}",
                userIdClaim is not null, tokenStampClaim is not null, request.IP, request.UserAgent);
            context.Result = new UnauthorizedResult();
            return;
        }

        // Decrypt the user id exactly once (supports both encrypted and legacy plain tokens).
        var userId = ResolveUserId(services, userIdClaim.Value);
        if (userId <= 0)
        {
            logger.LogWarning("SECURITY FAILURE - Invalid user ID format in token from IP: {IP}, UserAgent: {UserAgent}",
                request.IP, request.UserAgent);
            context.Result = new UnauthorizedResult();
            return;
        }

        var db = services.GetRequiredService<IdentityContext>();

        // Skip duplicate stamp query when JwtBearer already validated the stamp on this request.
        var stampAlreadyValidated = context.HttpContext.Items.TryGetValue(AuthorizationContextKeys.StampValidatedUserId, out var validatedObj)
            && validatedObj is long validatedUserId
            && validatedUserId == userId;

        if (!stampAlreadyValidated)
        {
            // Security stamp validation (rejects replayed/stale tokens). Single async scalar query.
            var currentStamp = await db.AspNetUsers.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.SecurityStamp)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(currentStamp) || !string.Equals(currentStamp, tokenStampClaim.Value, StringComparison.Ordinal))
            {
                logger.LogWarning("SECURITY FAILURE - Invalid security stamp for UserId: {UserId} from IP: {IP}, UserAgent: {UserAgent}. Current: {CurrentStamp}, Token: {TokenStamp}",
                    userId, request.IP, request.UserAgent, Mask(currentStamp), Mask(tokenStampClaim.Value));
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        // Resolve the permission key from route data / [CustomAccessKey].
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var accessKey = context.ActionDescriptor.EndpointMetadata.OfType<CustomAccessKey>().FirstOrDefault();
        if (accessKey is not null)
            actionName = accessKey.AccessKey;

        var permission = $"{controllerName}.{actionName}";
        var normalizedPermission = NormalizeKey(permission);

        var userAccess = await GetUserAccessAsync(services, db, userId, tokenStampClaim.Value);
        if (userAccess.IsAdminRole)
            return;

        var accessibleKeys = userAccess.AccessibleKeys;

        // Evaluate against the cached permission graph.
        var graph = await GetPermissionGraphAsync(services, db);

        graph.TryGetNode(normalizedPermission, out var targetNode);
        var permissionDescription = graph.ResolveDescription(
            normalizedPermission, targetNode, controllerName, actionName);

        if (targetNode is null)
        {
            logger.LogWarning("SECURITY FAILURE - Missing permission configuration for {Permission} (User: {UserId}) from IP: {IP}, UserAgent: {UserAgent}",
                permission, userId, request.IP, request.UserAgent);
            SetAccessDenied(context, permission, permissionDescription, normalizedPermission, controllerName, actionName);
            return;
        }

        if (IsAuthorized(targetNode, accessibleKeys))
            return;

        logger.LogWarning("SECURITY FAILURE - Access denied to {Permission} for User: {UserId} from IP: {IP}, UserAgent: {UserAgent}",
            permission, userId, request.IP, request.UserAgent);
        SetAccessDenied(context, permission, permissionDescription, normalizedPermission, controllerName, actionName);
    }

    private static void SetAccessDenied(
        AuthorizationFilterContext context,
        string permission,
        string permissionDescription,
        string normalizedPermission,
        string? controllerName,
        string? actionName)
    {
        SetUnauthorizedWithMessage(
            context,
            "شما به این عملیات دسترسی ندارید",
            new
            {
                missingPermission = permission,
                permissionDescription,
                normalizedKey = normalizedPermission,
                controller = controllerName,
                action = actionName
            });
    }

    /// <summary>
    /// Determines whether <paramref name="target"/> is reachable given the caller's accessible keys.
    /// Mirrors the original tree-filter semantics: access is granted when the node itself, any of its
    /// ancestors (an accessible node grants its whole subtree), or any of its descendants
    /// (an accessible descendant keeps the ancestor visible) is in the accessible set.
    /// </summary>
    private static bool IsAuthorized(Node target, HashSet<string> accessibleKeys)
    {
        for (Node? node = target; node is not null; node = node.Parent)
        {
            if (!string.IsNullOrEmpty(node.Key) && accessibleKeys.Contains(node.Key))
                return true;
        }

        return SubtreeContainsAccessibleKey(target, accessibleKeys);
    }

    private static bool SubtreeContainsAccessibleKey(Node node, HashSet<string> accessibleKeys)
    {
        foreach (var child in node.Children)
        {
            if (!string.IsNullOrEmpty(child.Key) && accessibleKeys.Contains(child.Key))
                return true;
            if (SubtreeContainsAccessibleKey(child, accessibleKeys))
                return true;
        }

        return false;
    }

    private static long ResolveUserId(IServiceProvider services, string rawValue)
    {
        var encryption = services.GetService<UserIdEncryptionService>();
        if (encryption is not null)
        {
            var decrypted = encryption.DecryptUserId(rawValue);
            if (decrypted > 0)
                return decrypted;
        }

        return long.TryParse(rawValue, out var plain) ? plain : -1;
    }

    private static void AddNormalizedKeys(HashSet<string> target, IEnumerable<string?> rawKeys)
    {
        foreach (var raw in rawKeys)
        {
            var normalized = NormalizeKey(raw);
            if (normalized.Length > 0)
                target.Add(normalized);
        }
    }

    private static async Task<UserAccessCacheEntry> GetUserAccessAsync(
        IServiceProvider services,
        IdentityContext db,
        long userId,
        string securityStamp)
    {
        var cache = services.GetService<IMemoryCache>();
        var cacheKey = AuthorizationCacheKeys.UserAccess(userId, securityStamp);
        if (cache is not null && cache.TryGetValue(cacheKey, out UserAccessCacheEntry? cached) && cached is not null)
            return cached;

        var roleIds = await db.UserRoles.AsNoTracking()
            .Where(r => r.UserId == userId)
            .Select(r => r.RoleId)
            .ToArrayAsync();

        if (roleIds.Contains(AdminRoleId))
        {
            var adminEntry = new UserAccessCacheEntry
            {
                IsAdminRole = true,
                AccessibleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            };
            cache?.Set(cacheKey, adminEntry, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = UserAccessCacheTtl
            });
            return adminEntry;
        }

        var userClaimKeys = await db.UserClaims.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => c.ClaimType)
            .ToArrayAsync();

        var roleClaimKeys = roleIds.Length == 0
            ? Array.Empty<string?>()
            : await db.RoleClaims.AsNoTracking()
                .Where(c => roleIds.Contains(c.RoleId))
                .Select(c => c.ClaimType)
                .ToArrayAsync();

        var accessibleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddNormalizedKeys(accessibleKeys, userClaimKeys);
        AddNormalizedKeys(accessibleKeys, roleClaimKeys);

        var entry = new UserAccessCacheEntry
        {
            IsAdminRole = false,
            AccessibleKeys = accessibleKeys
        };

        cache?.Set(cacheKey, entry, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = UserAccessCacheTtl
        });

        return entry;
    }

    private static async Task<PermissionGraph> GetPermissionGraphAsync(IServiceProvider services, IdentityContext db)
    {
        var cache = services.GetService<IMemoryCache>();
        if (cache is not null && cache.TryGetValue(AuthorizationCacheKeys.PermissionGraph, out PermissionGraph? cached) && cached is not null)
            return cached;

        var graph = await BuildPermissionGraphAsync(db);

        cache?.Set(AuthorizationCacheKeys.PermissionGraph, graph, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = PermissionGraphCacheTtl
        });

        return graph;
    }

    private static async Task<PermissionGraph> BuildPermissionGraphAsync(IdentityContext db)
    {
        var flat = await db.Permissions.AsNoTracking()
            .Select(p => new Node
            {
                Id = p.Id,
                Name = p.DisplayName,
                ParentId = p.ParentId,
                Key = p.Name
            })
            .ToListAsync();

        var routes = await db.PermissionRoutes.AsNoTracking()
            .Where(r => r.Claim != null && r.Claim != "")
            .Select(r => new { r.Claim, r.Description, r.title })
            .ToListAsync();

        var fullKeysById = flat.ToDictionary(n => n.Id, n => n.Key);

        var byId = new Dictionary<Guid, Node>(flat.Count);
        foreach (var node in flat)
            byId[node.Id] = node;

        var byKey = new Dictionary<string, Node>(flat.Count, StringComparer.OrdinalIgnoreCase);
        var controllerLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var moduleLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var node in flat)
        {
            var fullKey = fullKeysById[node.Id];
            var segments = fullKey.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!string.IsNullOrWhiteSpace(node.Name))
            {
                if (segments.Length == 1)
                    moduleLabels[segments[0]] = node.Name.Trim();
                else if (segments.Length == 2)
                    controllerLabels[segments[1]] = node.Name.Trim();
            }

            node.Key = NormalizeKey(fullKey);

            if (node.ParentId.HasValue && byId.TryGetValue(node.ParentId.Value, out var parent))
            {
                node.Parent = parent;
                parent.Children.Add(node);
            }

            if (node.Key.Length > 0)
                byKey[node.Key] = node;
        }

        var routeDescriptions = new Dictionary<string, string>(routes.Count, StringComparer.Ordinal);
        foreach (var route in routes)
        {
            var description = !string.IsNullOrWhiteSpace(route.Description)
                ? route.Description
                : route.title;
            if (string.IsNullOrWhiteSpace(description))
                continue;

            routeDescriptions[route.Claim!.Trim().ToLowerInvariant()] = description.Trim();
        }

        return new PermissionGraph(byKey, routeDescriptions, controllerLabels, moduleLabels);
    }

    private static string NormalizeKey(string? input) => PermissionKeyNormalizer.Normalize(input);

    private static void SetUnauthorizedWithMessage(AuthorizationFilterContext context, string message, object? payload = null)
    {
        context.Result = new ObjectResult(OperationResult.Failed(msg: message, payload: payload))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }

    private static string Mask(string? value)
        => string.IsNullOrEmpty(value) ? "(empty)" : value[..Math.Min(8, value.Length)];

    /// <summary>
    /// Resolves WHY the current request is unauthenticated, so the security log states the exact
    /// cause instead of a generic message. The JwtBearer events (Program.cs) stash the failure code
    /// in <c>HttpContext.Items["auth_error"]</c> and the exception detail in
    /// <c>HttpContext.Items["auth_error_detail"]</c>; when no code is present we distinguish between
    /// "no token was sent at all" and "a token was sent but was never validated".
    /// </summary>
    private static (string Reason, string Detail) ResolveAuthFailureReason(HttpContext httpContext)
    {
        var detail = httpContext.Items["auth_error_detail"] as string ?? "-";

        if (httpContext.Items["auth_error"] is string code && !string.IsNullOrWhiteSpace(code))
            return (code, detail);

        var hasAuthHeader = !string.IsNullOrWhiteSpace(httpContext.Request.Headers.Authorization.ToString());
        var hasJwtCookie = !string.IsNullOrWhiteSpace(httpContext.Request.Cookies["jwt"]);

        if (!hasAuthHeader && !hasJwtCookie)
        {
            // Log which cookies DID arrive plus request origin/host: distinguishes
            // "browser sent nothing" (cross-origin / stale Secure cookie) from "wrong cookie names".
            var cookieNames = httpContext.Request.Cookies.Count > 0
                ? string.Join(",", httpContext.Request.Cookies.Keys)
                : "(none)";
            var origin = httpContext.Request.Headers.Origin.ToString();
            if (string.IsNullOrEmpty(origin)) origin = "(none)";
            return ("no_token",
                $"No Authorization header and no 'jwt' cookie. ReceivedCookies: {cookieNames}, Origin: {origin}, Host: {httpContext.Request.Scheme}://{httpContext.Request.Host}");
        }

        var source = hasJwtCookie && hasAuthHeader ? "header+cookie" : hasJwtCookie ? "cookie" : "header";
        return ("token_not_validated", $"A token was present ({source}) but authentication did not succeed");
    }

    private static RequestInfo GetRequestInfo(AuthorizationFilterContext context)
    {
        var routeValues = context.RouteData.Values;
        var headers = context.HttpContext.Request.Headers;

        return new RequestInfo
        {
            Controller = routeValues["controller"]?.ToString() ?? "Unknown",
            Action = routeValues["action"]?.ToString() ?? "Unknown",
            IP = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserAgent = headers.UserAgent.ToString(),
            Referer = headers.Referer.ToString()
        };
    }

    private sealed class UserAccessCacheEntry
    {
        public bool IsAdminRole { get; init; }
        public HashSet<string> AccessibleKeys { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed class PermissionGraph
    {
        private readonly Dictionary<string, Node> _byKey;
        private readonly Dictionary<string, string> _routeDescriptionsByClaim;
        private readonly Dictionary<string, string> _controllerLabels;
        private readonly Dictionary<string, string> _moduleLabels;

        public PermissionGraph(
            Dictionary<string, Node> byKey,
            Dictionary<string, string> routeDescriptionsByClaim,
            Dictionary<string, string> controllerLabels,
            Dictionary<string, string> moduleLabels)
        {
            _byKey = byKey;
            _routeDescriptionsByClaim = routeDescriptionsByClaim;
            _controllerLabels = controllerLabels;
            _moduleLabels = moduleLabels;
        }

        public bool TryGetNode(string normalizedKey, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out Node node)
            => _byKey.TryGetValue(normalizedKey, out node);

        public string ResolveDescription(
            string normalizedKey,
            Node? permissionNode,
            string? controller,
            string? action)
        {
            var actionLabel = ResolveActionLabel(normalizedKey, permissionNode, controller, action);
            var controllerLabel = ResolveControllerLabel(permissionNode, controller);
            var moduleLabel = ResolveModuleLabel(permissionNode, controller, action);

            var parts = new List<string>(3);
            if (!string.IsNullOrWhiteSpace(moduleLabel))
                parts.Add(moduleLabel);
            if (!string.IsNullOrWhiteSpace(controllerLabel))
                parts.Add(controllerLabel);
            if (!string.IsNullOrWhiteSpace(actionLabel))
                parts.Add(actionLabel);

            if (parts.Count > 0)
                return string.Join(" / ", parts);

            return PermissionKeyDescription.TryResolveActionDescription(controller, action)
                   ?? $"{controller}.{action}";
        }

        private string? ResolveActionLabel(
            string normalizedKey,
            Node? permissionNode,
            string? controller,
            string? action)
        {
            if (permissionNode is not null && !string.IsNullOrWhiteSpace(permissionNode.Name))
                return permissionNode.Name.Trim();

            foreach (var (claim, description) in _routeDescriptionsByClaim)
            {
                if (claim.EndsWith(normalizedKey, StringComparison.Ordinal))
                    return description;
            }

            return PermissionKeyDescription.TryResolveActionDescription(controller, action);
        }

        private string? ResolveControllerLabel(Node? permissionNode, string? controller)
        {
            var parent = permissionNode?.Parent;
            if (parent is not null && !string.IsNullOrWhiteSpace(parent.Name))
                return parent.Name.Trim();

            if (!string.IsNullOrWhiteSpace(controller))
            {
                if (_controllerLabels.TryGetValue(controller, out var fromGraph))
                    return fromGraph;

                var fromRoute = TryGetRouteLabelForController(controller);
                if (!string.IsNullOrWhiteSpace(fromRoute))
                    return fromRoute;
            }

            return null;
        }

        private string? ResolveModuleLabel(Node? permissionNode, string? controller, string? action)
        {
            var moduleNode = permissionNode?.Parent?.Parent;
            if (moduleNode is not null
                && !string.IsNullOrWhiteSpace(moduleNode.Name)
                && !moduleNode.Key.Equals("actions", StringComparison.OrdinalIgnoreCase))
            {
                return moduleNode.Name.Trim();
            }

            string? moduleKey = PermissionKeyDescription.TryResolveModuleKey(controller, action);

            if (string.IsNullOrWhiteSpace(moduleKey) && !string.IsNullOrWhiteSpace(controller))
            {
                foreach (var claim in _routeDescriptionsByClaim.Keys)
                {
                    var segments = claim.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (segments.Length >= 2
                        && segments[1].Equals(controller, StringComparison.OrdinalIgnoreCase))
                    {
                        moduleKey = segments[0];
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(moduleKey)
                && _moduleLabels.TryGetValue(moduleKey, out var moduleLabel))
            {
                return moduleLabel;
            }

            return null;
        }

        private string? TryGetRouteLabelForController(string controller)
        {
            var controllerLower = controller.Trim().ToLowerInvariant();

            foreach (var (claim, description) in _routeDescriptionsByClaim)
            {
                var segments = claim.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (segments.Length == 2 && segments[1].Equals(controllerLower, StringComparison.Ordinal))
                    return description;
            }

            return null;
        }
    }

    private sealed class RequestInfo
    {
        public string Controller { get; init; } = string.Empty;
        public string Action { get; init; } = string.Empty;
        public string IP { get; init; } = string.Empty;
        public string UserAgent { get; init; } = string.Empty;
        public string Referer { get; init; } = string.Empty;
    }
}
