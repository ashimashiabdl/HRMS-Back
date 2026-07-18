using Dapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Security.Claims;

namespace HR.SharedKernel.Service;

/// <summary>
/// Resolves the current HTTP user context (id, employee id, IP, etc.) for the active request scope.
/// Must be registered as <c>Scoped</c> alongside <see cref="IHttpContextAccessor"/>.
/// </summary>
public class UserResolverService(
    IHttpContextAccessor context,
    IDapper dapper,
    IServiceProvider serviceProvider,
    ILogger<UserResolverService> logger)
{
    private readonly IHttpContextAccessor _context = context;
    private readonly IDapper _dapper = dapper;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<UserResolverService> _logger = logger;

    public long GetUserId()
    {
        var httpContext = GetHttpContext();
        if (httpContext == null)
        {
            return -1;
        }

        var userIdClaim = FindClaim(httpContext, "userId", "currentUserId");
        if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
        {
            var encryptionService = GetEncryptionService();
            if (encryptionService != null
                && encryptionService.TryDecryptUserId(userIdClaim.Value, out var decryptedUserId)
                && decryptedUserId > 0)
            {
                return decryptedUserId;
            }

            if (long.TryParse(userIdClaim.Value, out var plainUserId) && plainUserId > 0)
            {
                return plainUserId;
            }

            _logger.LogDebug(
                "Could not resolve userId from claim value (length={Length})",
                userIdClaim.Value.Length);
        }

        if (TryGetItemLong(httpContext, "currentUserId", out var itemUserId))
        {
            return itemUserId;
        }

        return -1;
    }

    public bool IsAdmin()
    {
        var httpContext = GetHttpContext();
        if (httpContext?.User == null)
        {
            return false;
        }

        var isAdminClaim = FindClaim(httpContext, "isAdmin");
        if (isAdminClaim != null
            && string.Equals(isAdminClaim.Value, "true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var userId = GetUserId();
        if (userId <= 0)
        {
            return false;
        }

        try
        {
            const string query = "SELECT COUNT(*) FROM [Identity].[User_Role] WHERE [UserId] = @UserId AND [RoleId] = 1";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int64);
            return _dapper.Get<int>(query, parameters, CommandType.Text) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve admin role for user {UserId}", userId);
            return false;
        }
    }

    public long currentEmployeeId()
    {
        var httpContext = GetHttpContext();
        if (httpContext == null)
        {
            return -1;
        }

        if (TryGetClaimLong(httpContext, out var employeeId, "currentUserEmployeeId"))
        {
            return employeeId;
        }

        // HR.UserProfile uses currentUserId for emp.Employee.Id — not HRMS.API Dashboard tokens.
        var isDashboardToken = FindClaim(httpContext, "userId") != null;
        if (!isDashboardToken)
        {
            if (TryGetClaimLong(httpContext, out employeeId, "currentUserId"))
            {
                return employeeId;
            }

            if (TryGetItemLong(httpContext, "currentUserId", out employeeId))
            {
                return employeeId;
            }
        }

        if (TryGetItemLong(httpContext, "currentUserEmployeeId", out employeeId))
        {
            return employeeId;
        }

        return -1;
    }

    public string GetUser()
    {
        var userId = GetUserId();
        if (userId <= 0)
        {
            return "Unknown";
        }

        try
        {
            const string query = "SELECT [UserName] FROM [Identity].[AspNetUsers] WHERE [Id] = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int64);
            var username = _dapper.Get<string>(query, parameters, CommandType.Text);
            return string.IsNullOrEmpty(username) ? "Unknown" : username;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve username for user {UserId}", userId);
            return "Unknown";
        }
    }

    public string fullname()
    {
        var httpContext = GetHttpContext();
        if (httpContext == null)
        {
            return "Unknown";
        }

        var fullNameClaim = FindClaim(httpContext, "fullname");
        if (fullNameClaim != null && !string.IsNullOrWhiteSpace(fullNameClaim.Value))
        {
            return fullNameClaim.Value;
        }

        if (httpContext.Items.TryGetValue("CurrentUserFullName", out var itemFullName)
            && itemFullName is string fullNameFromItems
            && !string.IsNullOrWhiteSpace(fullNameFromItems))
        {
            return fullNameFromItems;
        }

        return httpContext.User?.Identity?.Name ?? "Unknown";
    }

    public string GetIP()
    {
        var httpContext = GetHttpContext();
        if (httpContext == null)
        {
            return "Notfound";
        }

        string? rawIp = null;

        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            rawIp = forwardedFor.Split(',')[0].Trim();
        }

        if (string.IsNullOrWhiteSpace(rawIp))
        {
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                rawIp = realIp.Trim();
            }
        }

        if (string.IsNullOrWhiteSpace(rawIp))
        {
            var remoteIp = httpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                rawIp = remoteIp.IsIPv4MappedToIPv6
                    ? remoteIp.MapToIPv4().ToString()
                    : remoteIp.ToString();
            }
        }

        return NormalizeIpAddress(rawIp);
    }

    public string GetUserAgent()
    {
        var httpContext = GetHttpContext();
        if (httpContext == null)
        {
            return "unknown";
        }

        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrEmpty(userAgent))
        {
            return "unknown";
        }

        return userAgent.Length > 500 ? userAgent[..500] : userAgent;
    }

    private HttpContext? GetHttpContext() => _context.HttpContext;

    private UserIdEncryptionService? GetEncryptionService()
        => _serviceProvider.GetService<UserIdEncryptionService>();

    private static Claim? FindClaim(HttpContext httpContext, params string[] claimTypes)
    {
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        foreach (var claimType in claimTypes)
        {
            var claim = user.Claims.FirstOrDefault(c =>
                string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase));
            if (claim != null)
            {
                return claim;
            }
        }

        return null;
    }

    private static bool TryGetClaimLong(HttpContext httpContext, out long value, params string[] claimTypes)
    {
        value = -1;
        var claim = FindClaim(httpContext, claimTypes);
        return claim != null && long.TryParse(claim.Value, out value) && value > 0;
    }

    private static bool TryGetItemLong(HttpContext httpContext, string key, out long value)
    {
        value = -1;
        if (!httpContext.Items.TryGetValue(key, out var item) || item == null)
        {
            return false;
        }

        return long.TryParse(item.ToString(), out value) && value > 0;
    }

    private static string NormalizeIpAddress(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)
            || string.Equals(ipAddress, "unknown", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ipAddress, "Notfound", StringComparison.OrdinalIgnoreCase))
        {
            return "Notfound";
        }

        ipAddress = ipAddress.Trim();

        if (ipAddress.Contains(':'))
        {
            var parts = ipAddress.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out _))
            {
                ipAddress = parts[0];
            }
        }

        if (IPAddress.TryParse(ipAddress, out var parsedIp))
        {
            if (parsedIp.IsIPv4MappedToIPv6)
            {
                return parsedIp.MapToIPv4().ToString();
            }

            return parsedIp.ToString();
        }

        return ipAddress;
    }

    /// <summary>
    /// EF design-time factories only — no real HTTP user or database access.
    /// </summary>
    public static UserResolverService CreateForDesignTime()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddLogging();
        services.AddSingleton<IDapper, DesignTimeDapper>();
        var provider = services.BuildServiceProvider();
        return new UserResolverService(
            provider.GetRequiredService<IHttpContextAccessor>(),
            provider.GetRequiredService<IDapper>(),
            provider,
            provider.GetRequiredService<ILogger<UserResolverService>>());
    }

    private sealed class DesignTimeDapper : IDapper
    {
        public DbConnection GetDbconnection()
            => throw new InvalidOperationException("Dapper is not available in design-time context.");

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
            => default!;

        public List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
            => [];

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
            => 0;

        public T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
            => default!;

        public T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
            => default!;

        public void Dispose()
        {
        }
    }
}
