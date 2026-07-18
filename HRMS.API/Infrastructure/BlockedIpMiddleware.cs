using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace HRMS.API.Infrastructure;

/// <summary>
/// Middleware برای بررسی IPهای بلاک شده قبل از پردازش درخواست‌ها
/// </summary>
public class BlockedIpMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BlockedIpMiddleware> _logger;

    public BlockedIpMiddleware(
        RequestDelegate next,
        ILogger<BlockedIpMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var userResolverService = context.RequestServices.GetRequiredService<UserResolverService>();
            var ipAddress = userResolverService.GetIP();

            // بررسی اینکه آیا IP بلاک شده است (فقط اگر IP معتبر باشد)
            if (!string.IsNullOrWhiteSpace(ipAddress) && ipAddress != "Notfound" && ipAddress != "Unknown")
            {
                var blockedIpSecurityService = context.RequestServices.GetRequiredService<BlockedIpSecurityService>();
                var isBlocked = await blockedIpSecurityService.IsIpBlockedAsync(ipAddress);

                if (isBlocked)
                {
                    _logger.LogWarning(
                        "SECURITY ALERT - Blocked IP attempt - IP:{IP} - Path:{Path} - Method:{Method} - UserAgent:{UserAgent}",
                        ipAddress,
                        context.Request.Path,
                        context.Request.Method,
                        context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown");

                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        success = false,
                        message = "دسترسی از این آدرس IP مسدود شده است",
                        code = "ip_blocked"
                    };

                    await context.Response.WriteAsJsonAsync(response);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            // در صورت بروز خطا در بررسی IP، لاگ کرده و ادامه می‌دهیم (fail-open strategy)
            _logger.LogError(ex,
                "Error in BlockedIpMiddleware while checking IP - Path:{Path} - Method:{Method}",
                context.Request.Path, context.Request.Method);
            // Continue processing - don't block requests if IP check fails
        }

        await _next(context);
    }
}
