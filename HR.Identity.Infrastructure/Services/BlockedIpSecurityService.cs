using HR.Identity.Core.Entities;

using HR.Identity.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace HR.Identity.infrastructure.Services;

/// <summary>
/// سرویس امنیتی برای مدیریت IPهای بلاک شده و بررسی تلاش‌های ناموفق لاگین
/// </summary>
public class BlockedIpSecurityService : IScopedServices
{
    private readonly IdentityContext _identityContext;
    private readonly CustomIdentityContext _customIdentityContext;
    private readonly UserResolverService _userResolverService;
    private readonly ILogger<BlockedIpSecurityService> _logger;
    private readonly IDistributedCache? _cache;
    
    // تنظیمات امنیتی
    private const int FAILED_ATTEMPT_THRESHOLD = 5; // تعداد تلاش‌های ناموفق مجاز
    private const int TIME_WINDOW_MINUTES = 15; // بازه زمانی بررسی (دقیقه)
    private const string CACHE_KEY_PREFIX = "blocked_ip:";
    private const string BLOCKED_IP_LIST_CACHE_KEY = "blocked_ip:active_list";
    private const string NOT_BLOCKED_CACHE_PREFIX = "blocked_ip:not_blocked:";
    private const int CACHE_DURATION_HOURS = 24; // مدت زمان کش (ساعت)
    private static readonly TimeSpan BlockedIpListCacheTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan NotBlockedIpCacheTtl = TimeSpan.FromMinutes(1);

    public BlockedIpSecurityService(
        IdentityContext identityContext,
        CustomIdentityContext customIdentityContext,
        UserResolverService userResolverService,
        ILogger<BlockedIpSecurityService> logger,
        IDistributedCache? cache = null)
    {
        _identityContext = identityContext;
        _customIdentityContext = customIdentityContext;
        _userResolverService = userResolverService;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// بررسی می‌کند که آیا IP بلاک شده است یا نه
    /// </summary>
    public async Task<bool> IsIpBlockedAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "unknown" || ipAddress == "Notfound")
            return false;

        // نرمال‌سازی IP آدرس برای مقایسه یکسان
        var normalizedIp = NormalizeIpAddress(ipAddress);
        if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown" || normalizedIp == "Notfound")
            return false;

