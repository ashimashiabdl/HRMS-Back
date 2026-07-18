using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HR.Identity.Infrastructure.Services;

/// <summary>
/// سرویس برای Rate Limiting تغییر رمز عبور
/// جلوگیری از فراخوانی بیش از حد UpdateCurrentUserPassword
/// </summary>
public class PasswordChangeRateLimitService : IScopedServices
{
    private readonly CustomIdentityContext _context;
    private readonly IdentityContext _identityContext;
    private readonly UserManager<AspNetUsers> _userManager;
    private readonly BlockedIpSecurityService _blockedIpSecurityService;
    private readonly ILogger<PasswordChangeRateLimitService> _logger;
    
    // تنظیمات Rate Limiting
    private const int MaxRequests = 5;
    private const int TimeWindowMinutes = 15;

    public PasswordChangeRateLimitService(
        CustomIdentityContext context,
        IdentityContext identityContext,
        UserManager<AspNetUsers> userManager,
        BlockedIpSecurityService blockedIpSecurityService,
        ILogger<PasswordChangeRateLimitService> logger)
    {
        _context = context;
        _identityContext = identityContext;
        _userManager = userManager;
        _blockedIpSecurityService = blockedIpSecurityService;
        _logger = logger;
    }

    /// <summary>
    /// بررسی Rate Limit و ثبت فراخوانی
    /// در صورت بیش از حد مجاز، کاربر و IP را مسدود می‌کند
    /// </summary>
    public async Task<OperationResult> CheckRateLimitAndRecordAsync(
        long userId,
        string ipAddress,
        bool isSuccess,
        string? errorMessage = null)
    {
        try
        {
            var now = DateTime.UtcNow;
            var timeWindowStart = now.AddMinutes(-TimeWindowMinutes);

            // شمارش فراخوانی‌های این کاربر در 15 دقیقه گذشته
            var requestCount = await _context.PasswordChangeRateLimits
                .Where(r => r.AspNetUserId == userId 
                    && r.RequestDateTime >= timeWindowStart
                    && !r.IsDeleted)
                .CountAsync();

            // ثبت فراخوانی فعلی
            var rateLimitLog = new PasswordChangeRateLimit
            {
                AspNetUserId = userId,
                RequestIPAddress = ipAddress,
                RequestDateTime = now,
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage,
                title = $"PasswordChangeRateLimit_{userId}_{now:yyyyMMddHHmmss}",
                CreateDate = now,
                IPAddress = ipAddress
            };

            _context.PasswordChangeRateLimits.Add(rateLimitLog);
            await _context.SaveChangesAsync();

            // بررسی Rate Limit
            if (requestCount >= MaxRequests)
            {
                // بیش از حد مجاز - مسدود کردن کاربر و IP
                _logger.LogError(
                    "SECURITY ALERT: Password change rate limit exceeded - UserId: {UserId}, IP: {IP}, RequestCount: {RequestCount}, MaxAllowed: {MaxAllowed}, TimeWindow: {TimeWindow} minutes",
                    userId, ipAddress, requestCount + 1, MaxRequests, TimeWindowMinutes);

                // مسدود کردن کاربر
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // مسدود دائمی تا زمانی که دستی باز شود
                    user.DeactivationReason = $"تعداد تلاش برای تغییر رمز عبور بیش از حد مجاز ({MaxRequests} بار در {TimeWindowMinutes} دقیقه). IP: {ipAddress}";
                    await _userManager.UpdateAsync(user);
                    
                    _logger.LogError(
                        "SECURITY ACTION: User locked due to password change rate limit - UserId: {UserId}, UserName: {UserName}",
                        userId, user.UserName);
                }

                // مسدود کردن IP - استفاده از BlockedIpSecurityService
                // با فراخوانی CheckAndBlockIpIfNeededAsync که به صورت دستی IP را مسدود می‌کند
                await BlockIpAsync(ipAddress, userId, user?.UserName);

                return OperationResult.Failed(
                    $"تعداد تلاش برای تغییر رمز عبور بیش از حد مجاز است ({MaxRequests} بار در {TimeWindowMinutes} دقیقه). حساب کاربری و آدرس IP شما مسدود شد. لطفاً با پشتیبانی تماس بگیرید.");
            }

            // در صورت نزدیک شدن به حد مجاز، هشدار بده
            if (requestCount + 1 >= MaxRequests - 1)
            {
                _logger.LogWarning(
                    "Password change rate limit warning - UserId: {UserId}, IP: {IP}, RequestCount: {RequestCount}, MaxAllowed: {MaxAllowed}",
                    userId, ipAddress, requestCount + 1, MaxRequests);
            }

            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error in CheckRateLimitAndRecordAsync - UserId: {UserId}, IP: {IP}",
                userId, ipAddress);
            // در صورت خطا، اجازه می‌دهیم تا درخواست ادامه یابد (fail-open)
            return OperationResult.Succeeded();
        }
    }

    /// <summary>
    /// مسدود کردن IP به صورت مستقیم
    /// </summary>
    private async Task BlockIpAsync(string ipAddress, long? userId = null, string? userName = null)
    {
        try
        {
            // بررسی اینکه آیا IP قبلاً مسدود شده است
            var existingBlockedIp = await _identityContext.Set<BlockedIp>()
                .FirstOrDefaultAsync(b => b.IpAddress == ipAddress && b.IsActive && !b.IsDeleted);

            if (existingBlockedIp == null)
            {
                var blockedIp = new BlockedIp
                {
                    IpAddress = ipAddress,
                    IsActive = true,
                    title = $"Blocked for Password Change Rate Limit - {ipAddress}",
                    CreateDate = DateTime.UtcNow,
                    IPAddress = ipAddress
                };

                _identityContext.Set<BlockedIp>().Add(blockedIp);
                await _identityContext.SaveChangesAsync();

                _logger.LogError(
                    "SECURITY ACTION: IP blocked due to password change rate limit - IP: {IP}, UserId: {UserId}, UserName: {UserName}",
                    ipAddress, userId, userName);
            }
            else
            {
                _logger.LogWarning(
                    "IP already blocked - IP: {IP}",
                    ipAddress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error blocking IP - IP: {IP}",
                ipAddress);
        }
    }

    /// <summary>
    /// پاک‌سازی لاگ‌های قدیمی (بیش از 24 ساعت)
    /// </summary>
    public async Task CleanupOldLogsAsync()
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddHours(-24);
            var oldLogs = await _context.PasswordChangeRateLimits
                .Where(r => r.RequestDateTime < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.PasswordChangeRateLimits.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation(
                    "Cleaned up {Count} old password change rate limit logs",
                    oldLogs.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old password change rate limit logs");
        }
    }
}

