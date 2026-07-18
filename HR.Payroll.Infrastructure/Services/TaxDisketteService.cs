using AutoMapper;
using Hr.Employee.infrastructure.Services;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Services;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Services;
using HR.Organisation.Infrastructure.Services;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static HR.SharedKernel.Share.Enums;


namespace HR.Payroll.Infrastructure.Services;

public class TaxDisketteService(IMapper mapper, FicheService FicheService, InsuranceService InsuranceService, OrganisationCostCenterService OrganisationCostCenterService, OrganisationChartService OrganisationChartService, BaseTableValueService BaseTableValueService, OrganisationEmployeeTypeService OrganisationEmployeeTypeService, ContactInfoService ContactInfoService, EducationGradeService EducationGradeService, PlacesService PlacesService, OrderService OrderService, OrganisationEmployeeTypeFicheItemService OrganisationEmployeeTypeFicheItemService, TaxCoefficientItemService TaxCoefficientItemService, EmployeeService EmployeeService, PaymentPeriodService PaymentPeriodService, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<TaxDisketteService> logger) : BaseService<TaxDiskette, PayrollContext, TaxDisketteDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private PaymentPeriodService _paymentPeriodService = PaymentPeriodService;
    private EmployeeService _employeeService = EmployeeService;
    private FicheService _ficheService = FicheService;
    private OrderService _orderService = OrderService;
    private OrganisationChartService _organisationChartService = OrganisationChartService;
    private EducationGradeService _educationGradeService = EducationGradeService;
    private PlacesService _placesService = PlacesService;
    private BaseTableValueService _baseTableValueService = BaseTableValueService;
    private OrganisationEmployeeTypeService _organisationEmployeeTypeService = OrganisationEmployeeTypeService;
    private ContactInfoService _contactInfoService = ContactInfoService;
    private TaxCoefficientItemService _taxCoefficientItemService = TaxCoefficientItemService;
    private OrganisationEmployeeTypeFicheItemService _organisationEmployeeTypeFicheItemService = OrganisationEmployeeTypeFicheItemService;
    private OrganisationCostCenterService _organisationCostCenterService = OrganisationCostCenterService;
    private InsuranceService _insuranceService = InsuranceService;
    private readonly ILogger<TaxDisketteService> _logger = logger;
    private const string customDelimiter = "MySpecialChar";
    // Number of delimiters per WP line (fields - 1). Ensures trailing empty fields are present
    private const int WPExpectedDelimiterCount = 22; // 23 fields -> 22 delimiters
    // Number of delimiters per WH line (fields - 1). Ensures trailing empty fields are present
    private const int WHExpectedDelimiterCount = 38; // 39 fields -> 38 delimiters
    private readonly UserResolverService _userResolverService = userService;

    public new OperationResult Get(long id)
    {
        try
        {
            var Properties = typeof(TaxDiskette).GetProperties();
            var all = All(false);
            foreach (var Propertiy in Properties)
            {
                if (Propertiy.PropertyType.BaseType == typeof(TaxDiskette).BaseType)
                {
                    all = all.Include(Propertiy.Name);
                }
            }
            var row = all.SingleOrDefault(i => i.Id == id);
            var record = _mapper.Map<TaxDisketteDTO>(row);

            var TaxDisketteWH = _unitOfWork.Context.TaxDisketteWHs
                .Include(i => i.PaymentType)

                .Where(w => w.TaxDisketteId == id);

            if (TaxDisketteWH == null)
            {

            }
            else
            {
                if (TaxDisketteWH.Any())
                {
                    if (TaxDisketteWH.Count() == 1)
                    {
                        var relatedWK = TaxDisketteWH.Single();
                    }
                }
            }

            if (record == null)
            {
                return OperationResult.NotFound();
            }
            else
            {
                return OperationResult.Succeeded(payload: record);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت دیسکت مالیات با شناسه {Id}", id);
            return OperationResult.Failed($"خطا در دریافت اطلاعات: {ex.Message}");
        }
    }

    public string GetNumericalDate(DateTime date)
    {
        System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();
        date = DateTime.Parse(date.ToShortDateString());
        int year = p.GetYear(date);
        int month = p.GetMonth(date);
        int day = p.GetDayOfMonth(date);
        string str = string.Format("{0}{1}{2}", year, month.ToString().PadLeft(2, '0'), day.ToString().PadLeft(2, '0'));
        return str;
    }

    private void SaveChangesWithDetailedLogging(string operationDescription)
    {
        const int maxRetryCount = 3;
        var tempAutoDetect = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
        try
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;
            for (int attempt = 1; attempt <= maxRetryCount; attempt++)
            {
                try
                {
                    _unitOfWork.Context.SaveChanges();
                    _logger.LogInformation("SaveChanges succeeded: {Operation}", operationDescription);
                    return;
                }
                catch (DbUpdateConcurrencyException concurrencyEx)
                {
                    // Resolve concurrency by refreshing/de-taching conflicted entries, then retry
                    try
                    {
                        foreach (var entry in concurrencyEx.Entries)
                        {
                            // If entity was deleted by another process, just detach to skip re-saving it
                            if (entry.State == EntityState.Deleted)
                            {
                                entry.State = EntityState.Detached;
                                continue;
                            }

                            var databaseValues = entry.GetDatabaseValues();
                            if (databaseValues == null)
                            {
                                // Entity no longer exists in DB; detach to skip
                                entry.State = EntityState.Detached;
                            }
                            else
                            {
                                // Align original values with database to unblock the next retry
                                entry.OriginalValues.SetValues(databaseValues);
                            }
                        }
                    }
                    catch (Exception resolveEx)
                    {
                        _logger.LogError(resolveEx, "Failed to resolve concurrency during {Operation}", operationDescription);
                        throw;
                    }

                    if (attempt == maxRetryCount)
                    {
                        _logger.LogError(concurrencyEx, "SaveChanges concurrency failure after retries during {Operation}", operationDescription);
                        throw;
                    }

                    // Brief backoff before retry
                    System.Threading.Thread.Sleep(50 * attempt);
                    continue;
                }
                catch (DbUpdateException dbEx)
                {
                    try
                    {
                        var entries = dbEx.Entries != null ? string.Join(",", dbEx.Entries.Select(e => e.Entity.GetType().Name)) : "";
                        _logger.LogError(dbEx, "SaveChanges failed during {Operation}. Affected entries: {Entries}", operationDescription, entries);
                    }
                    catch
                    {
                        _logger.LogError(dbEx, "SaveChanges failed during {Operation}", operationDescription);
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error on SaveChanges during {Operation}", operationDescription);
                    throw;
                }
            }
        }
        finally
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = tempAutoDetect;
        }
    }

    private const int TaxStuckRequestTimeoutMinutes = 30;
    private const string TaxWorkerIpAddress = "Job";

    private static readonly long[] ValidTaxBatchRequestStates =
    [
        (long)Enums.BatchPayRollRequestState.Initial,
        (long)Enums.BatchPayRollRequestState.TryAgain,
    ];

    private static readonly long[] ValidTaxFicheStatusList =
    [
        (long)Enums.FicheStatus.Initial,
        (long)Enums.FicheStatus.Payed,
    ];

    private void CleanupStuckTaxDisketteRequests()
    {
        try
        {
            WithTaxTrackingEnabled(() =>
            {
                var stuckRequests = _unitOfWork.Context.BatchPayRollRequests
                    .Where(i => i.RequestTypeId == (long)Enums.BatchPayRollRequestType.TaxDisketteCalculation
                                && i.RequestStateId == (long)Enums.BatchPayRollRequestState.Running
                                && i.LastPoolingTime.HasValue
                                && i.LastPoolingTime.Value < DateTime.Now.AddMinutes(-TaxStuckRequestTimeoutMinutes))
                    .ToList();

                if (!stuckRequests.Any())
                {
                    return;
                }

                _logger.LogWarning("تعداد {Count} درخواست دیسکت مالیات گیرکرده (بیش از {Timeout} دقیقه) یافت شد",
                    stuckRequests.Count, TaxStuckRequestTimeoutMinutes);

                foreach (var stuckRequest in stuckRequests)
                {
                    try
                    {
                        stuckRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.TryAgain;
                        stuckRequest.ExeptionMessage =
                            $"پردازش قبلی بیش از {TaxStuckRequestTimeoutMinutes} دقیقه پاسخ نداد. سیستم درخواست را برای «تلاش مجدد» آماده کرد. لطفاً چند دقیقه صبر کنید یا از فرم نظارت، «تلاش مجدد» را بزنید.";
                        stuckRequest.LastModifiedDate = DateTime.Now;
                        _unitOfWork.Context.Update(stuckRequest);
                        SaveChangesWithDetailedLogging("Cleanup stuck tax diskette request");
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "خطا در پاکسازی درخواست گیرکرده {RequestId}", stuckRequest.Id);
                        _unitOfWork.Context.ChangeTracker.Clear();
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پاکسازی درخواست‌های گیرکرده دیسکت مالیات");
        }
        finally
        {
            _unitOfWork.Context.ChangeTracker.Clear();
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }

    private void ApplyTaxDisketteOrganisationContext(long organisationChartId)
    {
        _currentUserDefaultOrganId = organisationChartId;
        _taxCoefficientItemService._currentUserDefaultOrganId = organisationChartId;
        _organisationEmployeeTypeFicheItemService._currentUserDefaultOrganId = organisationChartId;
        _employeeService._currentUserDefaultOrganId = organisationChartId;
        _paymentPeriodService._currentUserDefaultOrganId = organisationChartId;
        _orderService._currentUserDefaultOrganId = organisationChartId;
    }

    private void FailTaxDisketteRequest(
        BatchPayRollRequest batchRequest,
        TaxDiskette? taxDiskette,
        string userMessage)
    {
        _logger.LogWarning("درخواست دیسکت مالیات {RequestId} با پیام کاربر متوقف شد: {Message}",
            batchRequest.Id, userMessage);

        try
        {
            SetTaxBatchRequestStateInDatabase(
                batchRequest,
                Enums.BatchPayRollRequestState.EndLoop,
                userMessage,
                setFinishDate: true);

            if (taxDiskette != null
                && taxDiskette.TaxDisketteStatusId != (long)Enums.TaxDisketteStatus.Payed
                && taxDiskette.TaxDisketteStatusId != (long)Enums.TaxDisketteStatus.Deleted)
            {
                SetTaxDisketteStatusInDatabase(
                    taxDiskette.Id,
                    (long)Enums.TaxDisketteStatus.CalculationFinished);
                taxDiskette.TaxDisketteStatusId = (long)Enums.TaxDisketteStatus.CalculationFinished;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FailTaxDisketteRequest برای درخواست {RequestId} شکست خورد", batchRequest.Id);
            EnsureTaxBatchRequestNotLeftRunning(batchRequest.Id, taxDiskette?.Id, userMessage);
        }
    }

    private void CompleteTaxDisketteRequest(
        BatchPayRollRequest batchRequest,
        TaxDiskette taxDiskette,
        string? summaryMessage)
    {
        SetTaxBatchRequestStateInDatabase(
            batchRequest,
            Enums.BatchPayRollRequestState.EndLoop,
            summaryMessage,
            setFinishDate: true);

        SetTaxDisketteStatusInDatabase(
            taxDiskette.Id,
            (long)Enums.TaxDisketteStatus.CalculationFinished);

        taxDiskette.TaxDisketteStatusId = (long)Enums.TaxDisketteStatus.CalculationFinished;
        taxDiskette.LastModifiedDate = DateTime.Now;
    }

    private long GetTaxBatchRequestStateFromDatabase(long requestId)
    {
        return _unitOfWork.Context.BatchPayRollRequests
            .AsNoTracking()
            .Where(r => r.Id == requestId)
            .Select(r => r.RequestStateId)
            .FirstOrDefault();
    }

    private void SetTaxBatchRequestStateInDatabase(
        BatchPayRollRequest request,
        Enums.BatchPayRollRequestState newState,
        string? exceptionMessage,
        bool setFinishDate)
    {
        try
        {
            _unitOfWork.Rollback();
        }
        catch
        {
            // ignore — ممکن است تراکنشی باز نباشد
        }

        var now = DateTime.Now;
        var stateId = (long)newState;

        int affected;
        if (setFinishDate)
        {
            affected = _unitOfWork.Context.BatchPayRollRequests
                .Where(r => r.Id == request.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.RequestStateId, stateId)
                    .SetProperty(r => r.LastPoolingTime, now)
                    .SetProperty(r => r.LastModifiedDate, now)
                    .SetProperty(r => r.FinishDateTime, now)
                    .SetProperty(r => r.ExeptionMessage, exceptionMessage));
        }
        else
        {
            affected = _unitOfWork.Context.BatchPayRollRequests
                .Where(r => r.Id == request.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.RequestStateId, stateId)
                    .SetProperty(r => r.LastPoolingTime, now)
                    .SetProperty(r => r.LastModifiedDate, now)
                    .SetProperty(r => r.ExeptionMessage, exceptionMessage));
        }

        if (affected == 0)
        {
            throw new InvalidOperationException(
                $"به‌روزرسانی وضعیت درخواست مالیات {request.Id} در پایگاه داده انجام نشد.");
        }

        request.RequestStateId = stateId;
        request.LastPoolingTime = now;
        request.LastModifiedDate = now;
        request.ExeptionMessage = exceptionMessage;
        if (setFinishDate)
        {
            request.FinishDateTime = now;
        }

        var verifiedState = GetTaxBatchRequestStateFromDatabase(request.Id);
        if (verifiedState != stateId)
        {
            _logger.LogCritical(
                "وضعیت درخواست مالیات {RequestId} پس از ExecuteUpdate هنوز {Actual} است (انتظار: {Expected})",
                request.Id, verifiedState, stateId);
            throw new InvalidOperationException(
                $"وضعیت درخواست مالیات {request.Id} در پایگاه داده درست ذخیره نشد.");
        }

        _logger.LogInformation(
            "وضعیت درخواست مالیات {RequestId} در DB به {State} تغییر کرد",
            request.Id, newState);
    }

    private void SetTaxDisketteStatusInDatabase(long taxDisketteId, long statusId)
    {
        var now = DateTime.Now;
        _unitOfWork.Context.TaxDiskettes
            .Where(d => d.Id == taxDisketteId)
            .ExecuteUpdate(s => s
                .SetProperty(d => d.TaxDisketteStatusId, statusId)
                .SetProperty(d => d.LastModifiedDate, now));
    }

    private void EnsureTaxBatchRequestNotLeftRunning(long requestId, long? taxDisketteId, string message)
    {
        try
        {
            var currentState = GetTaxBatchRequestStateFromDatabase(requestId);
            if (currentState != (long)Enums.BatchPayRollRequestState.Running)
            {
                return;
            }

            _logger.LogError(
                "درخواست دیسکت مالیات {RequestId} هنوز Running است — اجبار به EndLoop",
                requestId);

            var now = DateTime.Now;
            _unitOfWork.Context.BatchPayRollRequests
                .Where(r => r.Id == requestId
                            && r.RequestStateId == (long)Enums.BatchPayRollRequestState.Running)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.RequestStateId, (long)Enums.BatchPayRollRequestState.EndLoop)
                    .SetProperty(r => r.FinishDateTime, now)
                    .SetProperty(r => r.LastPoolingTime, now)
                    .SetProperty(r => r.LastModifiedDate, now)
                    .SetProperty(r => r.ExeptionMessage, message));

            if (taxDisketteId.HasValue && taxDisketteId.Value > 0)
            {
                SetTaxDisketteStatusInDatabase(
                    taxDisketteId.Value,
                    (long)Enums.TaxDisketteStatus.CalculationFinished);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "نتوانستیم وضعیت گیرکرده درخواست مالیات {RequestId} را از Running خارج کنیم",
                requestId);
        }
    }

    private void IncrementTaxBatchRequestSuccessInDatabase(BatchPayRollRequest batchRequest)
    {
        var now = DateTime.Now;
        _unitOfWork.Context.BatchPayRollRequests
            .Where(r => r.Id == batchRequest.Id)
            .ExecuteUpdate(s => s
                .SetProperty(r => r.SuccessCount, r => r.SuccessCount + 1)
                .SetProperty(r => r.LastPoolingTime, now)
                .SetProperty(r => r.LastModifiedDate, now));

        batchRequest.SuccessCount += 1;
        batchRequest.LastPoolingTime = now;
        batchRequest.LastModifiedDate = now;
    }

    private void SetTaxBatchRequestCountersInDatabase(long requestId, int employeeCount, int successCount)
    {
        var now = DateTime.Now;
        _unitOfWork.Context.BatchPayRollRequests
            .Where(r => r.Id == requestId)
            .ExecuteUpdate(s => s
                .SetProperty(r => r.EmployeeCount, employeeCount)
                .SetProperty(r => r.SuccessCount, successCount)
                .SetProperty(r => r.LastPoolingTime, now)
                .SetProperty(r => r.LastModifiedDate, now));
    }

    private bool IsTaxRequestCancelledByUser(long requestId)
    {
        return GetTaxBatchRequestStateFromDatabase(requestId) == (long)Enums.BatchPayRollRequestState.CancelByUser;
    }

    private void WithTaxTrackingEnabled(Action action)
    {
        var previousAutoDetect = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
        var previousTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;

        try
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            action();
        }
        finally
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = previousTracking;
        }
    }

    private static string BuildTaxUserFriendlyErrorMessage(Exception ex, string context)
    {
        if (ex is InvalidOperationException invalidOpEx && !string.IsNullOrWhiteSpace(invalidOpEx.Message))
        {
            return invalidOpEx.Message;
        }

        var message = ex.InnerException?.Message ?? ex.Message ?? string.Empty;

        if (message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
            || message.Contains("timed out", StringComparison.OrdinalIgnoreCase))
        {
            return "ارتباط با پایگاه داده کند یا قطع شد. لطفاً چند دقیقه بعد دوباره «تلاش مجدد» را بزنید.";
        }

        if (message.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
        {
            return "سیستم موقتاً مشغول است. لطفاً چند دقیقه بعد دوباره تلاش کنید.";
        }

        if (message.Contains("foreign key", StringComparison.OrdinalIgnoreCase))
        {
            return "برخی اطلاعات پایه (دوره، بانک، مرکز هزینه یا حکم کارگزینی) ناقص یا حذف شده است. تنظیمات پایه را بررسی کنید.";
        }

        if (message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
            || message.Contains("unique", StringComparison.OrdinalIgnoreCase))
        {
            return "رکورد تکراری در دیسکت مالیات شناسایی شد. در صورت تکرار، با پشتیبانی تماس بگیرید.";
        }

        return $"{context} با خطا مواجه شد. لطفاً جزئیات را در لیست ریز عملیات ببینید یا با پشتیبانی تماس بگیرید.";
    }

    public new OperationResult DeleteRecord(long Id)
    {
        try
        {
            var existingDiskette = _unitOfWork.Context.TaxDiskettes.Find(Id);
            if (existingDiskette == null)
            {
                return OperationResult.NotFound("دیسکت مالیات یافت نشد");
            }
            
            if (existingDiskette.TaxDisketteStatusId == (long)Enums.TaxDisketteStatus.Payed || existingDiskette.TaxDisketteStatusId == (long)Enums.TaxDisketteStatus.Deleted)
            {
                return OperationResult.Failed("وضعیت دیسکت جهت حذف معتبر نمی باشد");
            }
            
            var relatedPeriod = _unitOfWork.Context.PaymentPeriods.Find(existingDiskette.PaymentPeriodId);
            if (relatedPeriod == null)
            {
                return OperationResult.Failed("دوره پرداخت مرتبط یافت نشد");
            }
            
            if (relatedPeriod.IsClosed == true)
            {
                return OperationResult.Failed("دوره مورد نظر بسته شده است و امکان حذف دیسکت وجود ندارد");
            }
            
            _unitOfWork.Context.Entry(existingDiskette).Property("TaxDisketteStatusId").CurrentValue = (long)Enums.TaxDisketteStatus.Deleted;
            _unitOfWork.Context.TaxDiskettes.Update(existingDiskette);
            _unitOfWork.Context.SaveChanges();
            return OperationResult.Succeeded();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "خطا در حذف دیسکت مالیات با شناسه {Id}", Id);
            return OperationResult.Failed("خطا در ذخیره تغییرات در پایگاه داده. لطفاً دوباره تلاش کنید.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف دیسکت مالیات با شناسه {Id}", Id);
            return OperationResult.Failed($"خطا در حذف دیسکت مالیات: {ex.Message}");
        }
    }
    public OperationResult GetCurrentTaxDisketteCostCenters(long id)
    {
        try
        {
            var TaxDiskette = GetIdAsync(id).Result;
            if (TaxDiskette == null)
            {
                return OperationResult.NotFound("دیسکت مالیات یافت نشد");
            }
            
            if (TaxDiskette.CalculateAllFichesInCurrentPeriod == true)
            {
                return OperationResult.NotFound("این دیسکت برای تمام فیش‌های دوره محاسبه شده است");
            }
            
            var costCenterList = _unitOfWork.Context.TaxDisketteCostCenters.Include(i => i.CostCenter).Where(i => i.TaxDisketteId == id);

            if (costCenterList == null || !costCenterList.Any())
            {
                return OperationResult.NotFound("مرکز هزینه‌ای برای این دیسکت یافت نشد");
            }
            
            var result = _mapper.Map<List<TaxDisketteCostCenterDTO>>(costCenterList.ToList());
            return OperationResult.Succeeded(payload: result);
        }
        catch (AggregateException ex)
        {
            _logger.LogError(ex, "خطا در دریافت مراکز هزینه دیسکت مالیات با شناسه {Id}", id);
            var innerEx = ex.InnerException ?? ex;
            return OperationResult.Failed($"خطا در دریافت اطلاعات: {innerEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت مراکز هزینه دیسکت مالیات با شناسه {Id}", id);
            return OperationResult.Failed($"خطا در دریافت مراکز هزینه: {ex.Message}");
        }
    }

    public OperationResult downloadTaxDisk(long BatchPayRollRequestId, long FileTypeId)
    {
        try
        {
            var TaxDisk = _unitOfWork.Context.TaxDiskettes.Where(i => i.BatchPayRollRequestId == BatchPayRollRequestId).SingleOrDefault();
            if (TaxDisk == null)
            {
                return OperationResult.NotFound("دیسکت مالیات برای این درخواست یافت نشد");
            }
            
            var diskList = _unitOfWork.Context.TaxDisketteFiles.Where(i => i.TaxDisketteId == TaxDisk.Id && i.FileTypeId == FileTypeId);
            if (diskList == null || !diskList.Any())
            {
                return OperationResult.NotFound("فایل دیسکت مالیات یافت نشد");
            }
            
            return OperationResult.Succeeded(payload: diskList.OrderBy(i => i.Id).Last());
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "خطا در دریافت دیسکت مالیات - BatchPayRollRequestId: {BatchPayRollRequestId}, FileTypeId: {FileTypeId}", BatchPayRollRequestId, FileTypeId);
            return OperationResult.NotFound("دیسکت مالیات یا فایل مورد نظر یافت نشد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دانلود دیسکت مالیات - BatchPayRollRequestId: {BatchPayRollRequestId}, FileTypeId: {FileTypeId}", BatchPayRollRequestId, FileTypeId);
            return OperationResult.Failed($"خطا در دانلود فایل دیسکت: {ex.Message}");
        }
    }
    /// <summary>
    /// محاسبه دیسکت مالیات گروهی و ساخت فایل های دیسکت
    /// </summary>
    public void CalculateTaxDisketteBatch()
    {
        _logger.LogInformation("شروع CalculateTaxDisketteBatch");

        _unitOfWork.Context.ChangeTracker.Clear();
        var originalAutoDetectChanges = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
        var originalQueryTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;

        _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
        _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        try
        {
            CleanupStuckTaxDisketteRequests();

            var readyToCalculateRequests = _unitOfWork.Context.BatchPayRollRequests
                .Where(i => i.RequestTypeId == (long)Enums.BatchPayRollRequestType.TaxDisketteCalculation
                            && ValidTaxBatchRequestStates.Contains(i.RequestStateId))
                .OrderBy(i => i.Id)
                .ToList();

            _logger.LogInformation("تعداد {Count} درخواست دیسکت مالیات برای پردازش یافت شد", readyToCalculateRequests.Count);

            foreach (var batchRequest in readyToCalculateRequests)
            {
                if (IsTaxRequestCancelledByUser(batchRequest.Id))
                {
                    _logger.LogInformation("درخواست {RequestId} توسط کاربر لغو شده — رد شد", batchRequest.Id);
                    continue;
                }

                try
                {
                    ProcessSingleTaxDisketteRequest(batchRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطای غیرمنتظره در پردازش درخواست دیسکت مالیات {RequestId}", batchRequest.Id);
                    try
                    {
                        FailTaxDisketteRequest(
                            batchRequest,
                            null,
                            BuildTaxUserFriendlyErrorMessage(ex, "پردازش درخواست دیسکت مالیات"));
                    }
                    catch (Exception failEx)
                    {
                        _logger.LogCritical(failEx, "FailTaxDisketteRequest نیز شکست خورد برای {RequestId}", batchRequest.Id);
                        EnsureTaxBatchRequestNotLeftRunning(
                            batchRequest.Id,
                            batchRequest.TaxDisketteId,
                            BuildTaxUserFriendlyErrorMessage(ex, "پردازش درخواست دیسکت مالیات"));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای کلی در CalculateTaxDisketteBatch");
        }
        finally
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = originalQueryTracking;
            _logger.LogInformation("پایان CalculateTaxDisketteBatch");
        }
    }

    private void ProcessSingleTaxDisketteRequest(BatchPayRollRequest BatchPayRollRequest)
    {
        _logger.LogInformation("شروع پردازش درخواست دیسکت مالیات: {RequestId}", BatchPayRollRequest.Id);

        var isRetryRequest = BatchPayRollRequest.RequestStateId == (long)Enums.BatchPayRollRequestState.TryAgain;
        TaxDiskette? TaxDiskette = null;
        var enteredRunning = false;

        try
        {
            ApplyTaxDisketteOrganisationContext(BatchPayRollRequest.OrganisationChartId);

            #region بررسی وجود فایل TaxResponse
            var taxDisketteForResponse = _unitOfWork.Context.TaxDiskettes.Find(BatchPayRollRequest.TaxDisketteId);
            if (taxDisketteForResponse != null)
            {
                var hasTaxResponseFile = _unitOfWork.Context.TaxDisketteFiles
                    .Any(f => f.TaxDisketteId == taxDisketteForResponse.Id
                           && f.FileTypeId == (long)Enums.TaxDisketteFileType.TaxResponse);

                if (hasTaxResponseFile)
                {
                    _logger.LogInformation("فایل TaxResponse برای BatchPayRollRequestId {BatchPayRollRequestId} پیدا شد. شروع پردازش...",
                        BatchPayRollRequest.Id);

                    SetTaxBatchRequestStateInDatabase(
                        BatchPayRollRequest,
                        Enums.BatchPayRollRequestState.Running,
                        null,
                        setFinishDate: false);
                    enteredRunning = true;
                    TaxDiskette = taxDisketteForResponse;

                    var result = ProcessTaxResponseFileForBatch(BatchPayRollRequest.Id).GetAwaiter().GetResult();

                    var responseSummary = result.errors.Count > 0
                        ? $"پردازش پاسخ سازمان مالیات با {result.failCount} خطا انجام شد. {result.errors.FirstOrDefault()}"
                        : null;

                    SetTaxBatchRequestCountersInDatabase(
                        BatchPayRollRequest.Id,
                        result.successCount + result.failCount,
                        result.successCount);
                    BatchPayRollRequest.SuccessCount = result.successCount;
                    BatchPayRollRequest.EmployeeCount = result.successCount + result.failCount;

                    CompleteTaxDisketteRequest(BatchPayRollRequest, taxDisketteForResponse, responseSummary);

                    _logger.LogInformation("پردازش فایل TaxResponse برای BatchPayRollRequestId {BatchPayRollRequestId} به پایان رسید. موفق: {SuccessCount}, ناموفق: {FailCount}",
                        BatchPayRollRequest.Id, result.successCount, result.failCount);

                    return;
                }
            }
            #endregion بررسی وجود فایل TaxResponse

            TaxDiskette = _unitOfWork.Context.TaxDiskettes.Find(BatchPayRollRequest.TaxDisketteId);
            if (TaxDiskette == null)
            {
                FailTaxDisketteRequest(
                    BatchPayRollRequest,
                    null,
                    "اطلاعات دیسکت مالیات مربوط به این درخواست در سیستم یافت نشد. لطفاً درخواست را حذف و مجدداً ثبت کنید.");
                return;
            }

            if (TaxDiskette.TaxDisketteStatusId == (long)Enums.TaxDisketteStatus.Payed)
            {
                FailTaxDisketteRequest(
                    BatchPayRollRequest,
                    TaxDiskette,
                    "این دیسکت مالیات قبلاً پرداخت شده است و امکان محاسبه مجدد وجود ندارد.");
                return;
            }

            if (TaxDiskette.TaxDisketteStatusId == (long)Enums.TaxDisketteStatus.Deleted)
            {
                SetTaxBatchRequestStateInDatabase(
                    BatchPayRollRequest,
                    Enums.BatchPayRollRequestState.Deleted,
                    "دیسکت مالیات حذف شده است.",
                    setFinishDate: true);
                return;
            }

            if (TaxDiskette.TaxDisketteStatusId == (long)Enums.TaxDisketteStatus.CalculationFinished
                && !isRetryRequest)
            {
                CompleteTaxDisketteRequest(BatchPayRollRequest, TaxDiskette, null);
                _logger.LogInformation("درخواست {RequestId} قبلاً محاسبه شده — بدون پردازش مجدد بسته شد", BatchPayRollRequest.Id);
                return;
            }

            SetTaxBatchRequestStateInDatabase(
                BatchPayRollRequest,
                Enums.BatchPayRollRequestState.Running,
                null,
                setFinishDate: false);
            SetTaxDisketteStatusInDatabase(TaxDiskette.Id, (long)Enums.TaxDisketteStatus.Running);
            TaxDiskette.TaxDisketteStatusId = (long)Enums.TaxDisketteStatus.Running;
            enteredRunning = true;

            var CurrentPeriod = _paymentPeriodService.GetIdAsync(TaxDiskette.PaymentPeriodId).GetAwaiter().GetResult();
            if (CurrentPeriod == null)
            {
                FailTaxDisketteRequest(
                    BatchPayRollRequest,
                    TaxDiskette,
                    "دوره پرداخت مربوط به دیسکت مالیات یافت نشد. دوره را بررسی کنید.");
                return;
            }

            var fiches = _unitOfWork.Context.Fiches
                .Include(i => i.InterdictOrder)
                .Include(i => i.InterdictOrder!.RecruitOrder)
                .Where(i => i.PaymentPeriodId == TaxDiskette.PaymentPeriodId && ValidTaxFicheStatusList.Contains(i.FicheStatusId))
                .ToList();

            if (TaxDiskette.CalculateAllFichesInCurrentPeriod != true)
            {
                var selectedCostCenters = _unitOfWork.Context.TaxDisketteCostCenters
                    .Where(i => i.TaxDisketteId == TaxDiskette.Id)
                    .Select(i => i.CostCenterId)
                    .Distinct()
                    .ToList();

                if (!selectedCostCenters.Any())
                {
                    FailTaxDisketteRequest(
                        BatchPayRollRequest,
                        TaxDiskette,
                        "هیچ مرکز هزینه‌ای برای این دیسکت مالیات ثبت نشده است. درخواست را با انتخاب مراکز هزینه دوباره ثبت کنید.");
                    return;
                }

                fiches = fiches.Where(i => selectedCostCenters.Contains(i.CostCenterId)).ToList();
            }

            if (!fiches.Any())
            {
                FailTaxDisketteRequest(
                    BatchPayRollRequest,
                    TaxDiskette,
                    TaxDiskette.CalculateAllFichesInCurrentPeriod
                        ? "برای دوره انتخابی هیچ فیش حقوقی مناسب (محاسبه‌شده یا پرداخت‌شده) یافت نشد. ابتدا فیش حقوق را محاسبه کنید."
                        : "برای مراکز هزینه انتخاب‌شده هیچ فیش حقوقی مناسب یافت نشد. مراکز هزینه یا فیش‌های دوره را بررسی کنید.");
                return;
            }

            SetTaxBatchRequestCountersInDatabase(BatchPayRollRequest.Id, fiches.Count, successCount: 0);
            BatchPayRollRequest.EmployeeCount = fiches.Count;
            BatchPayRollRequest.SuccessCount = 0;

                    StringBuilder wpBuilder = new StringBuilder();
                    StringBuilder whBuilder = new StringBuilder();

                    // Initialize or reset summary (WK) row for this TaxDiskette
                    var wkRow = _unitOfWork.Context.TaxDisketteWKs.SingleOrDefault(i => i.TaxDisketteId == TaxDiskette.Id);
                    if (wkRow == null)
                    {
                        wkRow = new TaxDisketteWK()
                        {
                            TaxDisketteId = TaxDiskette.Id,
                            CreateDate = DateTime.Now,
                            IPAddress = "WorkerService",
                            PaymentTypeId = (long)Enums.PaymentTypeTable15Tax.Cash_Deposit,
                            ExceptionsSubjectToTheBudgetLawOf1404 = 1,
                            CurrencyCode = 84,
                            ExchangeRateOfCurrency = 0,
                            title = ""
                        };
                        _unitOfWork.Context.TaxDisketteWKs.Add(wkRow);
                        SaveChangesWithDetailedLogging("Create TaxDisketteWK summary row");
                    }
                    else
                    {
                        // Reset all long fields to zero before accumulation
                        wkRow.GrossContinuousCashCurrentMonth = 0;
                        wkRow.ContinuousCashArearsNoTax = 0;
                        wkRow.EmployeeHousingDeductionCurrentMonth = 0;
                        wkRow.EmployeeCarDeductionCurrentMonth = 0;
                        wkRow.ContinuousNonCashOtherBenefitsCost = 0;
                        wkRow.ContinuousNonCashArearsNoTax = 0;
                        wkRow.ConsultingFeesAndSimilar = 0;
                        wkRow.ResearchContracts = 0;
                        wkRow.Overtime = 0;
                        wkRow.TravelExpense = 0;
                        wkRow.MissionAllowance = 0;
                        wkRow.Karaneh = 0;
                        wkRow.BonusExceptYearEndServiceEndProductivity = 0;
                        wkRow.YearEndBonus = 0;
                        wkRow.AnnualEydi = 0;
                        wkRow.EndOfServiceBonus = 0;
                        wkRow.DismissalCompensation = 0;
                        wkRow.ServiceBuyback = 0;
                        wkRow.SeverancePay = 0;
                        wkRow.UnusedLeavePay = 0;
                        wkRow.NonContinuousCashCurrentMonth = 0;
                        wkRow.NonContinuousCashArearsNoTax = 0;
                        wkRow.NonContinuousNonCashCostCurrentMonth = 0;
                        wkRow.NonContinuousNonCashArearsNoTax = 0;
                        wkRow.MedicalInsuranceArticle137 = 0;
                        wkRow.LifeInsuranceArticle137 = 0;
                        wkRow.TeachingResearchFees = 0;
                        wkRow.OnCallPay = 0;
                        wkRow.WelfareMotivationProductivity = 0;
                        wkRow.WorkEffortExcludingWageSalaryBonus = 0;
                        wkRow.PurePaymentAmount = 0;
                        wkRow.ExceptionsSubjectToTheBudgetLawOf1404 = 1;
                        wkRow.CurrencyCode = 84;
                        wkRow.ExchangeRateOfCurrency = 0;
                        wkRow.LastModifiedDate = DateTime.Now;
                        _unitOfWork.Context.Update(wkRow);
                        SaveChangesWithDetailedLogging("Reset TaxDisketteWK summary row");
                    }

                    foreach (var fiche in fiches)
                    {
                        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction ficheTransaction = null;
                        try
                        {
                            ficheTransaction = _unitOfWork.Context.Database.BeginTransaction();
                            DateTime now = DateTime.Now;
                            TaxDisketteWP currentFicheTaxDisketteWP = new TaxDisketteWP();

                            currentFicheTaxDisketteWP.TaxDisketteId = TaxDiskette.Id;
                            currentFicheTaxDisketteWP.EmployeeId = fiche.EmployeeId;
                            var employee = _employeeService.GetIdAsync(fiche.EmployeeId).Result;

                            #region FillWp - اطلاعات حقوق بگیر


                            #region ملیت

                            if (employee.NationalityId == 262) /// ایرانی
                            {
                                wpBuilder.Append("1" + customDelimiter);
                                currentFicheTaxDisketteWP.Nationality = 1;
                            }
                            else
                            {
                                wpBuilder.Append("2" + customDelimiter);
                                currentFicheTaxDisketteWP.Nationality = 2;
                            }

                            #endregion


                            var interdict = _orderService.GetIdAsync(fiche.InterdictOrderId).Result;

                            if (interdict != null)
                            {
                                #region کد ملی
                                if (string.IsNullOrEmpty(interdict.NationalNo))
                                {
                                    if (!string.IsNullOrEmpty(employee.NationalNo))
                                    {
                                        wpBuilder.Append(employee.NationalNo.PadLeft(10, '0') + customDelimiter);
                                        currentFicheTaxDisketteWP.NationalNo = employee.NationalNo.PadLeft(10, '0');
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append(interdict.NationalNo.PadLeft(10, '0') + customDelimiter);
                                    currentFicheTaxDisketteWP.NationalNo = interdict.NationalNo.PadLeft(10, '0');
                                }
                                #endregion کد ملی
                                #region نام و نام خانودگی
                                currentFicheTaxDisketteWP.FirstName = "";
                                if (string.IsNullOrEmpty(interdict.FirstName))
                                {
                                    if (!string.IsNullOrEmpty(employee.FirstName))
                                    {
                                        wpBuilder.Append(employee.FirstName + customDelimiter);
                                        currentFicheTaxDisketteWP.FirstName = employee.FirstName;
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append(interdict.FirstName + customDelimiter);
                                    currentFicheTaxDisketteWP.FirstName = interdict.FirstName;
                                }
                                currentFicheTaxDisketteWP.LastName = "";
                                if (string.IsNullOrEmpty(interdict.LastName))
                                {
                                    if (!string.IsNullOrEmpty(employee.LastName))
                                    {
                                        wpBuilder.Append(employee.LastName + customDelimiter);
                                        currentFicheTaxDisketteWP.LastName = employee.LastName;
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append(interdict.LastName + customDelimiter);
                                }


                                #endregion  نام و نام خانودگی
                                #region نام پدر
                                currentFicheTaxDisketteWP.FatherName = "";
                                if (string.IsNullOrEmpty(interdict.FatherName))
                                {
                                    if (!string.IsNullOrEmpty(employee.FatherName))
                                    {
                                        wpBuilder.Append(employee.FatherName + customDelimiter);
                                        currentFicheTaxDisketteWP.FatherName = employee.FatherName;
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append(interdict.FatherName + customDelimiter);
                                    currentFicheTaxDisketteWP.FatherName = interdict.FatherName;
                                }

                                #endregion
                                #region تاریخ تولد

                                if (interdict.BirthDate == null)
                                {
                                    if (employee.BirthDate != null)
                                    {
                                        wpBuilder.Append(GetNumericalDate(employee.BirthDate.Value) + customDelimiter);
                                        currentFicheTaxDisketteWP.BirthDate = Convert.ToInt32(GetNumericalDate(employee.BirthDate.Value));
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append(GetNumericalDate(interdict.BirthDate.Value) + customDelimiter);
                                    currentFicheTaxDisketteWP.BirthDate = Convert.ToInt32(GetNumericalDate(interdict.BirthDate.Value));
                                }

                                #endregion
                                #region شماره شناسنامه

                                if (string.IsNullOrEmpty(interdict.IdentityNo))
                                {
                                    if (!string.IsNullOrEmpty(employee.IdentityNo))
                                    {
                                        wpBuilder.Append(employee.IdentityNo.TrimStart('0') + customDelimiter);
                                        if (long.TryParse(employee.IdentityNo, out var identityNoLong) && identityNoLong <= int.MaxValue && identityNoLong >= int.MinValue)
                                        {
                                            currentFicheTaxDisketteWP.IdentityNo = (int)identityNoLong;
                                        }
                                        else
                                        {
                                            currentFicheTaxDisketteWP.IdentityNo = 0;
                                        }
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append(interdict.IdentityNo.TrimStart('0') + customDelimiter);
                                    if (long.TryParse(interdict.IdentityNo, out var interdictIdentityNoLong) && interdictIdentityNoLong <= int.MaxValue && interdictIdentityNoLong >= int.MinValue)
                                    {
                                        currentFicheTaxDisketteWP.IdentityNo = (int)interdictIdentityNoLong;
                                    }
                                    else
                                    {
                                        currentFicheTaxDisketteWP.IdentityNo = 0;
                                    }
                                }

                                #endregion
                                #region محل تولد
                                if (interdict.BirthPlaceId > 0)
                                {
                                    var place = (PlacesDTO)(_placesService.Get(interdict.BirthPlaceId.Value).Payload);
                                    if (place != null)
                                    {
                                        wpBuilder.Append(place.title + customDelimiter);
                                        currentFicheTaxDisketteWP.BirthPlace = place.title;
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);

                                    }
                                }
                                else
                                {
                                    if (employee.BirthPlaceId > 0)
                                    {
                                        var place = (PlacesDTO)(_placesService.Get(employee.BirthPlaceId.Value).Payload);
                                        if (place != null)
                                        {
                                            wpBuilder.Append(place.title + customDelimiter);
                                            currentFicheTaxDisketteWP.BirthPlace = place.title;
                                        }
                                        else
                                        {
                                            wpBuilder.Append(customDelimiter);
                                        }
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                //wpBuilder.Append('\x200E');
                                #endregion محل تولد
                                #region مقطع تحصیلی

                                if (interdict.EducationGradeId > 0)
                                {
                                    var EducationGrade = _educationGradeService.GetIdAsync(interdict.EducationGradeId.Value).Result;

                                    if (string.IsNullOrEmpty(EducationGrade.TaxCode))
                                    {
                                        wpBuilder.Append("1" + customDelimiter);  // زیر دیپلم
                                        currentFicheTaxDisketteWP.EducationGrade = "1";
                                    }
                                    else
                                    {
                                        wpBuilder.Append(EducationGrade.TaxCode + customDelimiter);
                                        currentFicheTaxDisketteWP.EducationGrade = EducationGrade.TaxCode;
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append("1" + customDelimiter);  // زیر دیپلم  
                                    currentFicheTaxDisketteWP.EducationGrade = "1";
                                }

                                #endregion مقطع تحصیلی
                                string insuranceName = string.Empty;
                                #region بیمه
                                if (interdict.InsuranceTypeId > 0)
                                {
                                    var insurance = _unitOfWork.Context.InsuranceTypes.Find(interdict.InsuranceTypeId);
                                    if (insurance == null)
                                    {
                                        wpBuilder.Append("5" + customDelimiter);
                                        currentFicheTaxDisketteWP.InsuranceTypeId = 5;
                                    }
                                    else
                                    {
                                        insuranceName = insurance.title;
                                        if (!string.IsNullOrEmpty(insurance.InsuranceCode))
                                        {
                                            wpBuilder.Append(insurance.InsuranceCode.Trim() + customDelimiter);
                                            currentFicheTaxDisketteWP.InsuranceTypeId = Convert.ToInt32(insurance.InsuranceCode.Trim());
                                        }
                                        else
                                        {
                                            wpBuilder.Append("5" + customDelimiter);
                                            currentFicheTaxDisketteWP.InsuranceTypeId = 5;
                                        }
                                    }
                                }
                                else
                                {
                                    wpBuilder.Append("5" + customDelimiter);
                                    currentFicheTaxDisketteWP.InsuranceTypeId = 5;
                                }

                                #region خواندن اطلاعات بیمه از پرونده
                                var insuranceRecors = _insuranceService.All().Where(i => i.EmployeeId == employee.Id);
                                if (insuranceRecors == null)
                                {
                                    wpBuilder.Append(customDelimiter);
                                }
                                else
                                {
                                    if (insuranceRecors.Any())
                                    {
                                        if (insuranceRecors.Any(i => i.IsLast == true))
                                        {
                                            if (insuranceRecors.Count(i => i.IsLast == true) == 1)
                                            {
                                                wpBuilder.Append(insuranceRecors.Single(i => i.IsLast == true).InsuranceNumber.PadLeft(10, '0') + customDelimiter);
                                                currentFicheTaxDisketteWP.InsuranceNo = insuranceRecors.Single(i => i.IsLast == true).InsuranceNumber.PadLeft(10, '0');


                                                if (currentFicheTaxDisketteWP.InsuranceNo.Length > 10)
                                                {
                                                    currentFicheTaxDisketteWP.InsuranceNo = "";
                                                }
                                            }
                                            else
                                            {
                                                wpBuilder.Append(customDelimiter);
                                            }
                                        }
                                        else
                                        {
                                            wpBuilder.Append(customDelimiter);
                                        }
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }

                                if (string.IsNullOrEmpty(insuranceName))
                                {
                                    wpBuilder.Append(customDelimiter);
                                }
                                else
                                {
                                    wpBuilder.Append(insuranceName + customDelimiter);
                                    currentFicheTaxDisketteWP.InsuranceName = insuranceName;
                                }
                                #endregion خواندن اطلاعات بیمه از پرونده

                                #endregion بیمه

                                #region نوع معافیت

                                if (employee.TaxExemptionTypeId > 0)
                                {
                                    var relatedExemptionType = _baseTableValueService._unitOfWork.Context.TaxExemptionTypes.Find(employee.TaxExemptionTypeId.Value);
                                    wpBuilder.Append(relatedExemptionType.Id + customDelimiter);
                                    currentFicheTaxDisketteWP.ExemptionType = relatedExemptionType.Id.ToString();
                                }
                                else
                                {
                                    wpBuilder.Append(customDelimiter);
                                }


                                #endregion  نوع معافیت


                                #region کشور محل تابعیت

                                wpBuilder.Append("103" + customDelimiter);
                                currentFicheTaxDisketteWP.CountryOfCitizenship = "103";

                                #endregion

                                #region کشور محل زندگی

                                wpBuilder.Append("103" + customDelimiter);
                                currentFicheTaxDisketteWP.CountryOfResidence = "103";

                                #endregion

                                #region اطلاعات تماس
                                currentFicheTaxDisketteWP.Address = "";
                                var ContactInfos = _contactInfoService.All().Where(i => i.EmployeeId == fiche.EmployeeId && i.IsLast == true && i.IsDeleted != true);

                                if (ContactInfos == null)
                                {
                                    wpBuilder.Append(customDelimiter);
                                }
                                else
                                {
                                    if (ContactInfos.Any())
                                    {
                                        var last = ContactInfos.OrderBy(i => i.CreateDate).Last();

                                        if (string.IsNullOrEmpty(last.Zipcode))
                                        {
                                            wpBuilder.Append(customDelimiter);
                                        }
                                        else
                                        {
                                            if (last.Zipcode.Length == 10)
                                            {
                                                if (IsDigitsOnly(last.Zipcode))
                                                {
                                                    wpBuilder.Append(last.Zipcode + customDelimiter);
                                                    currentFicheTaxDisketteWP.PostalCode = last.Zipcode;
                                                }
                                                else
                                                {
                                                    wpBuilder.Append(customDelimiter);
                                                }
                                            }
                                            else
                                            {
                                                wpBuilder.Append(customDelimiter);
                                            }
                                        }
                                        if (string.IsNullOrEmpty(last.Address))
                                        {
                                            wpBuilder.Append(customDelimiter);
                                        }
                                        else
                                        {
                                            wpBuilder.Append(last.Address.Trim() + customDelimiter);
                                            currentFicheTaxDisketteWP.Address = last.Address.Trim();
                                        }
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                        wpBuilder.Append(customDelimiter);
                                    }
                                }
                                #endregion اطلاعات تماس

                                #region رسته شغلی

                                var recruitOrder = _orderService._unitOfWork.Context.RecruitOrders.Find(interdict.RecruitOrderId);
                                if (recruitOrder.OrganizationJobId > 0)
                                {
                                    var relatedJob = _organisationChartService._unitOfWork.Context.OrganizationJobs.Find(recruitOrder.OrganizationJobId);
                                    if (relatedJob.TaxOccupationId > 0)
                                    {
                                        wpBuilder.Append(relatedJob.TaxOccupationId + customDelimiter);
                                        currentFicheTaxDisketteWP.Occupation = Convert.ToInt32(relatedJob.TaxOccupationId);
                                    }
                                    else
                                    {
                                        wpBuilder.Append(customDelimiter);
                                    }

                                    #endregion سمت

                                    #region سمت
                                    var job = _baseTableValueService._unitOfWork.Context.Jobs.Find(relatedJob.JobId);

                                    wpBuilder.Append(job.title + customDelimiter);
                                    currentFicheTaxDisketteWP.Position = job.title;

                                    #endregion



                                }
                                else
                                {
                                    wpBuilder.Append(customDelimiter);
                                }

                                #region نوع استخدام

                                var employeeTypeSetting = _organisationEmployeeTypeService.All().Where(i => i.OrganisationChartId == recruitOrder.PayLocationId && i.EmployeeTypeId == fiche.EmployeeTypeId);
                                if (employeeTypeSetting == null)
                                {
                                    wpBuilder.Append(customDelimiter);
                                }
                                else
                                {
                                    if (employeeTypeSetting.Any())
                                    {
                                        var singleSetting = employeeTypeSetting.Single();
                                        if (singleSetting.TaxBaseTable7Id.GetValueOrDefault() > 0)
                                        {
                                            var tableValue = _baseTableValueService.GetIdAsync(singleSetting.TaxBaseTable7Id.GetValueOrDefault()).Result;
                                            if (string.IsNullOrEmpty(tableValue.Value))
                                            {
                                                wpBuilder.Append("12" + customDelimiter);
                                                currentFicheTaxDisketteWP.EmployeeType = 12;
                                            }
                                            else
                                            {
                                                wpBuilder.Append(tableValue.Value.Trim() + customDelimiter);
                                                currentFicheTaxDisketteWP.EmployeeType = Convert.ToInt32(tableValue.Value.Trim());
                                            }
                                        }
                                        else
                                        {
                                            wpBuilder.Append("12" + customDelimiter);
                                            currentFicheTaxDisketteWP.EmployeeType = 12;
                                        }
                                    }
                                    else
                                    {
                                        wpBuilder.Append("12" + customDelimiter);
                                        currentFicheTaxDisketteWP.EmployeeType = 12;
                                    }
                                }

                                #endregion نوع استخدام
                                #region تاریخ استخدام و خاتمه همکاری
                                List<GetLightOrderListForInsuranceDiskette_Result> ret = new();
                                using (SqlConnection con = new SqlConnection(_connectionString))
                                {
                                    SqlCommand cmd = new SqlCommand("[Order].[GetLightOrderListForInsuranceDiskette]", con);
                                    cmd.Parameters.AddWithValue("@OrganisationChartId", _currentUserDefaultOrganId);
                                    cmd.Parameters.AddWithValue("@EmployeeId", fiche.EmployeeId);

                                    cmd.CommandType = CommandType.StoredProcedure;
                                    con.Open();
                                    SqlDataReader rdr = cmd.ExecuteReader();

                                    while (rdr.Read())
                                    {
                                        ret.Add(rdr.ConvertToObject<GetLightOrderListForInsuranceDiskette_Result>());
                                    }
                                    con.Close();
                                }

                                if (ret == null)
                                {
                                    throw new InvalidOperationException($"عدم بازگشت رکورد از [Order].[GetLightOrderListForInsuranceDiskette] برای کارکنان با کد ملی {employee.NationalNo} (FicheId:{fiche.Id})");
                                }
                                else
                                {
                                    if (ret.Any())
                                    {
                                        
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException($"عدم بازگشت رکورد از [Order].[GetLightOrderListForInsuranceDiskette] برای کارکنان با کد ملی {employee.NationalNo} (FicheId:{fiche.Id})");
                                    }
                                }

                                var firstOrder = ret.OrderBy(i => i.OrderSerial).First();
                                /// تاریخ شروع به کار
                                if (employee.StartWorkDate == null || employee.StartWorkDate == DateTime.MinValue)
                                {
                                    wpBuilder.Append(GetNumericalDate(firstOrder.StartDate.Value) + customDelimiter);
                                    currentFicheTaxDisketteWP.StartWorkDate = Convert.ToInt32(GetNumericalDate(firstOrder.StartDate.Value));
                                }
                                else
                                {
                                    wpBuilder.Append(GetNumericalDate(employee.StartWorkDate.Value) + customDelimiter);
                                    currentFicheTaxDisketteWP.StartWorkDate = Convert.ToInt32(GetNumericalDate(employee.StartWorkDate.Value));
                                }

                                #endregion   تاریخ استخدام و خاتمه همکاری
                                #region تاریخ پایان کار

                                var lastOrder = ret.Where(i => i.OrderStatusId == 9).Single();
                                if (lastOrder.IsEmployed == false && lastOrder.OrderDirectionTypeId == 1163) //  حکم خاتمه دهنده
                                {
                                    // تاریخ ترک کار
                                    wpBuilder.Append(GetNumericalDate(lastOrder.StartDate.Value) + customDelimiter);
                                    currentFicheTaxDisketteWP.EndWorkDate = Convert.ToInt32(GetNumericalDate(lastOrder.StartDate.Value));
                                }
                                else
                                {
                                    wpBuilder.Append(customDelimiter);
                                }

                                #endregion تاریخ پایان کار
                                #region بازنشستگی

                                //TODO
                                wpBuilder.Append(customDelimiter);

                                #endregion



                            }
                            var currentRowLastRecord = _unitOfWork.Context.TaxDisketteWPs.Where(i => i.FicheId == fiche.Id && i.TaxDisketteId == TaxDiskette.Id);

                            if (currentRowLastRecord == null)
                            {

                            }
                            else
                            {
                                if (currentRowLastRecord.Any())
                                {
                                    _unitOfWork.Context.TaxDisketteWPs.RemoveRange(currentRowLastRecord);
                                }
                            }
                            currentFicheTaxDisketteWP.FicheId = fiche.Id;
                            currentFicheTaxDisketteWP.IPAddress = "";
                            currentFicheTaxDisketteWP.title = "";
                            _unitOfWork.Context.TaxDisketteWPs.Add(currentFicheTaxDisketteWP);
                            SaveChangesWithDetailedLogging("Insert WP row");
                            // Ensure no trailing field delimiter(s) and append line terminator "\r\n"
                            while (wpBuilder.Length >= customDelimiter.Length &&
                                   wpBuilder.ToString(wpBuilder.Length - customDelimiter.Length, customDelimiter.Length) == customDelimiter)
                            {
                                wpBuilder.Remove(wpBuilder.Length - customDelimiter.Length, customDelimiter.Length);
                            }
                            wpBuilder.Append("\r\n");


                            #endregion FillWp - اطلاعات حقوق بگیر

                            var ficheItems = _unitOfWork.Context.FicheItems.Where(i => i.FicheId == fiche.Id);
                            BatchPayRollRequest.LastPoolingTime = DateTime.Now;
                            BatchPayRollRequest.PoolingEmployeeId = fiche.EmployeeId;
                            _unitOfWork.Context.Update(BatchPayRollRequest);
                            SaveChangesWithDetailedLogging("Update pooling employee and time on request");
                            #region BatchPayRollRequestDetail-Generation

                            BatchPayRollRequestDetail BatchPayRollRequestDetailRow = null;


                            #endregion BatchPayRollRequestDetail-Generation
                            #region TaxDisketteWh-Generation
                            // Insert TaxDisketteWh record if not exist
                            TaxDisketteWH relatedWhRow = null;
                            var result = _unitOfWork.Context.TaxDisketteWHs.Where(i => i.TaxDisketteId == TaxDiskette.Id && i.FicheId == fiche.Id).ToList();
                            bool needToCreateDetailRecord = false;
                            if (result == null)
                            {
                                needToCreateDetailRecord = true;
                            }
                            else
                            {
                                if (result.Any())
                                {
                                    relatedWhRow = result.Single();
                                }
                                else
                                {
                                    needToCreateDetailRecord = true;
                                }
                            }

                            if (needToCreateDetailRecord)
                            {
                                relatedWhRow = new TaxDisketteWH()
                                {
                                    TaxDisketteId = TaxDiskette.Id,
                                    CreateDate = DateTime.Now,
                                    EmployeeId = fiche.EmployeeId,
                                    InterdictOrderId = fiche.InterdictOrderId,
                                    FicheId = fiche.Id,
                                    IPAddress = "WorkerService",
                                    PaymentTypeId = (long)Enums.PaymentTypeTable15Tax.Cash_Deposit,
                                    title = ""
                                };
                                _unitOfWork.Context.TaxDisketteWHs.Add(relatedWhRow);
                                SaveChangesWithDetailedLogging("Create TaxDisketteWh row");
                            }
                            needToCreateDetailRecord = false;
                            BatchPayRollRequestDetail detailRow = null;
                            var BatchPayRollRequestDetail = _unitOfWork.Context.BatchPayRollRequestDetails.Where(i => i.BatchPayRollRequestId == BatchPayRollRequest.Id && i.FicheId == fiche.Id).ToList();

                            if (BatchPayRollRequestDetail == null)
                            {
                                needToCreateDetailRecord = true;
                            }
                            else
                            {
                                if (BatchPayRollRequestDetail.Any())
                                {
                                    detailRow = BatchPayRollRequestDetail.Single();
                                }
                                else
                                {
                                    needToCreateDetailRecord = true;
                                }
                            }
                            if (needToCreateDetailRecord)
                            {
                                detailRow = new BatchPayRollRequestDetail()
                                {
                                    CreateDate = DateTime.Now,
                                    IPAddress = "Job",
                                    InsuranceDisketteItemId = relatedWhRow.Id,
                                    BatchPayRollRequestId = BatchPayRollRequest.Id,
                                    FicheId = fiche.Id,
                                    EmployeeId = employee.Id,
                                    LastTryDateTime = DateTime.Now,
                                };
                                _unitOfWork.Context.BatchPayRollRequestDetails.Add(detailRow);
                                SaveChangesWithDetailedLogging("Create BatchPayRollRequestDetail row");
                            }
                            else
                            {
                                detailRow.LastTryDateTime = DateTime.Now;
                                detailRow.FinalMessage = "شروع محاسبه";
                                detailRow.InsuranceDisketteItemId = relatedWhRow.Id;
                                _unitOfWork.Context.BatchPayRollRequestDetails.Update(detailRow);
                                SaveChangesWithDetailedLogging("Update BatchPayRollRequestDetail to start");
                            }

                            #endregion TaxDisketteWh-Generation
                            #region Fill-TaxDisketteWh
                            // واکشی فهرست فیش های حقوق کارکنان جاری

                            if (relatedWhRow != null)
                            {



                                #region کد ملی
                                whBuilder.Append(employee.NationalNo.PadLeft(10, '0') + customDelimiter);
                                relatedWhRow.EmployeeId = employee.Id;
                                #endregion

                                #region  نوع پرداخت
                                //  همیشه ریال

                                whBuilder.Append("6" + customDelimiter);
                                relatedWhRow.PaymentTypeId = (long)Enums.PaymentTypeTable15Tax.Cash_Deposit;
                                #endregion


                                #region محل خدمت  - مرکز هزینه 
                                relatedWhRow.WorkplaceStatusId = 1;
                                var recruitOrder = _orderService._unitOfWork.Context.RecruitOrders.Find(interdict.RecruitOrderId);

                                if (recruitOrder.CostCenterId > 0)
                                {
                                    var costCenter = _organisationChartService.GetIdAsync(recruitOrder.CostCenterId).Result;
                                    //  whBuilder.Append(costCenter.title.Trim() + customDelimiter);
                                    relatedWhRow.WorkplaceStatusId = costCenter.TaxNodeStatusId.Value;
                                    if (costCenter.TaxNodeStatusId > 0)
                                    {
                                        var baseTableValue = _baseTableValueService.GetIdAsync(costCenter.TaxNodeStatusId.Value).Result;
                                        whBuilder.Append(baseTableValue.Value.Trim() + customDelimiter);
                                        relatedWhRow.WorkplaceStatusId = Convert.ToInt32(baseTableValue.Value.Trim());
                                    }
                                    else
                                    {
                                        whBuilder.Append(customDelimiter);
                                    }
                                }
                                else
                                {
                                    whBuilder.Append("1" + customDelimiter);
                                    relatedWhRow.WorkplaceStatusId = 1;
                                }

                                #endregion محل خدمت


                                #region جدول 10استثنائات قانون بودجه 
                                whBuilder.Append("1" + customDelimiter);
                                relatedWhRow.ExceptionsSubjectToTheBudgetLawOf1404 = 1;
                                #endregion

                                #region نوع ارز
                                whBuilder.Append("84" + customDelimiter);
                                relatedWhRow.CurrencyCode = 84;
                                #endregion
                                #region نرخ تسعیر ارز
                                whBuilder.Append(customDelimiter);
                                relatedWhRow.ExchangeRateOfCurrency = 0;
                                #endregion




                                #region Setting

                                List<OrganisationEmployeeTypeFicheItemDTO> organItems = null;

                                // آماده سازی اقلام تنظیمات نوع استخدام و اقلام فیش برای محاسبه تجمیعی
                                _ficheService
                                     .GetComputeSettings(fiche.EmployeeId, new FicheDTO(), new PaymentPeriod() { Id = fiche.PaymentPeriodId }, fiche.InterdictOrder.RecruitOrder, out organItems)
                                     ;
                                var ficheItemsList = ficheItems.ToList();

                                long SumValues(IEnumerable<long> wageItemIds, bool arrearOnly, bool isDeduction)
                                {
                                    var query = ficheItemsList.Where(fi => wageItemIds.Contains(fi.WageItemId));
                                    query = arrearOnly ? query.Where(fi => fi.IsArear == true) : query.Where(fi => fi.IsArear != true);
                                    query = isDeduction
                                        ? query.Where(fi => fi.PaymentTypeId == (long)Enums.PaymentType.Deduction)
                                        : query.Where(fi => fi.PaymentTypeId == (long)Enums.PaymentType.Payment);
                                    var sum = query.Sum(fi => fi.Value);
                                    return Convert.ToInt64(Math.Round(sum));
                                }

                                #endregion



                                #region مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsGrossContinuousCashCurrentMonth).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.GrossContinuousCashCurrentMonth = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.GrossContinuousCashCurrentMonth.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsContinuousCashArearsNoTax).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.ContinuousCashArearsNoTax = SumValues(wageIds, arrearOnly: true, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.ContinuousCashArearsNoTax.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsEmployeeHousingDeductionCurrentMonth).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.EmployeeHousingDeductionCurrentMonth = SumValues(wageIds, arrearOnly: false, isDeduction: true);
                                    whBuilder.Append(relatedWhRow.EmployeeHousingDeductionCurrentMonth.ToString() + customDelimiter);
                                }
                                #endregion
                                #region مسکن

                                relatedWhRow.HouseStatusId = 1;

                                var ContactInfos = _contactInfoService.All().Where(i => i.EmployeeId == fiche.EmployeeId && i.IsLast == true && i.IsDeleted != true);

                                if (ContactInfos == null)
                                {
                                    whBuilder.Append(customDelimiter);
                                }
                                else
                                {
                                    if (ContactInfos.Any())
                                    {
                                        var last = ContactInfos.OrderBy(i => i.CreateDate).Last();
                                        if (last.LocationTypeId > 0)
                                        {
                                            var code = (BaseTableValueDTO)_baseTableValueService.Get(last.LocationTypeId.Value).Payload;
                                            if (!string.IsNullOrEmpty(code.Value))
                                            {
                                                relatedWhRow.HouseStatusId = Convert.ToInt32(code.Value);
                                            }
                                            whBuilder.Append(code.Value + customDelimiter);
                                        }
                                        else
                                        {
                                            whBuilder.Append(customDelimiter);
                                        }

                                    }
                                    else
                                    {
                                        whBuilder.Append(customDelimiter);
                                    }
                                }


                                #endregion مسکن
                                #region مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsEmployeeCarDeductionCurrentMonth).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.EmployeeCarDeductionCurrentMonth = SumValues(wageIds, arrearOnly: false, isDeduction: true);
                                    whBuilder.Append(relatedWhRow.EmployeeCarDeductionCurrentMonth.ToString() + customDelimiter);
                                }
                                #endregion
                                #region   خودرو سازمانی

                                if (employee.VehicleStatusId == 21362)
                                {
                                    relatedWhRow.CarStatusId = 2;// با راننده
                                    whBuilder.Append("2" + customDelimiter);
                                }
                                else if (employee.VehicleStatusId == 21363)
                                {
                                    relatedWhRow.CarStatusId = 3;// بدون راننده
                                    whBuilder.Append("3" + customDelimiter);
                                }
                                else
                                {
                                    relatedWhRow.CarStatusId = 1;
                                    whBuilder.Append("1" + customDelimiter);
                                }

                                #endregion  خودرو سازمانی
                                #region مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsContinuousNonCashOtherBenefitsCost).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.ContinuousNonCashOtherBenefitsCost = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.ContinuousNonCashOtherBenefitsCost.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsContinuousNonCashArearsNoTax).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.ContinuousNonCashArearsNoTax = SumValues(wageIds, arrearOnly: true, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.ContinuousNonCashArearsNoTax.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف
                                {
                                    var wageIds = organItems.Where(i => i.IsConsultingFeesAndSimilar).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.ConsultingFeesAndSimilar = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.ConsultingFeesAndSimilar.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ قراردادهای پژوهشی- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsResearchContracts).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.ResearchContracts = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.ResearchContracts.ToString() + customDelimiter);
                                }
                                #endregion

                                #region اضافه کاری- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsOvertime).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.Overtime = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.Overtime.ToString() + customDelimiter);
                                }
                                #endregion

                                #region هزینه سفر- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsTravelExpense).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.TravelExpense = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.TravelExpense.ToString() + customDelimiter);
                                }
                                #endregion

                                #region فوق العاده مسافرت (ماموریت) - ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsMissionAllowance).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.MissionAllowance = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.MissionAllowance.ToString() + customDelimiter);
                                }
                                #endregion

                                #region کارانه- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsKaraneh).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.Karaneh = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.Karaneh.ToString() + customDelimiter);
                                }
                                #endregion

                                #region پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsBonusExceptYearEndServiceEndProductivity).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.BonusExceptYearEndServiceEndProductivity = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.BonusExceptYearEndServiceEndProductivity.ToString() + customDelimiter);
                                }
                                #endregion

                                #region پاداش آخر سال- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsYearEndBonus).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.YearEndBonus = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.YearEndBonus.ToString() + customDelimiter);
                                }
                                #endregion

                                #region عیدی سالانه- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsAnnualEydi).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.AnnualEydi = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.AnnualEydi.ToString() + customDelimiter);
                                }
                                #endregion

                                #region پاداش پایان خدمت- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsEndOfServiceBonus).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.EndOfServiceBonus = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.EndOfServiceBonus.ToString() + customDelimiter);
                                }
                                #endregion

                                #region خسارت اخراج- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsDismissalCompensation).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.DismissalCompensation = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.DismissalCompensation.ToString() + customDelimiter);
                                }
                                #endregion

                                #region بازخرید خدمت- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsServiceBuyback).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.ServiceBuyback = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.ServiceBuyback.ToString() + customDelimiter);
                                }
                                #endregion

                                #region حق سنوات- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsSeverancePay).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.SeverancePay = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.SeverancePay.ToString() + customDelimiter);
                                }
                                #endregion

                                #region حقوق ایام مرخصی استفاده نشده- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsUnusedLeavePay).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.UnusedLeavePay = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.UnusedLeavePay.ToString() + customDelimiter);
                                }
                                #endregion

                                #region سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsNonContinuousCashCurrentMonth).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.NonContinuousCashCurrentMonth = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.NonContinuousCashCurrentMonth.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsNonContinuousCashArearsNoTax).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.NonContinuousCashArearsNoTax = SumValues(wageIds, arrearOnly: true, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.NonContinuousCashArearsNoTax.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsNonContinuousNonCashCostCurrentMonth).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.NonContinuousNonCashCostCurrentMonth = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.NonContinuousNonCashCostCurrentMonth.ToString() + customDelimiter);
                                }
                                #endregion

                                #region مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
                                {
                                    var wageIds = organItems.Where(i => i.IsNonContinuousNonCashArearsNoTax).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.NonContinuousNonCashArearsNoTax = SumValues(wageIds, arrearOnly: true, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.NonContinuousNonCashArearsNoTax.ToString() + customDelimiter);
                                }
                                #endregion

                                #region حق بیمه های درمان موضوع ماده  137ق.م.م
                                {
                                    var wageIds = organItems.Where(i => i.IsMedicalInsuranceArticle137).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.MedicalInsuranceArticle137 = SumValues(wageIds, arrearOnly: false, isDeduction: true);
                                    whBuilder.Append(relatedWhRow.MedicalInsuranceArticle137.ToString() + customDelimiter);
                                }
                                #endregion

                                #region حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م
                                {
                                    var wageIds = organItems.Where(i => i.IsLifeInsuranceArticle137).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.LifeInsuranceArticle137 = SumValues(wageIds, arrearOnly: false, isDeduction: true);
                                    whBuilder.Append(relatedWhRow.LifeInsuranceArticle137.ToString() + customDelimiter);
                                }
                                #endregion


                                #region خالص پرداختی به حقوق بگیر

                                relatedWhRow.PurePaymentAmount = fiche.PurePaymentAmount;
                                whBuilder.Append(relatedWhRow.PurePaymentAmount.ToString() + customDelimiter);


                                #endregion


                                #region حق التدریس/حق التحقیق/ حق پژوهش
                                {
                                    var wageIds = organItems.Where(i => i.IsTeachingResearchFees).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.TeachingResearchFees = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.TeachingResearchFees.ToString() + customDelimiter);
                                }
                                #endregion

                                #region حق کشیک
                                {
                                    var wageIds = organItems.Where(i => i.IsOnCallPay).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.OnCallPay = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.OnCallPay.ToString() + customDelimiter);
                                }
                                #endregion

                                #region رفاهی و انگیزشی و بهره وری
                                {
                                    var wageIds = organItems.Where(i => i.IsWelfareMotivationProductivity).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.WelfareMotivationProductivity = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.WelfareMotivationProductivity.ToString() + customDelimiter);
                                }
                                #endregion

                                #region حق السعی ( به استثنای مزد، حقوق، پاداش )
                                {
                                    var wageIds = organItems.Where(i => i.IsWorkEffortExcludingWageSalaryBonus).Select(i => i.WageItemId).Distinct();
                                    relatedWhRow.WorkEffortExcludingWageSalaryBonus = SumValues(wageIds, arrearOnly: false, isDeduction: false);
                                    whBuilder.Append(relatedWhRow.WorkEffortExcludingWageSalaryBonus.ToString() );
                                }
                                #endregion



                                relatedWhRow.LastModifiedDate = DateTime.Now;
                                _unitOfWork.Context.Update(relatedWhRow);
                                SaveChangesWithDetailedLogging("Update TaxDisketteWh after fill");

                                // Accumulate into WK summary using the tracked wkRow instance
                                wkRow.GrossContinuousCashCurrentMonth += relatedWhRow.GrossContinuousCashCurrentMonth;
                                wkRow.ContinuousCashArearsNoTax += relatedWhRow.ContinuousCashArearsNoTax;
                                wkRow.EmployeeHousingDeductionCurrentMonth += relatedWhRow.EmployeeHousingDeductionCurrentMonth;
                                wkRow.EmployeeCarDeductionCurrentMonth += relatedWhRow.EmployeeCarDeductionCurrentMonth;
                                wkRow.ContinuousNonCashOtherBenefitsCost += relatedWhRow.ContinuousNonCashOtherBenefitsCost;
                                wkRow.ContinuousNonCashArearsNoTax += relatedWhRow.ContinuousNonCashArearsNoTax;
                                wkRow.ConsultingFeesAndSimilar += relatedWhRow.ConsultingFeesAndSimilar;
                                wkRow.ResearchContracts += relatedWhRow.ResearchContracts;
                                wkRow.Overtime += relatedWhRow.Overtime;
                                wkRow.TravelExpense += relatedWhRow.TravelExpense;
                                wkRow.MissionAllowance += relatedWhRow.MissionAllowance;
                                wkRow.Karaneh += relatedWhRow.Karaneh;
                                wkRow.BonusExceptYearEndServiceEndProductivity += relatedWhRow.BonusExceptYearEndServiceEndProductivity;
                                wkRow.YearEndBonus += relatedWhRow.YearEndBonus;
                                wkRow.AnnualEydi += relatedWhRow.AnnualEydi;
                                wkRow.EndOfServiceBonus += relatedWhRow.EndOfServiceBonus;
                                wkRow.DismissalCompensation += relatedWhRow.DismissalCompensation;
                                wkRow.ServiceBuyback += relatedWhRow.ServiceBuyback;
                                wkRow.SeverancePay += relatedWhRow.SeverancePay;
                                wkRow.UnusedLeavePay += relatedWhRow.UnusedLeavePay;
                                wkRow.NonContinuousCashCurrentMonth += relatedWhRow.NonContinuousCashCurrentMonth;
                                wkRow.NonContinuousCashArearsNoTax += relatedWhRow.NonContinuousCashArearsNoTax;
                                wkRow.NonContinuousNonCashCostCurrentMonth += relatedWhRow.NonContinuousNonCashCostCurrentMonth;
                                wkRow.NonContinuousNonCashArearsNoTax += relatedWhRow.NonContinuousNonCashArearsNoTax;
                                wkRow.MedicalInsuranceArticle137 += relatedWhRow.MedicalInsuranceArticle137;
                                wkRow.LifeInsuranceArticle137 += relatedWhRow.LifeInsuranceArticle137;
                                wkRow.TeachingResearchFees += relatedWhRow.TeachingResearchFees;
                                wkRow.OnCallPay += relatedWhRow.OnCallPay;
                                wkRow.WelfareMotivationProductivity += relatedWhRow.WelfareMotivationProductivity;
                                wkRow.WorkEffortExcludingWageSalaryBonus += relatedWhRow.WorkEffortExcludingWageSalaryBonus;
                                wkRow.PurePaymentAmount += relatedWhRow.PurePaymentAmount;
                                wkRow.LastModifiedDate = DateTime.Now;
                                _unitOfWork.Context.Update(wkRow);
                                SaveChangesWithDetailedLogging("Accumulate TaxDisketteWK totals");

                                #region whBuilder 

                                // Append line terminator "\r\n"
                                whBuilder.Append("\r\n");
                                #endregion whBuilder 

                            }

                            #endregion Fill-TaxDisketteWh

                            detailRow.RunTimeinMilliseconds = (DateTime.Now - now).TotalMilliseconds;
                            detailRow.DoDatetime = DateTime.Now;
                            detailRow.LastModifiedDate = DateTime.Now;
                            detailRow.FinalMessage = "لحاظ شده در دیسکت";
                            if (detailRow.Id > 0)
                            {
                                _unitOfWork.Context.BatchPayRollRequestDetails.Update(detailRow);
                            }
                            else
                            {
                                _unitOfWork.Context.BatchPayRollRequestDetails.Add(detailRow);
                            }
                            SaveChangesWithDetailedLogging("Persist success detail for fiche");

                            IncrementTaxBatchRequestSuccessInDatabase(BatchPayRollRequest);
                            ficheTransaction.Commit();
                            ficheTransaction.Dispose();
                            ficheTransaction = null;


                        }
                        catch (Exception ex)
                        {
                            if (ficheTransaction != null)
                            {
                                try
                                {
                                    ficheTransaction.Rollback();
                                }
                                catch { }
                                ficheTransaction.Dispose();
                                ficheTransaction = null;
                            }
                            try
                            {
                                var friendlyMessage = BuildTaxUserFriendlyErrorMessage(
                                    ex,
                                    $"محاسبه دیسکت مالیات برای فیش {fiche.Id} (کارمند {fiche.EmployeeId})");

                                var detailRowErr = _unitOfWork.Context.BatchPayRollRequestDetails.Where(i => i.BatchPayRollRequestId == BatchPayRollRequest.Id && i.FicheId == fiche.Id).SingleOrDefault();
                                if (detailRowErr != null)
                                {
                                    detailRowErr.DoDatetime = DateTime.Now;
                                    detailRowErr.FinalMessage = friendlyMessage;
                                    _unitOfWork.Context.BatchPayRollRequestDetails.Update(detailRowErr);
                                    SaveChangesWithDetailedLogging("Persist error message for fiche detail");
                                }
                                else
                                {
                                    var newDetail = new BatchPayRollRequestDetail()
                                    {
                                        CreateDate = DateTime.Now,
                                        IPAddress = TaxWorkerIpAddress,
                                        BatchPayRollRequestId = BatchPayRollRequest.Id,
                                        FicheId = fiche.Id,
                                        EmployeeId = fiche.EmployeeId,
                                        LastTryDateTime = DateTime.Now,
                                        DoDatetime = DateTime.Now,
                                        FinalMessage = friendlyMessage,
                                    };
                                    _unitOfWork.Context.BatchPayRollRequestDetails.Add(newDetail);
                                    SaveChangesWithDetailedLogging("Create error detail for fiche");
                                }
                            }
                            catch (Exception persistEx)
                            {
                                _logger.LogError(persistEx, "خطا در ذخیره پیام خطای فیش {FicheId}", fiche.Id);
                            }
                            _logger.LogError(ex, "خطا در پردازش فیش {FicheId} در درخواست {RequestId}", fiche.Id, BatchPayRollRequest.Id);
                        }


                    }




                    #region ساخت فایل حقوق بگیران - WP

                    //.Replace('\x200E', '\0')

                    var normalizedWP = NormalizeFieldCounts(wpBuilder.ToString(), WPExpectedDelimiterCount);
                    var Content = normalizedWP
                        .Replace("\u200E", string.Empty) // remove LRM
                        .Replace(",", " ")            // strip ASCII commas inside fields
                        .Replace("،", " ")             // strip Arabic comma inside fields
                        .Replace("\u066C", " ")        // strip Arabic thousands separator
                        .Replace("\uFF0C", " ")        // strip fullwidth comma
                        .Replace(customDelimiter, Convert.ToString((char)44));  // finalize with ASCII comma as delimiter

                    TaxDisketteFile taxDisketteFile = new TaxDisketteFile()
                    {
                        CreateDate = DateTime.Now,
                        IPAddress = "",
                        Content = Content,
                        TaxDisketteId = TaxDiskette.Id,
                        FileName = "WP" + CurrentPeriod.ShamsiYear + CurrentPeriod.ShamsiMonth.ToString().PadLeft(2, '0'),
                        Extension = "txt",
                    };

                    _unitOfWork.Context.Entry(taxDisketteFile).Property("FileTypeId").CurrentValue = (long)Enums.TaxDisketteFileType.WP;
                    _unitOfWork.Context.TaxDisketteFiles.Add(taxDisketteFile);
                    SaveChangesWithDetailedLogging("Create WP file record");

                    #endregion ساخت فایل حقوق بگیران - WP
                    #region ساخت فایل فهرست حقوق - Wh



                    var normalizedWH = NormalizeFieldCounts(whBuilder.ToString(), WHExpectedDelimiterCount);
                    var WHContent = normalizedWH
                        .Replace("\u200E", string.Empty) // remove LRM
                        .Replace(",", " ")            // strip ASCII commas inside fields
                        .Replace("،", " ")             // strip Arabic comma inside fields
                        .Replace("\u066C", " ")        // strip Arabic thousands separator
                        .Replace("\uFF0C", " ")        // strip fullwidth comma
                        .Replace(customDelimiter, Convert.ToString((char)44));  // finalize with ASCII comma as delimiter
                    TaxDisketteFile taxDisketteFileWH = new TaxDisketteFile()
                    {
                        CreateDate = DateTime.Now,
                        IPAddress = "",
                        Content = WHContent,
                        TaxDisketteId = TaxDiskette.Id,

                        FileName = "WH" + CurrentPeriod.ShamsiYear + CurrentPeriod.ShamsiMonth.ToString().PadLeft(2, '0'),
                        Extension = "txt",
                    };
                    _unitOfWork.Context.TaxDisketteFiles.Add(taxDisketteFileWH);
                    _unitOfWork.Context.Entry(taxDisketteFileWH).Property("FileTypeId").CurrentValue = (long)Enums.TaxDisketteFileType.WH;
                    SaveChangesWithDetailedLogging("Create WH file record");
                    #endregion  ساخت فایل فهرست حقوق - WH

                    var summaryMessage = BatchPayRollRequest.SuccessCount == fiches.Count
                        ? null
                        : $"از {fiches.Count} فیش، {BatchPayRollRequest.SuccessCount} مورد با موفقیت در دیسکت مالیات لحاظ شد. جزئیات هر فیش را در لیست ریز عملیات ببینید.";

                    CompleteTaxDisketteRequest(BatchPayRollRequest, TaxDiskette, summaryMessage);

                    _logger.LogInformation(
                        "پایان پردازش درخواست {RequestId}. موفق: {Success}/{Total}",
                        BatchPayRollRequest.Id, BatchPayRollRequest.SuccessCount, fiches.Count);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            if (enteredRunning)
            {
                EnsureTaxBatchRequestNotLeftRunning(
                    BatchPayRollRequest.Id,
                    TaxDiskette?.Id,
                    "پردازش دیسکت مالیات ناقص ماند و سیستم وضعیت درخواست را به‌صورت خودکار بست. لطفاً جزئیات را بررسی و در صورت نیاز «تلاش مجدد» بزنید.");
            }
        }
    }

    // همگام سازی وضعیت درخواست و دیسکت مالیات (ExecuteUpdate — منبع حقیقت پایگاه داده)
    private void UpdateRequestAndTaxDisketteStateTransactional(BatchPayRollRequest request, Enums.BatchPayRollRequestState newState, TaxDiskette? taxDiskette, Action? mutateDisketteBeforeSave)
    {
        mutateDisketteBeforeSave?.Invoke();
        var isTerminal = newState == Enums.BatchPayRollRequestState.EndLoop
                         || newState == Enums.BatchPayRollRequestState.Deleted
                         || newState == Enums.BatchPayRollRequestState.CancelByUser;

        SetTaxBatchRequestStateInDatabase(
            request,
            newState,
            request.ExeptionMessage,
            setFinishDate: isTerminal);

        if (taxDiskette != null)
        {
            SetTaxDisketteStatusInDatabase(taxDiskette.Id, taxDiskette.TaxDisketteStatusId);
        }
    }
    bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }
    private string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
    private string NormalizeFieldCounts(string content, int targetFieldCount)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }

        string lineBreak = Environment.NewLine;
        var lines = content.Split(new[] { lineBreak }, StringSplitOptions.None);
        var normalized = new StringBuilder();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
            {
                // Preserve empty lines (e.g., final trailing newline)
                normalized.Append(line);
                if (i < lines.Length - 1)
                {
                    normalized.Append(lineBreak);
                }
                continue;
            }

            int delimiterCount = 0;
            int index = 0;
            while (true)
            {
                int foundAt = line.IndexOf(customDelimiter, index, StringComparison.Ordinal);
                if (foundAt < 0)
                {
                    break;
                }
                delimiterCount++;
                index = foundAt + customDelimiter.Length;
            }

            int toPad = targetFieldCount - delimiterCount;
            if (toPad > 0)
            {
                // Appending additional delimiters adds blank fields (results in ",," after CSV conversion)
                line += string.Concat(Enumerable.Repeat(customDelimiter, toPad));
            }

            normalized.Append(line);
            if (i < lines.Length - 1)
            {
                normalized.Append(lineBreak);
            }
        }

        return normalized.ToString();
    }
    public new OperationResult CreateForAsync(TaxDisketteDTO entityToCreate)
    {
        if (entityToCreate.PaymentPeriodId > 0)
        {
            List<long> invalidStatusList = new()
                {
                (long)TaxDisketteStatus.Initial,
                (long)TaxDisketteStatus.CalculationFinished,
                (long)TaxDisketteStatus.Payed,
                };

            if (_unitOfWork.Context.TaxDiskettes.Any(i => i.PaymentPeriodId == entityToCreate.PaymentPeriodId && invalidStatusList.Contains(i.TaxDisketteStatusId)))
            {
                //return OperationResult.Failed("برای دوره ارسالی دیسکت مالیات وجود دارد لطفا ابتدا دیسکت قبل را حذف بفرمایید");
            }

            var period = _unitOfWork.Context.PaymentPeriods.Find(entityToCreate.PaymentPeriodId);

            if (period.IsClosed == true)
            {
                return OperationResult.Failed("دوره جاری پیش فرض بسته می باشد و امکان تهیه دیسکت وجود ندارد");

            }

        }
        else
        {
            return OperationResult.Failed("دوره محاسبه ارسال نشده است");
        }

        var mappedTodo = _mapper.Map<TaxDiskette>(entityToCreate);
        if (mappedTodo == null)
        {
            return OperationResult.NotFound();
        }
        List<OrganisationCostCenter> costCenterList = new List<OrganisationCostCenter>();
        _unitOfWork.Context.Entry(mappedTodo).Property("TaxDisketteStatusId").CurrentValue = (long)TaxDisketteStatus.Initial;
        var fiches = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == entityToCreate.PaymentPeriodId && (i.FicheStatusId == (long)Enums.FicheStatus.Initial || i.FicheStatusId == (long)Enums.FicheStatus.Payed)).ToList();
        if (entityToCreate.CalculateAllFichesInCurrentPeriod == true)
        {

        }
        else
        {
            costCenterList = _organisationCostCenterService.All().Where(i => entityToCreate.CostCenterIdList.Contains(i.Id)).ToList();
            var idList = costCenterList.Select(i => i.CostCenterId).Distinct().ToList();
            var includeFiches = fiches.Where(i => idList.Contains(i.CostCenterId));
        }
        mappedTodo.BatchPayRollRequestId = null;
        _unitOfWork.CreateTransaction();
        try
        {
            if (typeof(TaxDiskette).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                if (_currentUserDefaultOrganId > 0)
                {
                    PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                    if (propertyInfo == null)
                    {
                        throw new Exception("سازمان پیش فرض مشخض نشده است");
                    }
                    propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                    var orgChartSetting = _unitOfWork.Context.OrganProperties.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
                    if (orgChartSetting == null)
                    {
                        throw new Exception("مشخصات مالیات ای محل پرداخت مورد نظر تعریف نشده است");
                    }
                    else
                    {
                        if (orgChartSetting.Any())
                        {
                            var settings = orgChartSetting.ToList();
                            if (settings.Count != 1)
                            {
                                _logger.LogError("تعداد تنظیمات سازمان برای مالیات نامعتبر است. OrganisationChartId={OrganisationChartId}, Count={Count}", _currentUserDefaultOrganId, settings.Count);
                                throw new Exception("برای محل پرداخت انتخاب‌شده باید دقیقا 1 رکورد تنظیمات وجود داشته باشد");
                            }

                            var s = settings[0];

                            var missing = new List<string>();
                            if (string.IsNullOrWhiteSpace(s.TaxEconomicNo)) missing.Add("کد اقتصادی مالیاتی");
                            if (!s.TaxBranchCode.HasValue) missing.Add("کد شعبه مالیاتی");
                            if (string.IsNullOrWhiteSpace(s.TaxBranchName)) missing.Add("نام شعبه مالیاتی");
                            if (string.IsNullOrWhiteSpace(s.TaxDocumentNo)) missing.Add("شماره سند مالیاتی");
                            if (string.IsNullOrWhiteSpace(s.TaxPostalCode)) missing.Add("کد پستی مالیاتی");
                            if (string.IsNullOrWhiteSpace(s.TaxPhoneNo)) missing.Add("شماره تماس مالیاتی");
                            if (string.IsNullOrWhiteSpace(s.TaxFirstEmployerNationalNo)) missing.Add("کد ملی نفر اول کارفرما (مالیات)");
                            if (string.IsNullOrWhiteSpace(s.TaxFirstEmployerFirstName)) missing.Add("نام نفر اول کارفرما (مالیات)");
                            if (string.IsNullOrWhiteSpace(s.TaxFirstEmployerLastName)) 
                            {
                                missing.Add("نام خانوادگی نفر اول کارفرما (مالیات)");
                                // لاگ دقیق‌تر برای دیباگ
                                _logger.LogWarning("TaxFirstEmployerLastName is null or whitespace. Value: '{Value}', Length: {Length}, IsNull: {IsNull}", 
                                    s.TaxFirstEmployerLastName ?? "NULL", 
                                    s.TaxFirstEmployerLastName?.Length ?? 0,
                                    s.TaxFirstEmployerLastName == null);
                            }
                            if (string.IsNullOrWhiteSpace(s.TaxFirstEmployerPostName)) missing.Add("سمت نفر اول کارفرما (مالیات)");

                            if (missing.Any())
                            {
                                var missingFieldsText = string.Join("، ", missing);
                                var errorMessage = $"تنظیمات مالیات محل پرداخت ناقص است. فیلدهای زیر الزامی هستند و باید تکمیل شوند: {missingFieldsText}";
                                _logger.LogError("تنظیمات مالیات برای OrganisationChartId={OrganisationChartId} ناقص است. فیلدهای خالی: {Missing}. TaxFirstEmployerLastName Value: '{LastNameValue}'", 
                                    _currentUserDefaultOrganId, 
                                    missingFieldsText,
                                    s.TaxFirstEmployerLastName ?? "NULL");
                                throw new Exception(errorMessage);
                            }
                        }
                        else
                        {
                            throw new Exception("مشخصات مالیات ای محل پرداخت مورد نظر تعریف نشده است");
                        }
                    }
                }
                else
                {
                    _unitOfWork.Rollback();
                    throw new Exception("سازمان پیش فرض مشخض نشده است");
                }
            }
            mappedTodo.IPAddress = "";
            mappedTodo.CreateDate = DateTime.Now;
            _unitOfWork.Context.TaxDiskettes.Add(mappedTodo);
            _unitOfWork.Context.SaveChanges();


            if (entityToCreate.CalculateAllFichesInCurrentPeriod == true)
            {
                entityToCreate.CostCenterIdList = new List<long>() { };
            }
            else
            {
                if (entityToCreate.CostCenterIdList == null)
                {
                    _unitOfWork.Rollback();
                    throw new Exception("فهرست مراکز هزینه ارسال نشده است");
                }
                else
                {
                    if (entityToCreate.CostCenterIdList.Any())
                    {
                        List<TaxDisketteCostCenter> CostCenterList = new List<TaxDisketteCostCenter>();
                        foreach (var CostCenter in costCenterList)
                        {
                            TaxDisketteCostCenter toAdd = new TaxDisketteCostCenter()
                            {
                                TaxDisketteId = mappedTodo.Id,
                                CostCenterId = CostCenter.CostCenterId,
                                CreateDate = DateTime.Now,
                                IPAddress = "",
                            };
                            CostCenterList.Add(toAdd);
                        }
                        _unitOfWork.Context.TaxDisketteCostCenters.AddRange(CostCenterList);
                        _unitOfWork.Context.SaveChanges();
                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        throw new Exception("فهرست مراکز هزینه ارسال نشده است");
                    }
                }
            }


            BatchPayRollRequest batch = new BatchPayRollRequest()
            {
                RequestTypeId = (long)Enums.BatchPayRollRequestType.TaxDisketteCalculation,
                EmployeeCount = fiches.Count(),
                IPAddress = "",
                CreateDate = DateTime.Now,
                OrganisationChartId = mappedTodo.OrganisationChartId,
                PaymentPeriodId = mappedTodo.PaymentPeriodId,
                RequestStateId = (long)Enums.BatchPayRollRequestState.Initial,
                UserId = _userResolverService.GetUserId(),
                Username = string.IsNullOrWhiteSpace(_userResolverService.fullname()) ? _userResolverService.GetUser() : _userResolverService.fullname(),
                SuccessCount = 0,
                RequsetDescription = "به درخواست صدور دیسکت مالیات",
                TaxDisketteId = mappedTodo.Id
            };
            _unitOfWork.Context.BatchPayRollRequests.Add(batch);
            _unitOfWork.Context.SaveChanges();

            mappedTodo.BatchPayRollRequestId = batch.Id;
            _unitOfWork.Context.Update(mappedTodo);

            _unitOfWork.Context.SaveChanges();


            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
            return OperationResult.Succeeded(payload: batch.Id);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            _logger.LogError(ex, "خطا در ایجاد دیسکت مالیات");
            
            // استخراج پیام خطای واضح برای کاربر
            string errorMessage = ex.Message;
            
            // اگر پیام خطا به فارسی است، همان را استفاده کن
            if (!string.IsNullOrWhiteSpace(errorMessage) && 
                (errorMessage.Contains("سازمان") || errorMessage.Contains("مشخصات") || 
                 errorMessage.Contains("تنظیمات") || errorMessage.Contains("مراکز هزینه")))
            {
                return OperationResult.Failed(errorMessage);
            }
            
            // برای خطاهای دیتابیس، پیام مناسب برگردان
            if (ex is DbUpdateException || ex is DbUpdateConcurrencyException)
            {
                return OperationResult.Failed("خطا در ذخیره اطلاعات در پایگاه داده. لطفاً دوباره تلاش کنید.");
            }
            
            // برای سایر خطاها، پیام عمومی برگردان
            return OperationResult.Failed($"خطا در ایجاد دیسکت مالیات: {errorMessage}");
        }

    }
    public OperationResult CheckIfPeriodIsValidForCreateTaxDiskette(long PaymentPeriodId)
    {
        try
        {
            List<long> invalidStatusList = new List<long>()
            {
                (long)Enums.TaxDisketteStatus.Initial,
                (long)Enums.TaxDisketteStatus.CalculationFinished,
                (long)Enums.TaxDisketteStatus.Payed,
            };
            var existingTaxDiskette = _unitOfWork.Context.TaxDiskettes.Where(i => invalidStatusList.Contains(i.TaxDisketteStatusId) && i.PaymentPeriodId == PaymentPeriodId);
            if (existingTaxDiskette == null)
            {

            }
            else
            {
                if (existingTaxDiskette.Any())
                {
                    //   return OperationResult.Failed("برای دوره مورد نظر دیسکت وجود دارد");
                }
            }
            var PaymentPeriod = _unitOfWork.Context.PaymentPeriods.Find(PaymentPeriodId);
            if (PaymentPeriod == null)
            {
                return OperationResult.NotFound("دوره پرداخت یافت نشد");
            }
            else
            {
                if (PaymentPeriod.OrganisationChartId == _currentUserDefaultOrganId)
                {

                }
                else
                {
                    return OperationResult.Failed("محل پرداخت پیش فرض با دوره ارسالی مطابقت ندارد");
                }

                if (PaymentPeriod.IsClosed == true)
                {
                    return OperationResult.Failed("دوره مورد نظر بسته است و امکان تهیه دیسکت وجود ندارد");
                }
                else
                {
                    var ficheCount = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == PaymentPeriod.Id && (i.FicheStatusId == (long)Enums.FicheStatus.Initial || i.FicheStatusId == (long)Enums.FicheStatus.Payed)).Count();

                    if (ficheCount > 0)
                    {
                        return OperationResult.Succeeded("تعداد " + ficheCount + " فیش برای دوره انتخابی یافت شد ");
                    }
                    else
                    {
                        return OperationResult.Failed("برای دوره انتخابی هیچ فیش محاسبه شده ای یافت نشد");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بررسی اعتبار دوره برای ایجاد دیسکت مالیات - PaymentPeriodId: {PaymentPeriodId}", PaymentPeriodId);
            return OperationResult.Failed($"خطا در بررسی اعتبار دوره: {ex.Message}");
        }
    }
    public bool Validate(TaxDiskette entity, object etc = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// ذخیره فایل پاسخ محاسبه مالیات و تغییر وضعیت درخواست گروهی به تلاش مجدد
    /// </summary>
    public async Task<OperationResult> ProcessTaxResponseFile(long batchPayRollRequestId, string fileContent)
    {
        try
        {
            // Find the TaxDiskette
            var taxDiskette = _unitOfWork.Context.TaxDiskettes
                .Include(t => t.BatchPayRollRequest)
                .FirstOrDefault(t => t.BatchPayRollRequestId == batchPayRollRequestId);

            if (taxDiskette == null)
            {
                return OperationResult.NotFound("دیسکت مالیات برای این درخواست گروهی یافت نشد");
            }

            var lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
            {
                return OperationResult.Failed("فایل خالی است");
            }

            // Save the file to TaxDisketteFile table
            try
            {
                var existingFile = _unitOfWork.Context.TaxDisketteFiles
                    .FirstOrDefault(f => f.TaxDisketteId == taxDiskette.Id
                                      && f.FileTypeId == (long)Enums.TaxDisketteFileType.TaxResponse);

                if (existingFile != null)
                {
                    // Update existing file
                    existingFile.Content = fileContent;
                    existingFile.LastModifiedDate = DateTime.Now;
                    existingFile.IPAddress = "";
                    _unitOfWork.Context.TaxDisketteFiles.Update(existingFile);
                }
                else
                {
                    // Create new file
                    var taxDisketteFile = new TaxDisketteFile
                    {
                        TaxDisketteId = taxDiskette.Id,
                        FileTypeId = (long)Enums.TaxDisketteFileType.TaxResponse,
                        Content = fileContent,
                        FileName = $"TaxResponse_{batchPayRollRequestId}",
                        Extension = "txt",
                        CreateDate = DateTime.Now,
                        IPAddress = "",
                        title = $"فایل پاسخ مالیات - {batchPayRollRequestId}"
                    };
                    _unitOfWork.Context.TaxDisketteFiles.Add(taxDisketteFile);
                }

                await _unitOfWork.Context.SaveChangesAsync();
                _logger.LogInformation("فایل پاسخ مالیات برای BatchPayRollRequestId {BatchPayRollRequestId} ذخیره شد", batchPayRollRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ذخیره فایل پاسخ مالیات");
                return OperationResult.Failed("خطا در ذخیره فایل: " + ex.Message);
            }

            // Change BatchPayRollRequest status to TryAgain
            var batchRequest = taxDiskette.BatchPayRollRequest;
            if (batchRequest != null)
            {
                batchRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.TryAgain;
                batchRequest.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.BatchPayRollRequests.Update(batchRequest);
                await _unitOfWork.Context.SaveChangesAsync();
                _logger.LogInformation("وضعیت BatchPayRollRequest {BatchPayRollRequestId} به TryAgain تغییر کرد", batchPayRollRequestId);
            }

            return OperationResult.Succeeded($"فایل با موفقیت ذخیره شد. تعداد {lines.Length} سطر در فایل وجود دارد. پردازش در Hosted Service انجام خواهد شد.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای کلی در ذخیره فایل پاسخ مالیات");
            return OperationResult.Failed("خطا در پردازش فایل: " + ex.Message);
        }
    }

    /// <summary>
    /// پردازش فایل TaxResponse برای یک BatchPayRollRequest - فراخوانی از Hosted Service
    /// </summary>
    public async Task<(int successCount, int failCount, List<string> errors)> ProcessTaxResponseFileForBatch(long batchPayRollRequestId)
    {
        int successCount = 0;
        int failCount = 0;
        var errors = new List<string>();

        try
        {
            // Find the TaxDiskette and file
            var taxDiskette = _unitOfWork.Context.TaxDiskettes
                .FirstOrDefault(t => t.BatchPayRollRequestId == batchPayRollRequestId);

            if (taxDiskette == null)
            {
                errors.Add("دیسکت مالیات یافت نشد");
                return (0, 1, errors);
            }

            var taxResponseFile = _unitOfWork.Context.TaxDisketteFiles
                .FirstOrDefault(f => f.TaxDisketteId == taxDiskette.Id
                                  && f.FileTypeId == (long)Enums.TaxDisketteFileType.TaxResponse);

            if (taxResponseFile == null || string.IsNullOrWhiteSpace(taxResponseFile.Content))
            {
                errors.Add("فایل TaxResponse یافت نشد");
                return (0, 1, errors);
            }

            var lines = taxResponseFile.Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            _logger.LogInformation("شروع پردازش {LineCount} خط از فایل TaxResponse برای BatchPayRollRequestId {BatchPayRollRequestId}",
                lines.Length, batchPayRollRequestId);

            foreach (var line in lines)
            {
                // Start a new transaction for each line
                using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
                try
                {
                    var parts = line.Split(',');

                    if (parts.Length < 3)
                    {
                        failCount++;
                        errors.Add($"فرمت خط نامعتبر: {line}");
                        continue;
                    }

                    var nationalNo = parts[0].Trim();
                    var valueStr = parts[1].Trim();
                    var isMainTax = parts[2].Trim();

                    // Parse Value
                    if (!double.TryParse(valueStr, out double value))
                    {
                        failCount++;
                        errors.Add($"مقدار Value نامعتبر برای کد ملی {nationalNo}: {valueStr}");
                        continue;
                    }

                    // Find Employee by NationalNo
                    var employee = _employeeService.All(false)
                        .FirstOrDefault(e => e.NationalNo == nationalNo);

                    if (employee == null)
                    {
                        failCount++;
                        errors.Add($"کارمندی با کد ملی {nationalNo} یافت نشد");
                        continue;
                    }

                    // Find BatchPayRollRequestDetail
                    var detail = _unitOfWork.Context.BatchPayRollRequestDetails
                        .FirstOrDefault(d => d.BatchPayRollRequestId == batchPayRollRequestId
                                          && d.EmployeeId == employee.Id);

                    if (detail == null)
                    {
                        failCount++;
                        errors.Add($"جزئیات درخواست گروهی برای کارمند با کد ملی {nationalNo} یافت نشد");
                        continue;
                    }

                    // Update the detail
                    detail.Value = value;
                    detail.ISMainTax = isMainTax;

                    // Append to FinalMessage
                    var newMessage = $"مالیات محاسبه شده: {value:N0} - نوع: {isMainTax} - تاریخ: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                    if (string.IsNullOrWhiteSpace(detail.FinalMessage))
                    {
                        detail.FinalMessage = newMessage;
                    }
                    else
                    {
                        detail.FinalMessage += Environment.NewLine + newMessage;
                    }

                    _unitOfWork.Context.BatchPayRollRequestDetails.Update(detail);
                    await _unitOfWork.Context.SaveChangesAsync();

                    #region HandleTaxItem

                    if (detail.FicheId > 0)
                    {
                        var fiche = _unitOfWork.Context.Fiches.Find(detail.FicheId);

                        if (fiche != null)
                        {
                            var setting = _organisationEmployeeTypeFicheItemService.GetCurrentOrganItemsByEmployeeType(fiche.OrganisationChartId, fiche.EmployeeTypeId);
                            bool needToInsert = false;
                            if (setting != null)
                            {
                                if (setting.Count == 0)
                                {
                                    needToInsert = true;
                                }
                                else
                                {
                                    if (setting.Any(i => i.IsMainTaxItem))
                                    {
                                        var mainItem = setting.Single(i => i.IsMainTaxItem);

                                        var relatedFicheItem = _unitOfWork.Context.FicheItems.Where(i => i.FicheId == detail.FicheId && i.WageItemId == mainItem.WageItemId);

                                        if (relatedFicheItem != null)
                                        {
                                            if (relatedFicheItem.Any())
                                            {
                                                var currentFicheTaxItem = relatedFicheItem.Single();

                                                fiche.PurePaymentAmount += Convert.ToInt64(currentFicheTaxItem.Value);
                                                fiche.DeductedAmount -= Convert.ToInt64(currentFicheTaxItem.Value);

                                                currentFicheTaxItem.Value = detail.Value;

                                                fiche.PurePaymentAmount -= Convert.ToInt64(currentFicheTaxItem.Value);
                                                fiche.DeductedAmount += Convert.ToInt64(currentFicheTaxItem.Value);

                                                _unitOfWork.Context.Fiches.Update(fiche);
                                                _unitOfWork.Context.FicheItems.Update(currentFicheTaxItem);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        needToInsert = true;
                                    }
                                }
                            }
                            if (needToInsert)
                            {
                                if (setting != null)
                                {
                                    if (setting.Count == 0)
                                    {

                                    }
                                    else
                                    {
                                        if (setting.Any(i => i.IsMainTaxItem))
                                        {
                                            var mainItem = setting.Single(i => i.IsMainTaxItem);
                                            fiche.PurePaymentAmount -= Convert.ToInt64(detail.Value);
                                            fiche.DeductedAmount += Convert.ToInt64(detail.Value);
                                            _unitOfWork.Context.FicheItems.Add(new FicheItem()
                                            {
                                                FicheId = detail.FicheId.Value,
                                                WageItemId = mainItem.WageItemId,
                                                Value = detail.Value,
                                                Comment = "مبلغ توسط اداره مالیات حساب شده است",
                                                CreateDate = DateTime.Now,
                                                title = "From Tax File",
                                                IPAddress = "",
                                                PaymentTypeId = (long)Enums.PaymentType.Deduction
                                            });

                                            _unitOfWork.Context.Fiches.Update(fiche);
                                        }
                                    }
                                }
                            }
                        }
                    }


                    #endregion HandleTaxItem
                    await _unitOfWork.Context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    successCount++;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    failCount++;
                    errors.Add($"خطا در پردازش خط: {line} - {ex.Message}");
                    _logger.LogError(ex, "خطا در پردازش خط فایل مالیات: {Line}", line);
                }
            }

            _logger.LogInformation("پایان پردازش فایل TaxResponse. موفق: {SuccessCount}, ناموفق: {FailCount}",
                successCount, failCount);

            return (successCount, failCount, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای کلی در پردازش فایل TaxResponse برای Batch");
            errors.Add($"خطای کلی: {ex.Message}");
            return (successCount, failCount, errors);
        }
    }

    /// <summary>
    /// بررسی وجود فایل پاسخ محاسبه مالیات
    /// </summary>
    public OperationResult CheckTaxResponseFileExists(long batchPayRollRequestId)
    {
        try
        {
            var taxDiskette = _unitOfWork.Context.TaxDiskettes
                .FirstOrDefault(t => t.BatchPayRollRequestId == batchPayRollRequestId);

            if (taxDiskette == null)
            {
                return OperationResult.Succeeded(payload: false);
            }

            var fileExists = _unitOfWork.Context.TaxDisketteFiles
                .Any(f => f.TaxDisketteId == taxDiskette.Id
                       && f.FileTypeId == (long)Enums.TaxDisketteFileType.TaxResponse);

            return OperationResult.Succeeded(payload: fileExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بررسی وجود فایل پاسخ مالیات");
            return OperationResult.Failed("خطا در بررسی وجود فایل");
        }
    }

    /// <summary>
    /// دریافت فهرست رکوردهای بدون Value برای مغایرت‌گیری نهایی
    /// </summary>
    public OperationResult GetDiscrepancyList(long batchPayRollRequestId)
    {
        try
        {
            var details = _unitOfWork.Context.BatchPayRollRequestDetails
                .Include(d => d.Employee)
                .Where(d => d.BatchPayRollRequestId == batchPayRollRequestId)
                .Select(d => new
                {
                    d.Id,
                    d.EmployeeId,
                    EmployeeName = d.Employee.FirstName + " " + d.Employee.LastName,
                    d.Employee.NationalNo,
                    d.Employee.PersonelCode,
                    d.FicheId,
                    d.Value,
                    d.ISMainTax,
                    d.FinalMessage,
                    HasValue = d.Value != 0
                })
                .ToList();

            var withoutValue = details.Where(d => d.Value == 0).ToList();
            var withValue = details.Where(d => d.Value != 0).ToList();

            var result = new
            {
                TotalCount = details.Count,
                WithValueCount = withValue.Count,
                WithoutValueCount = withoutValue.Count,
                WithoutValueList = withoutValue,
                WithValueList = withValue
            };

            return OperationResult.Succeeded(payload: result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت فهرست مغایرت‌ها");
            return OperationResult.Failed("خطا در دریافت فهرست مغایرت‌ها: " + ex.Message);
        }
    }
}