        // بررسی کش اول - IP مشخصاً بلاک شده
        if (_cache != null)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedIp}";
            var cachedValue = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                return true; // IP در کش بلاک شده است
            }

            var notBlockedKey = $"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}";
            var notBlockedValue = await _cache.GetStringAsync(notBlockedKey);
            if (!string.IsNullOrEmpty(notBlockedValue))
            {
                return false;
            }
        }

        var blockedIpSet = await GetActiveBlockedIpSetAsync();
        if (blockedIpSet.Contains(normalizedIp))
        {
            if (_cache != null)
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedIp}";
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_DURATION_HOURS)
                };
                await _cache.SetStringAsync(cacheKey, "1", options);
            }

            return true;
        }

        if (_cache != null)
        {
            var notBlockedKey = $"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = NotBlockedIpCacheTtl
            };
            await _cache.SetStringAsync(notBlockedKey, "1", options);
        }

        return false;
    }

    private async Task<HashSet<string>> GetActiveBlockedIpSetAsync()
    {
        if (_cache != null)
        {
            var cachedList = await _cache.GetStringAsync(BLOCKED_IP_LIST_CACHE_KEY);
            if (!string.IsNullOrEmpty(cachedList))
            {
                var deserialized = JsonSerializer.Deserialize<HashSet<string>>(cachedList);
                if (deserialized is not null)
                    return deserialized;
            }
        }

        var blockedIps = await _identityContext.Set<BlockedIp>()
            .AsNoTracking()
            .Where(b => b.IsActive && !b.IsDeleted)
            .Select(b => b.IpAddress)
            .ToListAsync();

        var blockedIpSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var blockedIp in blockedIps)
        {
            var normalizedBlockedIp = NormalizeIpAddress(blockedIp);
            if (!string.IsNullOrWhiteSpace(normalizedBlockedIp)
                && normalizedBlockedIp != "unknown"
                && normalizedBlockedIp != "Notfound")
            {
                blockedIpSet.Add(normalizedBlockedIp);
            }
        }

        if (_cache != null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = BlockedIpListCacheTtl
            };
            await _cache.SetStringAsync(BLOCKED_IP_LIST_CACHE_KEY, JsonSerializer.Serialize(blockedIpSet), options);
        }

        return blockedIpSet;
    }

    private async Task InvalidateBlockedIpListCacheAsync()
    {
        if (_cache == null)
            return;

        await _cache.RemoveAsync(BLOCKED_IP_LIST_CACHE_KEY);
    }

    /// <summary>
    /// نرمال‌سازی IP آدرس برای مقایسه یکسان
    /// </summary>
    private string NormalizeIpAddress(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "unknown" || ipAddress == "Notfound")
        {
            return "Notfound";
        }

        // حذف فاصله‌های اضافی
        ipAddress = ipAddress.Trim();

        // حذف پورت اگر وجود دارد (مثال: 192.168.1.1:8080)
        if (ipAddress.Contains(':'))
        {
            var parts = ipAddress.Split(':');
            // اگر فقط یک : وجود دارد و بعد از آن عدد است، پورت است
            if (parts.Length == 2 && int.TryParse(parts[1], out _))
            {
                ipAddress = parts[0];
            }
            // در غیر این صورت ممکن است IPv6 باشد
        }

        // تلاش برای پارس کردن و نرمال‌سازی IP
        if (System.Net.IPAddress.TryParse(ipAddress, out var parsedIp))
        {
            // اگر IPv6-mapped IPv4 است، به IPv4 تبدیل کن
            if (parsedIp.IsIPv4MappedToIPv6)
            {
                return parsedIp.MapToIPv4().ToString();
            }
            // اگر IPv4 است، به صورت استاندارد برگردان
            if (parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return parsedIp.ToString();
            }
            // برای IPv6، به صورت استاندارد برگردان
            return parsedIp.ToString();
        }

        // اگر نتوانستیم پارس کنیم، همان مقدار اصلی را برگردان (اما trim شده)
        return ipAddress;
    }

    /// <summary>
    /// بررسی تعداد تلاش‌های ناموفق در بازه زمانی مشخص شده
    /// </summary>
    public async Task<int> GetFailedLoginAttemptsCountAsync(string ipAddress, long? userId = null, long? employeeId = null)
    {
        // نرمال‌سازی IP آدرس ورودی
        var normalizedIp = NormalizeIpAddress(ipAddress);
        if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown")
        {
            return 0;
        }

        var timeWindow = DateTime.Now.AddMinutes(-TIME_WINDOW_MINUTES);
        
        // دریافت تمام تلاش‌های ناموفق در بازه زمانی
        var query = _customIdentityContext.Set<UserLoginHistory>()
            .Where(h => !h.IsSuccess && h.CreateDate >= timeWindow);

        if (employeeId.HasValue)
        {
            query = query.Where(h => h.EmployeeId == employeeId.Value);
        }
        else if (userId.HasValue)
        {
            query = query.Where(h => h.AspNetUserId == userId.Value);
        }

        // مقایسه IPهای نرمال‌سازی شده در حافظه
        var failedAttempts = await query.ToListAsync();
        return failedAttempts.Count(h => NormalizeIpAddress(h.IPAddress) == normalizedIp);
    }

    /// <summary>
    /// بررسی و بلاک کردن IP در صورت نیاز
    /// </summary>
    public async Task<OperationResult> CheckAndBlockIpIfNeededAsync(string ipAddress, long? userId = null, string? userName = null)
    {
        try
        {
            // نرمال‌سازی IP آدرس
            var normalizedIp = NormalizeIpAddress(ipAddress);
            if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown" || normalizedIp == "Notfound")
            {
                return OperationResult.Succeeded();
            }

            // بررسی تعداد تلاش‌های ناموفق
            var failedAttempts = await GetFailedLoginAttemptsCountAsync(normalizedIp, userId);
            
            if (failedAttempts >= FAILED_ATTEMPT_THRESHOLD)
            {
                // بررسی اینکه آیا IP قبلاً بلاک شده است (با مقایسه نرمال‌سازی شده)
                var allBlockedIps = await _identityContext.Set<BlockedIp>()
                    .Where(b => !b.IsDeleted)
                    .ToListAsync();

                var existingBlock = allBlockedIps.FirstOrDefault(b => 
                    NormalizeIpAddress(b.IpAddress) == normalizedIp);

                if (existingBlock == null)
                {
                    // ایجاد رکورد جدید بلاک IP (با IP نرمال‌سازی شده)
                    var blockedIp = new BlockedIp
                    {
                        IpAddress = normalizedIp,
                        IsActive = true,
                        CreateDate = DateTime.Now,
                        IPAddress = _userResolverService.GetIP(),
                        title = $"Auto-blocked after {failedAttempts} failed attempts"
                    };
                    
                    _identityContext.Set<BlockedIp>().Add(blockedIp);
                    await _identityContext.SaveChangesAsync();

                    // ذخیره در کش
                    if (_cache != null)
                    {
                        var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedIp}";
                        var options = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_DURATION_HOURS)
                        };
                        await _cache.SetStringAsync(cacheKey, "1", options);
                        await _cache.RemoveAsync($"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}");
                        await InvalidateBlockedIpListCacheAsync();
                    }

                    _logger.LogWarning(
                        "SECURITY ALERT - IP blocked automatically - IP:{IP} - FailedAttempts:{FailedAttempts} - UserId:{UserId} - UserName:{UserName}",
                        normalizedIp, failedAttempts, userId, userName);
                }
                else if (!existingBlock.IsActive)
                {
                    // فعال کردن مجدد بلاک
                    existingBlock.IsActive = true;
                    existingBlock.LastModifiedDate = DateTime.Now;
                    await _identityContext.SaveChangesAsync();

                    // به‌روزرسانی کش
                    if (_cache != null)
                    {
                        var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedIp}";
                        var options = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_DURATION_HOURS)
                        };
                        await _cache.SetStringAsync(cacheKey, "1", options);
                        await _cache.RemoveAsync($"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}");
                        await InvalidateBlockedIpListCacheAsync();
                    }
                }

                // اگر کاربر پیدا شد، کاربر را هم غیرفعال کن
                if (userId.HasValue)
                {
                    var user = await _identityContext.Set<AspNetUsers>()
                        .FirstOrDefaultAsync(u => u.Id == userId.Value);
                    
                    if (user != null && !user.LockoutEnabled)
                    {
                        user.LockoutEnabled = true;
                        user.DeactivationReason = $"غیرفعال شدن به دلیل تلاش‌های ناموفق لاگین از IP مسدود شده ({failedAttempts} تلاش ناموفق در {TIME_WINDOW_MINUTES} دقیقه). IP: {normalizedIp}";
                        _identityContext.Set<AspNetUsers>().Update(user);
                        await _identityContext.SaveChangesAsync();

                        _logger.LogWarning(
                            "SECURITY ALERT - User locked due to failed login attempts - UserId:{UserId} - UserName:{UserName} - IP:{IP}",
                            userId, user.UserName, normalizedIp);
                    }
                }

                return OperationResult.Failed($"IP مسدود شد به دلیل {failedAttempts} تلاش ناموفق در {TIME_WINDOW_MINUTES} دقیقه اخیر");
            }

            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CheckAndBlockIpIfNeededAsync for IP:{IP}", ipAddress);
            return OperationResult.Failed("خطا در بررسی امنیتی");
        }
    }

    /// <summary>
    /// ثبت تلاش لاگین در تاریخچه
    /// </summary>
    public async Task RecordLoginAttemptAsync(string ipAddress, long? userId, bool isSuccess, string? userAgent = null, string? failReason = null)
    {
        try
        {
            // نرمال‌سازی IP آدرس قبل از ذخیره
            var normalizedIp = NormalizeIpAddress(ipAddress);
            if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown" || normalizedIp == "Notfound")
            {
                normalizedIp = ipAddress ?? "Notfound";
            }

            var loginHistory = new UserLoginHistory
            {
                AspNetUserId = userId,
                IsSuccess = isSuccess,
                IPAddress = normalizedIp,
                UserAgent = userAgent,
                FailReason = isSuccess ? null : failReason,
                CreateDate = DateTime.Now,
                title = isSuccess ? "ورود موفق" : "ورود ناموفق"
            };

            _customIdentityContext.Set<UserLoginHistory>().Add(loginHistory);
            await _customIdentityContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording login attempt - IP:{IP} - UserId:{UserId}", ipAddress, userId);
        }
    }

    /// <summary>
    /// ثبت تلاش لاگین کارمند (پورتال UserProfile) بر اساس EmployeeId
    /// </summary>
    public async Task RecordEmployeeLoginAttemptAsync(string ipAddress, long? employeeId, bool isSuccess, string? userAgent = null, string? failReason = null)
    {
        try
        {
            var normalizedIp = NormalizeIpAddress(ipAddress);
            if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown" || normalizedIp == "Notfound")
            {
                normalizedIp = ipAddress ?? "Notfound";
            }

            var loginHistory = new UserLoginHistory
            {
                AspNetUserId = null,
                EmployeeId = employeeId,
                IsSuccess = isSuccess,
                IPAddress = normalizedIp,
                UserAgent = userAgent,
                FailReason = isSuccess ? null : failReason,
                CreateDate = DateTime.Now,
                title = isSuccess ? "ورود موفق" : "ورود ناموفق"
            };

            _customIdentityContext.Set<UserLoginHistory>().Add(loginHistory);
            await _customIdentityContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording employee login attempt - IP:{IP} - EmployeeId:{EmployeeId}", ipAddress, employeeId);
        }
    }

    /// <summary>
    /// حذف IP از لیست بلاک شده
    /// </summary>
    public async Task<bool> UnblockIpAsync(string ipAddress)
    {
        try
        {
            // نرمال‌سازی IP آدرس
            var normalizedIp = NormalizeIpAddress(ipAddress);
            if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown" || normalizedIp == "Notfound")
            {
                return false;
            }

            // دریافت تمام IPهای بلاک شده و مقایسه با IP نرمال‌سازی شده
            var allBlockedIps = await _identityContext.Set<BlockedIp>()
                .Where(b => !b.IsDeleted)
                .ToListAsync();

            var blockedIp = allBlockedIps.FirstOrDefault(b => 
                NormalizeIpAddress(b.IpAddress) == normalizedIp);

            if (blockedIp != null)
            {
                blockedIp.IsActive = false;
                blockedIp.LastModifiedDate = DateTime.Now;
                await _identityContext.SaveChangesAsync();

                // حذف از کش
                if (_cache != null)
                {
                    var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedIp}";
                    await _cache.RemoveAsync(cacheKey);
                    await _cache.RemoveAsync($"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}");
                    await InvalidateBlockedIpListCacheAsync();
                }

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking IP:{IP}", ipAddress);
            return false;
        }
    }

    /// <summary>
    /// به‌روزرسانی کش برای یک IP خاص (استفاده در Create/Update)
    /// </summary>
    public async Task UpdateCacheForIpAsync(string ipAddress, bool isActive)
    {
        try
        {
            if (_cache == null)
                return;

            // نرمال‌سازی IP آدرس
            var normalizedIp = NormalizeIpAddress(ipAddress);
            if (string.IsNullOrWhiteSpace(normalizedIp) || normalizedIp == "unknown" || normalizedIp == "Notfound")
                return;

            var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedIp}";

            if (isActive)
            {
                // اضافه کردن به کش
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CACHE_DURATION_HOURS)
                };
                await _cache.SetStringAsync(cacheKey, "1", options);
                await _cache.RemoveAsync($"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}");
                await InvalidateBlockedIpListCacheAsync();
            }
            else
            {
                // حذف از کش
                await _cache.RemoveAsync(cacheKey);
                await _cache.RemoveAsync($"{NOT_BLOCKED_CACHE_PREFIX}{normalizedIp}");
                await InvalidateBlockedIpListCacheAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cache for IP:{IP}", ipAddress);
        }
    }
}

