using AutoMapper;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/PermissionRoute")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("مدیریت مسیرهای کامپوننت ها")]
public class PermissionRouteController : AppBaseController
{
    private const int AdminRoleId = 1;
    private const string DefaultIcon = "setting";
    private const string RootMenuName = "سامانه سرمایه انسانی";

    private readonly PermissionRouteService _permissionRouteService;
    private readonly PermissionsService _permissionsService;

    public PermissionRouteController(
        PermissionRouteService permissionRouteService,
        PermissionsService permissionsService,
        ILogger<PermissionRouteController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _permissionsService = permissionsService;
        _permissionRouteService = permissionRouteService;
        _permissionRouteService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetCurrentUserMenu")]
    [CustomAccessKey(AccessKey: "GetCurrentUserMenu")]
    public IActionResult GetCurrentUserMenu()
    {
        var allPermissions = _permissionsService.GetAll();
        var routeDefinitions = _permissionRouteService.All().ToList();

        var permissionByName = allPermissions.ToDictionary(p => p.Name, p => p);
        var routeByNormalizedClaim = BuildRouteLookup(routeDefinitions);
        var routeClaimNames = new HashSet<string>(
            routeDefinitions.Where(r => r.Claim != null).Select(r => r.Claim!));

        var orderedFromRoute = routeDefinitions
            .OrderBy(pr => pr.Priority)
            .Select(pr => permissionByName.TryGetValue(pr.Claim, out var perm) ? perm : null)
            .Where(p => p != null)
            .Cast<Permission>()
            .ToList();

        var unmatched = allPermissions
            .Where(p => !routeClaimNames.Contains(p.Name))
            .ToList();

        var mergedPermissions = orderedFromRoute.Concat(unmatched).ToList();
        var flatMenu = BuildFlatMenu(mergedPermissions, routeByNormalizedClaim, permissionByName);

        var context = _permissionRouteService._unitOfWork.Context;
        var userRoleIds = context.UserRoles
            .AsNoTracking()
            .Where(i => i.UserId == currentUserId)
            .Select(i => i.RoleId)
            .ToList();

        var fullTree = BuildTree(flatMenu);

        if (userRoleIds.Contains(AdminRoleId))
        {
            return this.AppOk(OperationResult.Succeeded(payload: fullTree));
        }

        var userClaims = context.UserMenus
            .AsNoTracking()
            .Where(i => i.UserId == currentUserId)
            .Select(i => i.Claim.ToLower().Trim())
            .ToList();

        var roleClaims = context.RoleMenus
            .AsNoTracking()
            .Where(i => userRoleIds.Contains(i.RoleId))
            .Select(i => i.Claim.Trim().ToLower())
            .ToList();

        var distinctClaims = new HashSet<string>(userClaims);
        distinctClaims.UnionWith(roleClaims);

        var filteredTree = FilterMenuTree(fullTree, distinctClaims);
        return this.AppOk(OperationResult.Succeeded(payload: filteredTree));
    }

    [HttpGet, Route("GetRouteAccessContext")]
    [CustomAccessKey(AccessKey: "GetCurrentUserMenu")]
    public async Task<IActionResult> GetRouteAccessContext()
    {
        var context = _permissionRouteService._unitOfWork.Context;

        var routes = _permissionRouteService.All()
            .Where(r => !string.IsNullOrWhiteSpace(r.Claim) && !string.IsNullOrWhiteSpace(r.Route))
            .Select(r => new RouteClaimEntryDTO
            {
                Route = r.Route!.Trim(),
                Claim = r.Claim!.Trim().ToLowerInvariant()
            })
            .ToList();

        var userRoleIds = await context.UserRoles
            .AsNoTracking()
            .Where(i => i.UserId == currentUserId)
            .Select(i => i.RoleId)
            .ToArrayAsync();

        var isAdminUser = userRoleIds.Contains(AdminRoleId);
        List<string> grantedClaims;

        if (isAdminUser)
        {
            grantedClaims = [];
        }
        else
        {
            var userClaims = await context.UserClaims
                .AsNoTracking()
                .Where(i => i.UserId == currentUserId)
                .Select(i => i.ClaimType)
                .ToListAsync();

            var userMenuClaims = await context.UserMenus
                .AsNoTracking()
                .Where(i => i.UserId == currentUserId)
                .Select(i => i.Claim)
                .ToListAsync();

            var roleClaims = userRoleIds.Length == 0
                ? []
                : await context.RoleClaims
                    .AsNoTracking()
                    .Where(i => userRoleIds.Contains(i.RoleId))
                    .Select(i => i.ClaimType)
                    .ToListAsync();

            var roleMenuClaims = userRoleIds.Length == 0
                ? []
                : await context.RoleMenus
                    .AsNoTracking()
                    .Where(i => userRoleIds.Contains(i.RoleId))
                    .Select(i => i.Claim)
                    .ToListAsync();

            grantedClaims = userClaims
                .Concat(roleClaims)
                .Concat(userMenuClaims)
                .Concat(roleMenuClaims)
                .Select(c => c.Trim().ToLowerInvariant())
                .Where(c => c.Length > 0)
                .Distinct()
                .ToList();
        }

        var permissionRows = await context.Permissions
            .AsNoTracking()
            .Select(p => new { p.Id, p.ParentId, p.Name })
            .ToListAsync();

        var permissionNodes = permissionRows
            .Select(p => new PermissionGraphNodeDTO
            {
                Id = p.Id,
                ParentId = p.ParentId,
                Key = PermissionKeyNormalizer.Normalize(p.Name)
            })
            .Where(p => p.Key.Length > 0)
            .ToList();

        return this.AppOk(OperationResult.Succeeded(payload: new RouteAccessContextDTO
        {
            Routes = routes,
            GrantedClaims = grantedClaims,
            PermissionNodes = permissionNodes,
            IsAdmin = isAdminUser
        }));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(string id)
    {
        var normalizedId = id.ToLower();
        var relatedDetail = _permissionRouteService.All()
            .FirstOrDefault(i => i.Claim != null && i.Claim.ToLower() == normalizedId);

        if (relatedDetail == null)
        {
            return this.AppOk(OperationResult.Succeeded(payload: new PermissionRouteDTO()));
        }

        return this.AppOk(OperationResult.Succeeded(payload: relatedDetail));
    }

    [HttpPost("addDetailToClaim")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] PermissionRouteDTO body)
    {
        if (string.IsNullOrEmpty(body.Claim))
        {
            return this.AppBadRequest(OperationResult.Failed("فرم را به صورت صحیح پر کنید"));
        }

        try
        {
            var normalizedClaim = body.Claim.ToLower();
            var relatedDetail = _permissionRouteService.All()
                .Where(i => i.Claim != null && i.Claim.ToLower() == normalizedClaim)
                .ToList();

            if (relatedDetail.Count == 0)
            {
                _permissionRouteService.Add(new PermissionRoute
                {
                    Claim = body.Claim,
                    CreateDate = DateTime.Now,
                    Description = body.Description,
                    Icon = body.Icon,
                    Route = body.Route,
                    title = body.title,
                    Tooltip = body.Tooltip,
                    Priority = body.Priority,
                    IsEmployeeSpecific = body.IsEmployeeSpecific,
                    ParentMenuKey = body.ParentMenuKey,
                    PreferColor = body.PreferColor,
                    IsSpecial = body.IsSpecial,
                });
                await _permissionRouteService._unitOfWork.Save();
                return this.AppOk(OperationResult.Succeeded(payload: new PermissionRouteDTO()));
            }

            var existingRecord = relatedDetail[0];
            existingRecord.Description = body.Description;
            existingRecord.Icon = body.Icon;
            existingRecord.Route = body.Route;
            existingRecord.title = body.title;
            existingRecord.Tooltip = body.Tooltip;
            existingRecord.Priority = body.Priority;
            existingRecord.IsEmployeeSpecific = body.IsEmployeeSpecific;
            existingRecord.ParentMenuKey = body.ParentMenuKey;
            existingRecord.PreferColor = body.PreferColor;
            existingRecord.IsSpecial = body.IsSpecial;
            existingRecord.LastModifiedDate = DateTime.Now;

            _permissionRouteService._unitOfWork.Context.Update(existingRecord);
            await _permissionRouteService._unitOfWork.Save();
            return this.AppOk(OperationResult.Succeeded());
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
        {
            return this.AppOk(OperationResult.Failed("این آیتم قبلاً ثبت شده است یا در حال ثبت است. لطفاً دوباره کلیک نکنید یا صفحه را رفرش کنید."));
        }
    }

    private static List<MenuresultDTO> BuildFlatMenu(
        List<Permission> permissions,
        Dictionary<string, PermissionRoute> routeByNormalizedClaim,
        Dictionary<string, Permission> permissionByName)
    {
        var flatMenu = new List<MenuresultDTO>(permissions.Count + 1)
        {
            new MenuresultDTO
            {
                name = "Actions",
                id = permissionByName["Actions"].Id,
                claim = "actions"
            }
        };

        foreach (var permission in permissions)
        {
            var normalizedName = NormalizeClaim(permission.Name);
            if (routeByNormalizedClaim.TryGetValue(normalizedName, out var route))
            {
                flatMenu.Add(MapToMenuItem(permission, route));
            }
        }

        return flatMenu;
    }

    private static Dictionary<string, PermissionRoute> BuildRouteLookup(IEnumerable<PermissionRoute> routeDefinitions)
    {
        var lookup = new Dictionary<string, PermissionRoute>(StringComparer.Ordinal);
        foreach (var route in routeDefinitions)
        {
            if (string.IsNullOrWhiteSpace(route.Claim))
            {
                continue;
            }

            lookup[NormalizeClaim(route.Claim)] = route;
        }

        return lookup;
    }

    private static MenuresultDTO MapToMenuItem(Permission permission, PermissionRoute route)
    {
        return new MenuresultDTO
        {
            id = permission.Id,
            parentId = permission.ParentId,
            claim = NormalizeClaim(route.Claim),
            description = route.Description ?? string.Empty,
            icon = NormalizeClaim(string.IsNullOrEmpty(route.Icon) ? DefaultIcon : route.Icon),
            name = route.title ?? string.Empty,
            route = NormalizeClaim(route.Route ?? string.Empty),
            tooltip = route.Tooltip ?? string.Empty,
            priority = route.Priority,
            isEmployeeSpecific = route.IsEmployeeSpecific,
            parentMenuKey = route.ParentMenuKey,
            preferColor = route.PreferColor,
            isSpecial = route.IsSpecial
        };
    }

    private static string NormalizeClaim(string? value) =>
        (value ?? string.Empty).Trim().ToLower();

    private static List<MenuresultDTO> BuildTree(List<MenuresultDTO> flatList)
    {
        var lookup = flatList.ToDictionary(x => x.id);
        var roots = new List<MenuresultDTO>();

        foreach (var node in flatList)
        {
            if (node.parentId is Guid parentId && lookup.TryGetValue(parentId, out var parent))
            {
                parent.children ??= [];
                parent.children.Add(node);
                continue;
            }

            if (!node.parentId.HasValue)
            {
                node.name = RootMenuName;
                roots.Add(node);
            }
        }

        return roots;
    }

    public static List<MenuresultDTO> FilterMenuTree(List<MenuresultDTO> fullTree, HashSet<string> userClaims)
    {
        var result = new List<MenuresultDTO>(fullTree.Count);

        foreach (var node in fullTree)
        {
            var filteredNode = FilterNode(node, userClaims);
            if (filteredNode != null)
            {
                result.Add(filteredNode);
            }
        }

        return result;
    }

    private static MenuresultDTO? FilterNode(MenuresultDTO node, HashSet<string> userClaims)
    {
        if (IsAuthorizedByClaims(node.claim, userClaims))
        {
            return CloneSubtree(node);
        }

        if (node.children == null || node.children.Count == 0)
        {
            return null;
        }

        var filteredChildren = new List<MenuresultDTO>(node.children.Count);
        foreach (var child in node.children)
        {
            var filteredChild = FilterNode(child, userClaims);
            if (filteredChild != null)
            {
                filteredChildren.Add(filteredChild);
            }
        }

        return filteredChildren.Count > 0
            ? CopyNode(node, filteredChildren)
            : null;
    }

    private static bool IsAuthorizedByClaims(string nodeClaim, HashSet<string> userClaims)
    {
        if (string.IsNullOrWhiteSpace(nodeClaim) || userClaims.Count == 0)
        {
            return false;
        }

        var nodeKey = nodeClaim.Trim().ToLower();

        foreach (var claim in userClaims)
        {
            if (string.IsNullOrWhiteSpace(claim))
            {
                continue;
            }

            var key = claim.Trim().ToLower();

            if (nodeKey == key)
            {
                return true;
            }

            if (nodeKey.Length > key.Length && nodeKey.StartsWith(key + ".", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static MenuresultDTO? CloneSubtree(MenuresultDTO node)
    {
        if (node.children == null)
        {
            return CopyNode(node, null);
        }

        var clonedChildren = new List<MenuresultDTO>(node.children.Count);
        foreach (var child in node.children)
        {
            var clonedChild = CloneSubtree(child);
            if (clonedChild != null)
            {
                clonedChildren.Add(clonedChild);
            }
        }

        return CopyNode(node, clonedChildren);
    }

    private static MenuresultDTO CopyNode(MenuresultDTO node, List<MenuresultDTO>? children) =>
        new()
        {
            id = node.id,
            parentId = node.parentId,
            claim = node.claim,
            name = node.name,
            route = node.route,
            icon = node.icon,
            tooltip = node.tooltip,
            description = node.description,
            priority = node.priority,
            isEmployeeSpecific = node.isEmployeeSpecific,
            parentMenuKey = node.parentMenuKey,
            preferColor = node.preferColor,
            isSpecial = node.isSpecial,
            children = children
        };

    public class MenuresultDTO
    {
        public Guid id { get; set; }
        public Guid? parentId { get; set; }
        public string claim { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string? route { get; set; }
        public string? icon { get; set; }
        public string? tooltip { get; set; }
        public string? description { get; set; }
        public int priority { get; set; }
        public bool isEmployeeSpecific { get; set; }
        public string? parentMenuKey { get; set; }
        public string? preferColor { get; set; }
        public bool isSpecial { get; set; }
        public List<MenuresultDTO>? children { get; set; }
    }
}
