using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using HR.Identity.infrastructure.Data;
using HR.Identity.Core.Entities;

namespace HRMS.API.Infrastructure.Security;

/// <summary>
/// Service for logging security-related events and potential token theft attempts
/// </summary>
public class SecurityAuditService
{
    private readonly ILogger<SecurityAuditService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SecurityAuditService(
        ILogger<SecurityAuditService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Log a potential token theft attempt
    /// </summary>
    public async Task LogSuspiciousActivityAsync(
        long? userId,
        string userName,
        string activityType,
        string ipAddress,
        string userAgent,
        string details,
        string endpoint = null)
    {
        try
        {
            // Log to standard logging system
            _logger.LogWarning(
                "SECURITY ALERT: Suspicious Activity - Type: {ActivityType}, User: {UserName} (ID: {UserId}), IP: {IP}, UA: {UserAgent}, Details: {Details}, Endpoint: {Endpoint}",
                activityType, userName ?? "Unknown", userId, ipAddress, userAgent, details, endpoint ?? "N/A");

            await PersistAuditEntryAsync(new SecurityAuditLog
            {
                UserId = userId,
                UserName = Truncate(userName ?? "Unknown", 256),
                ActivityType = Truncate(activityType, 100),
                IpAddress = Truncate(ipAddress ?? "Unknown", 45),
                UserAgent = Truncate(userAgent ?? "Unknown", 500),
                Details = Truncate(details ?? string.Empty, 2000),
                Endpoint = Truncate(endpoint ?? string.Empty, 500),
                Timestamp = DateTime.UtcNow,
                Severity = Truncate(DetermineSeverity(activityType), 20),
                Metadata = string.Empty
            });
        }
        catch (Exception ex)
        {
            // Don't let logging failures affect the main application flow
            _logger.LogError(ex, "Failed to log security audit entry");
        }
    }

    /// <summary>
    /// Log successful authentication with details
    /// </summary>
    public async Task LogSuccessfulAuthenticationAsync(
        long userId,
        string userName,
        string ipAddress,
        string userAgent)
    {
        try
        {
            _logger.LogInformation(
                "SECURITY: Successful authentication - User: {UserName} (ID: {UserId}), IP: {IP}, UA: {UserAgent}",
                userName, userId, ipAddress, userAgent);

            await PersistAuditEntryAsync(new SecurityAuditLog
            {
                UserId = userId,
                UserName = Truncate(userName, 256),
                ActivityType = "SuccessfulLogin",
                IpAddress = Truncate(ipAddress ?? "Unknown", 45),
                UserAgent = Truncate(userAgent ?? "Unknown", 500),
                Details = "User logged in successfully",
                Endpoint = string.Empty,
                Timestamp = DateTime.UtcNow,
                Severity = "Info",
                Metadata = string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log successful authentication");
        }
    }

    /// <summary>
    /// Log token refresh event
    /// </summary>
    public async Task LogTokenRefreshAsync(
        long userId,
        string userName,
        string ipAddress,
        string userAgent,
        bool success,
        string reason = null)
    {
        try
        {
            var severity = success ? "Info" : "Warning";
            var activityType = success ? "TokenRefreshSuccess" : "TokenRefreshFailure";

            _logger.Log(
                success ? LogLevel.Information : LogLevel.Warning,
                "SECURITY: Token refresh {Status} - User: {UserName} (ID: {UserId}), IP: {IP}, Reason: {Reason}",
                success ? "succeeded" : "failed", userName, userId, ipAddress, reason ?? "N/A");

            await PersistAuditEntryAsync(new SecurityAuditLog
            {
                UserId = userId,
                UserName = Truncate(userName, 256),
                ActivityType = activityType,
                IpAddress = Truncate(ipAddress ?? "Unknown", 45),
                UserAgent = Truncate(userAgent ?? "Unknown", 500),
                Details = Truncate(reason ?? (success ? "Token refreshed successfully" : "Token refresh failed"), 2000),
                Endpoint = string.Empty,
                Timestamp = DateTime.UtcNow,
                Severity = Truncate(severity, 20),
                Metadata = string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log token refresh event");
        }
    }

    private async Task PersistAuditEntryAsync(SecurityAuditLog auditEntry)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CustomIdentityContext>();
        context.SecurityAuditLogs.Add(auditEntry);
        await context.SaveChangesAsync();
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value ?? string.Empty;
        }

        return value[..maxLength];
    }

    private string DetermineSeverity(string activityType)
    {
        return activityType switch
        {
            "TokenIPMismatch" => "Critical",
            "TokenUserAgentMismatch" => "Critical",
            "TokenStolen" => "Critical",
            "MultipleFailedLogins" => "High",
            "SuspiciousIPChange" => "High",
            "UnusualActivity" => "Medium",
            _ => "Low"
        };
    }
}

