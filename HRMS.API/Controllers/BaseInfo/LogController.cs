using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using HR.SharedKernel.Data;
using Newtonsoft.Json;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/Log")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("لاگ سیستم")]
public class LogController(BaseInfoContext Context, ILogger<LogController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private BaseInfoContext _context = Context;

    /// <summary>
    /// پاک‌سازی فیلدهای حساس در لاگ‌های حسابرسی
    /// برای امنیت، فیلد PasswordHash در جدول AspNetUsers با *** جایگزین می‌شود
    /// </summary>
    /// <param name="values">مقادیر فیلدها</param>
    /// <param name="tableName">نام جدول</param>
    /// <returns>مقادیر پاک‌سازی شده</returns>
    private object SanitizeSensitiveData(object values, string tableName)
    {
        if (values == null) return null;

        // فقط برای جدول AspNetUsers فیلد PasswordHash را مخفی کن
        if (tableName.Contains("AspNetUsers"))
        {
            if (values is Dictionary<string, object> dict)
            {
                var sanitizedDict = new Dictionary<string, object>(dict);
                
                // جایگزینی فیلد PasswordHash با *** برای امنیت
                if (sanitizedDict.ContainsKey("PasswordHash"))
                {
                    sanitizedDict["PasswordHash"] = "***";
                }
                
                // در صورت نیاز می‌توان فیلدهای حساس دیگر را نیز اضافه کرد
                // مثال: SecurityStamp, ConcurrencyStamp, etc.
                
                return sanitizedDict;
            }
        }

        // برای سایر موجودیت‌ها هیچ تغییری اعمال نمی‌شود
        return values;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> Get(long id)
    {
        var entity = await _context.Logs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return this.AppNotFound();
        }

        try
        {
            var clientIp = UserResolverService.GetIP();
            var currentUserName = UserResolverService.GetUser();
            var currentFullName = UserResolverService.fullname();
            _logger.LogInformation(
                "مشاهده جزئیات لاگ - کاربر:{UserId} - نام کاربری:{UserName} - نام کامل:{FullName} - IP:{IP} - شناسه:{LogId}",
                currentUserId,
                currentUserName,
                currentFullName,
                clientIp,
                id);
        }
        catch { }

        return this.AppOk(OperationResult.Succeeded(payload: entity));
    }
    /// <summary>
    /// خلاصه آماری لاگ‌ها برای داشبورد نظارتی
    /// </summary>
    /// <param name="periodHours">بازه زمانی به ساعت (پیش‌فرض ۲۴ ساعت؛ مقادیر رایج: ۲۴، ۱۶۸، ۷۲۰)</param>
    [HttpGet, Route("GetDashboardSummary")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetDashboardSummary([FromQuery] int periodHours = 24)
    {
        periodHours = periodHours switch
        {
            168 => 168,
            720 => 720,
            _ => 24
        };

        var fromDate = DateTime.UtcNow.AddHours(-periodHours);
        var baseQuery = _context.Logs.AsNoTracking().Where(l => l.CreatedOn >= fromDate);

        var totalCount = await baseQuery.CountAsync();

        var levelBreakdown = await baseQuery
            .GroupBy(l => l.Level ?? "Unknown")
            .Select(g => new { Level = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var errorCount = levelBreakdown
            .Where(x => x.Level.Equals("Error", StringComparison.OrdinalIgnoreCase)
                || x.Level.Equals("Fatal", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Count);

        var warningCount = levelBreakdown
            .Where(x => x.Level.Equals("Warn", StringComparison.OrdinalIgnoreCase)
                || x.Level.Equals("Warning", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Count);

        var infoCount = levelBreakdown
            .Where(x => x.Level.Equals("Info", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Count);

        var httpQuery = baseQuery.Where(l => l.StatusCode != null);
        var httpCount = await httpQuery.CountAsync();
        var successCount = await httpQuery.CountAsync(l => l.Success == true);
        var failedHttpCount = await httpQuery.CountAsync(l => l.Success == false);

        var durationStats = await baseQuery
            .Where(l => l.DurationMs != null && l.DurationMs > 0)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Avg = (int?)g.Average(l => l.DurationMs),
                Max = g.Max(l => l.DurationMs),
                Min = g.Min(l => l.DurationMs)
            })
            .FirstOrDefaultAsync();

        var statusCodeBreakdown = await baseQuery
            .Where(l => l.StatusCode != null)
            .GroupBy(l => l.StatusCode)
            .Select(g => new { StatusCode = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var methodBreakdown = await baseQuery
            .Where(l => l.Method != null && l.Method != "")
            .GroupBy(l => l.Method)
            .Select(g => new { Method = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(6)
            .ToListAsync();

        var topLoggers = await baseQuery
            .Where(l => l.Logger != null && l.Logger != "")
            .GroupBy(l => l.Logger)
            .Select(g => new { Logger = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync();

        var topUsers = await baseQuery
            .Where(l => l.User != null && l.User != "")
            .GroupBy(l => l.User)
            .Select(g => new { User = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync();

        var topErrorUrls = await baseQuery
            .Where(l => l.Url != null && l.Url != ""
                && (l.Level == "Error" || l.Level == "Fatal" || l.Success == false))
            .GroupBy(l => l.Url)
            .Select(g => new { Url = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(8)
            .ToListAsync();

        var errorMessagesRaw = await baseQuery
            .Where(l => l.Message != null && l.Message != ""
                && (l.Level == "Error" || l.Level == "Fatal"))
            .Select(l => l.Message)
            .Take(1000)
            .ToListAsync();

        var topErrorMessages = errorMessagesRaw
            .Select(m => m.Length > 120 ? m[..120] : m)
            .GroupBy(m => m)
            .Select(g => new { Message = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(6)
            .ToList();

        var recentCritical = await baseQuery
            .Where(l => l.Level == "Error" || l.Level == "Fatal")
            .OrderByDescending(l => l.CreatedOn)
            .Take(10)
            .Select(l => new
            {
                l.Id,
                l.Level,
                l.Message,
                l.Logger,
                l.Url,
                l.StatusCode,
                l.CreatedOn,
                l.User,
                l.IP
            })
            .ToListAsync();

        List<object> timeSeries;
        if (periodHours <= 24)
        {
            var hourly = await baseQuery
                .GroupBy(l => new { l.CreatedOn.Year, l.CreatedOn.Month, l.CreatedOn.Day, l.CreatedOn.Hour })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    g.Key.Hour,
                    Count = g.Count(),
                    ErrorCount = g.Count(x => x.Level == "Error" || x.Level == "Fatal")
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Day)
                .ThenBy(x => x.Hour)
                .ToListAsync();

            timeSeries = hourly.Select(x =>
            {
                var bucket = new DateTime(x.Year, x.Month, x.Day, x.Hour, 0, 0);
                return (object)new
                {
                    label = bucket.ToString("HH:00"),
                    Bucket = bucket,
                    x.Count,
                    x.ErrorCount
                };
            }).ToList();
        }
        else
        {
            var daily = await baseQuery
                .GroupBy(l => new { l.CreatedOn.Year, l.CreatedOn.Month, l.CreatedOn.Day })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    Count = g.Count(),
                    ErrorCount = g.Count(x => x.Level == "Error" || x.Level == "Fatal")
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Day)
                .ToListAsync();

            timeSeries = daily.Select(x =>
            {
                var bucket = new DateTime(x.Year, x.Month, x.Day);
                return (object)new
                {
                    label = bucket.ToString("yyyy-MM-dd"),
                    Bucket = bucket,
                    x.Count,
                    x.ErrorCount
                };
            }).ToList();
        }

        var auditCount = await _context.AuditLogs.AsNoTracking()
            .CountAsync(a => a.DateTime >= fromDate);

        var errorRate = totalCount > 0 ? Math.Round((double)errorCount / totalCount * 100, 1) : 0;
        var successRate = httpCount > 0 ? Math.Round((double)successCount / httpCount * 100, 1) : (double?)null;

        var healthStatus = errorRate >= 10 ? "critical" : errorRate >= 3 ? "warning" : "healthy";

        var summary = new
        {
            periodHours,
            fromDate,
            toDate = DateTime.UtcNow,
            totalCount,
            errorCount,
            warningCount,
            infoCount,
            errorRate,
            httpCount,
            successCount,
            failedHttpCount,
            successRate,
            avgDurationMs = durationStats?.Avg,
            maxDurationMs = durationStats?.Max,
            minDurationMs = durationStats?.Min,
            healthStatus,
            auditCount,
            levelBreakdown,
            statusCodeBreakdown,
            methodBreakdown,
            topLoggers,
            topUsers,
            topErrorUrls,
            topErrorMessages,
            timeSeries,
            recentCritical
        };

        return this.AppOk(OperationResult.Succeeded(payload: summary));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] string logLevel = "")
    {
        currentPage = currentPage >= 0 ? currentPage : 0;
        pageSize = pageSize > 0 ? pageSize : 10;
        var startIndex = pageSize * currentPage;

        var query = _context.Logs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(logLevel))
        {
            query = query.Where(l => l.Level.ToLower() == logLevel.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(l =>
                (l.Level != null && l.Level.Contains(f)) ||
                (l.Message != null && l.Message.Contains(f)) ||
                (l.StackTrace != null && l.StackTrace.Contains(f)) ||
                (l.Exception != null && l.Exception.Contains(f)) ||
                (l.Logger != null && l.Logger.Contains(f)) ||
                (l.Url != null && l.Url.Contains(f)) ||
                (l.IP != null && l.IP.Contains(f)) ||
                (l.User != null && l.User.Contains(f)) ||
                (l.Method != null && l.Method.Contains(f)) ||
                (l.UserAgent != null && l.UserAgent.Contains(f))
            );
        }

        if (string.IsNullOrEmpty(activeSortColumn))
        {
            activeSortColumn = "CreatedOn";
            Sortdirection = "desc";
        }

        query = HR.SharedKernel.Extensions.LinqExtensions.OrderBy(query, activeSortColumn, Sortdirection == "asc" ? false : true);

        var rowCount = await query.CountAsync();
        var items = await query.Skip(startIndex).Take(pageSize).ToListAsync();

        try
        {
            var returnedIds = string.Join(",", items.Select(i => i.Id));
            var clientIp = UserResolverService.GetIP();
            var currentUserName = UserResolverService.GetUser();
            var currentFullName = UserResolverService.fullname();
            _logger.LogInformation(
                "مشاهده تاریخچه لاگ‌ها - کاربر:{UserId} - نام کاربری:{UserName} - نام کامل:{FullName} - IP:{IP} - صفحه:{CurrentPage} - اندازه:{PageSize} - فیلتر:{Filter} - مرتب‌سازی:{SortColumn} {SortDirection} - شناسه‌های مشاهده‌شده:[{Ids}]",
                currentUserId,
                currentUserName,
                currentFullName,
                clientIp,
                currentPage,
                pageSize,
                filter,
                activeSortColumn,
                string.IsNullOrWhiteSpace(Sortdirection) ? "desc" : Sortdirection,
                returnedIds);
        }
        catch { }

        return this.AppOk(OperationResult.Succeeded(payload: items, rowCount: rowCount));
    }

    /// <summary>
    /// دریافت لاگ‌های حسابرسی (Audit Logs) برای یک موجودیت خاص
    /// </summary>
    /// <param name="tableName">نام جدول موجودیت</param>
    /// <param name="primaryKey">کلید اصلی موجودیت (JSON)</param>
    /// <param name="currentPage">صفحه جاری</param>
    /// <param name="pageSize">اندازه صفحه</param>
    /// <returns>فهرست لاگ‌های حسابرسی</returns>
    [HttpGet, Route("GetAuditLogs/{tableName}/{primaryKey}/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetAuditLogs(string tableName, string primaryKey, int currentPage = 0, int pageSize = 10)
    {
        currentPage = currentPage >= 0 ? currentPage : 0;
        pageSize = pageSize > 0 ? pageSize : 10;
        var startIndex = pageSize * currentPage;

        try
        {
            // جستجو در لاگ‌های حسابرسی بر اساس نام جدول و کلید اصلی
            var query = _context.AuditLogs.AsNoTracking()
                .Where(a => a.TableName.Contains(tableName) && a.PrimaryKey == primaryKey)
                .OrderByDescending(a => a.DateTime);

            var rowCount = await query.CountAsync();
            var items = await query.Skip(startIndex).Take(pageSize).ToListAsync();

            // پردازش داده‌ها برای نمایش بهتر
            var processedItems = items.Select(audit => {
                var oldValues = !string.IsNullOrEmpty(audit.OldValues) ? JsonConvert.DeserializeObject(audit.OldValues) : null;
                var newValues = !string.IsNullOrEmpty(audit.NewValues) ? JsonConvert.DeserializeObject(audit.NewValues) : null;
                
                return new
                {
                    audit.Id,
                    audit.UserId,
                    audit.Type,
                    audit.TableName,
                    audit.DateTime,
                    audit.IP,
                    OldValues = SanitizeSensitiveData(oldValues, audit.TableName),
                    NewValues = SanitizeSensitiveData(newValues, audit.TableName),
                    AffectedColumns = !string.IsNullOrEmpty(audit.AffectedColumns) ? JsonConvert.DeserializeObject<List<string>>(audit.AffectedColumns) : new List<string>(),
                    PrimaryKey = !string.IsNullOrEmpty(audit.PrimaryKey) ? JsonConvert.DeserializeObject(audit.PrimaryKey) : null
                };
            }).ToList();

            try
            {
                var clientIp = UserResolverService.GetIP();
                var currentUserName = UserResolverService.GetUser();
                var currentFullName = UserResolverService.fullname();
                _logger.LogInformation(
                    "مشاهده لاگ‌های حسابرسی - کاربر:{UserId} - نام کاربری:{UserName} - نام کامل:{FullName} - IP:{IP} - جدول:{TableName} - کلید:{PrimaryKey} - صفحه:{CurrentPage} - اندازه:{PageSize}",
                    currentUserId,
                    currentUserName,
                    currentFullName,
                    clientIp,
                    tableName,
                    primaryKey,
                    currentPage,
                    pageSize);
            }
            catch { }

            return this.AppOk(OperationResult.Succeeded(payload: processedItems, rowCount: rowCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت لاگ‌های حسابرسی برای جدول {TableName} و کلید {PrimaryKey}", tableName, primaryKey);
            return this.AppBadRequest(OperationResult.Failed("خطا در دریافت لاگ‌های حسابرسی"));
        }
    }

    /// <summary>
    /// دریافت جزئیات تغییرات فیلدها برای یک لاگ حسابرسی خاص
    /// </summary>
    /// <param name="auditId">شناسه لاگ حسابرسی</param>
    /// <returns>جزئیات تغییرات فیلدها</returns>
    [HttpGet, Route("GetAuditDetails/{auditId}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetAuditDetails(int auditId)
    {
        try
        {
            var audit = await _context.AuditLogs.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == auditId);

            if (audit == null)
            {
                return this.AppNotFound();
            }

            // پردازش تغییرات فیلدها
            var fieldChanges = new List<object>();

            if (!string.IsNullOrEmpty(audit.OldValues) || !string.IsNullOrEmpty(audit.NewValues))
            {
                var oldValues = !string.IsNullOrEmpty(audit.OldValues) 
                    ? JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.OldValues) 
                    : new Dictionary<string, object>();

                var newValues = !string.IsNullOrEmpty(audit.NewValues) 
                    ? JsonConvert.DeserializeObject<Dictionary<string, object>>(audit.NewValues) 
                    : new Dictionary<string, object>();

                // پاک‌سازی فیلدهای حساس
                var sanitizedOldValues = (Dictionary<string, object>)SanitizeSensitiveData(oldValues, audit.TableName) ?? new Dictionary<string, object>();
                var sanitizedNewValues = (Dictionary<string, object>)SanitizeSensitiveData(newValues, audit.TableName) ?? new Dictionary<string, object>();

                var affectedColumns = !string.IsNullOrEmpty(audit.AffectedColumns) 
                    ? JsonConvert.DeserializeObject<List<string>>(audit.AffectedColumns) 
                    : new List<string>();

                // ترکیب تمام فیلدهای تغییر یافته
                var allFields = sanitizedOldValues.Keys.Union(sanitizedNewValues.Keys).Union(affectedColumns).Distinct();

                foreach (var field in allFields)
                {
                    var oldValue = sanitizedOldValues.ContainsKey(field) ? sanitizedOldValues[field] : null;
                    var newValue = sanitizedNewValues.ContainsKey(field) ? sanitizedNewValues[field] : null;

                    // بررسی تغییر واقعی
                    bool hasChanged = !Equals(oldValue, newValue);

                    fieldChanges.Add(new
                    {
                        FieldName = field,
                        OldValue = oldValue?.ToString(),
                        NewValue = newValue?.ToString(),
                        HasChanged = hasChanged,
                        ChangeType = audit.Type
                    });
                }
            }

            var result = new
            {
                audit.Id,
                audit.UserId,
                audit.Type,
                audit.TableName,
                audit.DateTime,
                audit.IP,
                PrimaryKey = !string.IsNullOrEmpty(audit.PrimaryKey) ? JsonConvert.DeserializeObject(audit.PrimaryKey) : null,
                FieldChanges = fieldChanges.OrderBy(f => ((dynamic)f).FieldName).ToList()
            };

            try
            {
                var clientIp = UserResolverService.GetIP();
                var currentUserName = UserResolverService.GetUser();
                var currentFullName = UserResolverService.fullname();
                _logger.LogInformation(
                    "مشاهده جزئیات لاگ حسابرسی - کاربر:{UserId} - نام کاربری:{UserName} - نام کامل:{FullName} - IP:{IP} - شناسه لاگ:{AuditId}",
                    currentUserId,
                    currentUserName,
                    currentFullName,
                    clientIp,
                    auditId);
            }
            catch { }

            return this.AppOk(OperationResult.Succeeded(payload: result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت جزئیات لاگ حسابرسی {AuditId}", auditId);
            return this.AppBadRequest(OperationResult.Failed("خطا در دریافت جزئیات لاگ حسابرسی"));
        }
    }

    /// <summary>
    /// دریافت تمام لاگ‌های حسابرسی با قابلیت فیلتر و مرتب‌سازی
    /// </summary>
    /// <param name="currentPage">صفحه جاری</param>
    /// <param name="pageSize">اندازه صفحه</param>
    /// <param name="filter">فیلتر جستجو</param>
    /// <param name="tableName">فیلتر بر اساس نام جدول</param>
    /// <param name="auditType">فیلتر بر اساس نوع عملیات (Create, Update, Delete)</param>
    /// <param name="userId">فیلتر بر اساس کاربر</param>
    /// <param name="fromDate">از تاریخ</param>
    /// <param name="toDate">تا تاریخ</param>
    /// <param name="activeSortColumn">ستون مرتب‌سازی</param>
    /// <param name="Sortdirection">جهت مرتب‌سازی</param>
    /// <returns>فهرست لاگ‌های حسابرسی</returns>
    [HttpGet, Route("GetAllAuditLogs/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetAllAuditLogs(
        int currentPage = 0, 
        int pageSize = 10, 
        [FromQuery] string filter = "", 
        [FromQuery] string tableName = "", 
        [FromQuery] string auditType = "", 
        [FromQuery] string userId = "", 
        [FromQuery] DateTime? fromDate = null, 
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string activeSortColumn = "", 
        [FromQuery] string Sortdirection = "")
    {
        currentPage = currentPage >= 0 ? currentPage : 0;
        pageSize = pageSize > 0 ? pageSize : 10;
        var startIndex = pageSize * currentPage;

        try
        {
            var query = _context.AuditLogs.AsNoTracking().AsQueryable();

            // اعمال فیلترها
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var f = filter.Trim();
                query = query.Where(a =>
                    (a.UserId != null && a.UserId.Contains(f)) ||
                    (a.TableName != null && a.TableName.Contains(f)) ||
                    (a.Type != null && a.Type.Contains(f)) ||
                    (a.IP != null && a.IP.Contains(f))
                );
            }

            if (!string.IsNullOrWhiteSpace(tableName))
            {
                query = query.Where(a => a.TableName.Contains(tableName));
            }

            if (!string.IsNullOrWhiteSpace(auditType))
            {
                query = query.Where(a => a.Type == auditType);
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(a => a.UserId == userId);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.DateTime >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.DateTime <= toDate.Value);
            }

            // مرتب‌سازی
            if (string.IsNullOrEmpty(activeSortColumn))
            {
                activeSortColumn = "DateTime";
                Sortdirection = "desc";
            }

            query = HR.SharedKernel.Extensions.LinqExtensions.OrderBy(query, activeSortColumn, Sortdirection == "asc" ? false : true);

            var rowCount = await query.CountAsync();
            var items = await query.Skip(startIndex).Take(pageSize).ToListAsync();

            // پردازش داده‌ها برای نمایش بهتر
            var processedItems = items.Select(audit => new
            {
                audit.Id,
                audit.UserId,
                audit.Type,
                audit.TableName,
                audit.DateTime,
                audit.IP,
                HasOldValues = !string.IsNullOrEmpty(audit.OldValues),
                HasNewValues = !string.IsNullOrEmpty(audit.NewValues),
                AffectedColumnsCount = !string.IsNullOrEmpty(audit.AffectedColumns) 
                    ? JsonConvert.DeserializeObject<List<string>>(audit.AffectedColumns).Count 
                    : 0,
                PrimaryKey = !string.IsNullOrEmpty(audit.PrimaryKey) ? JsonConvert.DeserializeObject(audit.PrimaryKey) : null
            }).ToList();

            try
            {
                var returnedIds = string.Join(",", items.Select(i => i.Id));
                var clientIp = UserResolverService.GetIP();
                var currentUserName = UserResolverService.GetUser();
                var currentFullName = UserResolverService.fullname();
                _logger.LogInformation(
                    "مشاهده تمام لاگ‌های حسابرسی - کاربر:{UserId} - نام کاربری:{UserName} - نام کامل:{FullName} - IP:{IP} - صفحه:{CurrentPage} - اندازه:{PageSize} - فیلتر:{Filter} - شناسه‌های مشاهده‌شده:[{Ids}]",
                    currentUserId,
                    currentUserName,
                    currentFullName,
                    clientIp,
                    currentPage,
                    pageSize,
                    filter,
                    returnedIds);
            }
            catch { }

            return this.AppOk(OperationResult.Succeeded(payload: processedItems, rowCount: rowCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت لاگ‌های حسابرسی");
            return this.AppBadRequest(OperationResult.Failed("خطا در دریافت لاگ‌های حسابرسی"));
        }
    }
}
