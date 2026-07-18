using Hr.SystemSetting.Infrastructure.Services;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HR.Payroll.Infrastructure.Services;

/// <summary>
/// پردازش پس‌زمینه درخواست‌های تسویه حساب گروهی — از سرویس تسویه موردی استفاده می‌کند.
/// </summary>
public class BatchSettlementProcessorService(
    IUnitOfWork<PayrollContext> unitOfWork,
    IConfiguration configuration,
    ILogger<BatchSettlementProcessorService> logger,
    IServiceScopeFactory serviceScopeFactory) : IScopedServices
{
    private const int PerEmployeeTimeoutSeconds = 180;
    private readonly IUnitOfWork<PayrollContext> _unitOfWork = unitOfWork;
    private readonly string _connectionString = configuration.GetConnectionString("HRMSConnection") ?? string.Empty;
    private readonly ILogger<BatchSettlementProcessorService> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public void ProcessBatchSettlements()
    {
        _logger.LogInformation("شروع ProcessBatchSettlements — تسویه حساب گروهی");

        _unitOfWork.Context.ChangeTracker.Clear();
        var originalAutoDetect = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
        var originalTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;
        _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
        _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        try
        {
            var validStates = new List<long>
            {
                (long)Enums.BatchSettlementRequestState.Initial,
                (long)Enums.BatchSettlementRequestState.TryAgain,
            };

            var validRequests = _unitOfWork.Context.BatchSettlementRequests
                .Where(r => validStates.Contains(r.RequestStateId)
                    && r.RequestTypeId == (long)Enums.BatchSettlementRequestType.NormalSettlement
                    && !r.IsDeleted)
                .ToList();

            _logger.LogInformation("تعداد {Count} درخواست تسویه گروهی در صف پردازش", validRequests.Count);

            foreach (var request in validRequests)
            {
                ProcessSingleRequest(request);
            }
        }
        finally
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetect;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = originalTracking;
            _logger.LogInformation("پایان ProcessBatchSettlements");
        }
    }

    private void ProcessSingleRequest(BatchSettlementRequest request)
    {
        _logger.LogInformation("شروع پردازش درخواست تسویه گروهی {RequestId} — {EmployeeCount} کارمند",
            request.Id, request.EmployeeCount);

        try
        {
            request.RequestStateId = (long)Enums.BatchSettlementRequestState.Running;
            request.LastPoolingTime = DateTime.Now;
            _unitOfWork.Context.Update(request);
            _unitOfWork.Context.SaveChanges();

            var details = _unitOfWork.Context.BatchSettlementRequestDetails
                .Where(d => d.BatchSettlementRequestId == request.Id && !d.IsDeleted)
                .ToList();

            if (details.Count == 0)
            {
                request.ExeptionMessage = "هیچ ردیف جزئیاتی برای این درخواست یافت نشد";
                _logger.LogWarning("درخواست {RequestId} بدون ردیف جزئیات", request.Id);
            }

            foreach (var detail in details)
            {
                var currentState = _unitOfWork.Context.BatchSettlementRequests
                    .AsNoTracking()
                    .Where(r => r.Id == request.Id)
                    .Select(r => r.RequestStateId)
                    .FirstOrDefault();
                if (currentState == (long)Enums.BatchSettlementRequestState.CancelByUser)
                {
                    _logger.LogInformation("درخواست {RequestId} توسط کاربر متوقف شد", request.Id);
                    break;
                }

                ProcessDetailRow(request, detail);
            }

            request.RequestStateId = (long)Enums.BatchSettlementRequestState.EndLoop;
            request.FinishDateTime = DateTime.Now;
            request.IsDone = true;
            _unitOfWork.Context.Update(request);
            _unitOfWork.Context.SaveChanges();

            _logger.LogInformation(
                "پایان درخواست {RequestId} — موفق: {Success}/{Total}",
                request.Id, request.SuccessCount, request.EmployeeCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای کلی در پردازش درخواست تسویه گروهی {RequestId}", request.Id);
            try
            {
                request.RequestStateId = (long)Enums.BatchSettlementRequestState.EndLoop;
                request.FinishDateTime = DateTime.Now;
                request.ExeptionMessage = $"خطای کلی پردازش: {ex.Message}";
                _unitOfWork.Context.Update(request);
                _unitOfWork.Context.SaveChanges();
            }
            catch (Exception saveEx)
            {
                _logger.LogError(saveEx, "خطا در ذخیره وضعیت نهایی درخواست {RequestId}", request.Id);
            }
        }
    }

    private void ProcessDetailRow(BatchSettlementRequest request, BatchSettlementRequestDetail detail)
    {
        SafeLogDetailProgress(detail.Id, "شروع پردازش ردیف — آماده‌سازی تسویه");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            request.PoolingEmployeeId = detail.EmployeeId;
            _unitOfWork.Context.Update(request);

            detail.LastTryDateTime = DateTime.Now;
            detail.FinalMessage = "در حال پردازش تسویه حساب ...";
            _unitOfWork.Context.Update(detail);
            _unitOfWork.Context.SaveChanges();

            using var scope = _serviceScopeFactory.CreateScope();
            var settlementService = scope.ServiceProvider.GetRequiredService<EmployeeSettlementService>();
            var settlementItemService = scope.ServiceProvider.GetRequiredService<OrganisationEmployeeTypeSettlementItemService>();

            settlementService._currentUserDefaultOrganId = request.OrganisationChartId;
            settlementService._currentUserDefaultPaymentPeriod = request.PaymentPeriodId ?? 0;
            settlementItemService._currentUserDefaultOrganId = request.OrganisationChartId;

            SafeLogDetailProgress(detail.Id, "بررسی صلاحیت کارمند برای تسویه ...");
            var eligibilityResult = settlementService.GetSettlementEligibilityByEmployeeId(detail.EmployeeId);
            EmployeeSettlementEligibilityDTO? eligibility = eligibilityResult.Payload as EmployeeSettlementEligibilityDTO;
            if (!eligibilityResult.Success || eligibility == null || !eligibility.IsEligibleForSettlement)
            {
                var msg = eligibility?.Message ?? eligibilityResult.Message ?? "کارمند واجد شرایط تسویه نیست";
                FinalizeDetail(detail, request, msg, null, stopwatch);
                return;
            }

            SafeLogDetailProgress(detail.Id, "آماده‌سازی داده تسویه و محاسبه پیش‌نمایش ...");
            var settlementDto = BuildSettlementDto(request, detail, settlementItemService, eligibility);
            if (settlementDto == null)
            {
                FinalizeDetail(detail, request, "خطا در ساخت داده تسویه — آیتم‌های تسویه یافت نشد", null, stopwatch);
                return;
            }

            var previewResult = settlementService.PreviewSettlementCalculation(settlementDto, buildTreeTrace: false);
            if (!previewResult.Success)
            {
                var previewMsg = BuildUserFriendlyError(previewResult, detail.EmployeeId);
                FinalizeDetail(detail, request, previewMsg, null, stopwatch);
                return;
            }

            if (previewResult.Payload is EmployeeSettlementCalculationResultDTO calcResult)
            {
                if (calcResult.HasCalculationErrors)
                {
                    var calcMsg = string.Join(" | ", calcResult.ErrorMessages ?? []);
                    FinalizeDetail(detail, request, $"خطای محاسبه: {calcMsg}", null, stopwatch);
                    return;
                }

                if (calcResult.Settlement != null)
                {
                    settlementDto = calcResult.Settlement;
                    settlementDto.ProceedWithoutFiche = request.ProceedWithoutFiche;
                }
            }

            SafeLogDetailProgress(detail.Id, "ذخیره تسویه حساب ...");
            var createTask = settlementService.CreateForAsync(settlementDto);
            if (!createTask.Wait(TimeSpan.FromSeconds(PerEmployeeTimeoutSeconds)))
            {
                FinalizeDetail(detail, request,
                    $"ثبت تسویه در مهلت {PerEmployeeTimeoutSeconds} ثانیه به پایان نرسید (Timeout)",
                    null, stopwatch);
                return;
            }

            var createResult = createTask.Result;
            if (!createResult.Success)
            {
                FinalizeDetail(detail, request, BuildUserFriendlyError(createResult, detail.EmployeeId), null, stopwatch);
                return;
            }

            var settlementId = Convert.ToInt64(createResult.Payload);
            detail.EmployeeSettlementId = settlementId;

            if (request.SendToCartable)
            {
                SafeLogDetailProgress(detail.Id, "ارسال به گردش کار ...");
                var cartableResult = settlementService.SendSettlementToCartable(settlementId);
                if (!cartableResult.Success)
                {
                    FinalizeDetail(detail, request,
                        $"تسویه ثبت شد (شناسه {settlementId}) ولی ارسال به گردش کار ناموفق: {cartableResult.Message}",
                        settlementId, stopwatch);
                    return;
                }
            }

            request.SuccessCount += 1;
            _unitOfWork.Context.Update(request);
            FinalizeDetail(detail, request, "ثبت موفق تسویه حساب", settlementId, stopwatch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "خطا در پردازش ردیف {DetailId} — کارمند {EmployeeId} — درخواست {RequestId}",
                detail.Id, detail.EmployeeId, request.Id);

            var errorMessage = "خطا: " + ex.Message;
            if (ex.InnerException != null)
            {
                errorMessage += " | علت داخلی: " + ex.InnerException.Message;
            }

            FinalizeDetail(detail, request, errorMessage, detail.EmployeeSettlementId, stopwatch);
        }
    }

    private EmployeeSettlementDTO? BuildSettlementDto(
        BatchSettlementRequest request,
        BatchSettlementRequestDetail detail,
        OrganisationEmployeeTypeSettlementItemService settlementItemService,
        EmployeeSettlementEligibilityDTO eligibility)
    {
        var interdictOrder = _unitOfWork.Context.InterdictOrders
            .AsNoTracking()
            .Include(i => i.RecruitOrder)
            .FirstOrDefault(i => i.Id == detail.InterdictOrderId);

        if (interdictOrder?.RecruitOrder == null)
        {
            return null;
        }

        var employeeTypeId = interdictOrder.RecruitOrder.EmployeeTypeId;
        var allItemIds = settlementItemService.All()
            .AsNoTracking()
            .Where(i => i.OrganisationChartId == request.OrganisationChartId
                && i.EmployeeTypeId == employeeTypeId)
            .Select(i => i.SettlementItemId)
            .Distinct()
            .ToList();

        if (allItemIds.Count == 0)
        {
            return null;
        }

        var loanIds = eligibility.LoanSettlementItemIds.ToHashSet();
        var deductionIds = eligibility.DeductionSettlementItemIds.ToHashSet();
        var selectedIds = allItemIds.Where(id =>
        {
            if (!request.Loanincluded && loanIds.Contains(id))
            {
                return false;
            }

            if (!request.Deductionincluded && deductionIds.Contains(id))
            {
                return false;
            }

            return true;
        }).ToList();

        if (selectedIds.Count == 0)
        {
            return null;
        }

        return new EmployeeSettlementDTO
        {
            OrganisationChartId = request.OrganisationChartId,
            EmployeeId = detail.EmployeeId,
            SettlementCauseId = request.SettlementCauseId,
            SettlementDate = request.SettlementDate,
            StartDate = request.SettlementStartDate,
            EndDate = request.SettlementEndDate,
            FiscalYear = request.FiscalYear,
            IsYearLong = request.IsYearLong,
            Loanincluded = request.Loanincluded,
            Deductionincluded = request.Deductionincluded,
            ProceedWithoutFiche = request.ProceedWithoutFiche,
            Description = request.RequsetDescription,
            InterdictOrderId = detail.InterdictOrderId,
            LastInterdictOrderId = detail.InterdictOrderId,
            SettlementItemIds = selectedIds,
        };
    }

    private static string BuildUserFriendlyError(OperationResult result, long employeeId)
    {
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            return $"کارمند {employeeId}: {result.Message}";
        }

        return $"کارمند {employeeId}: عملیات ناموفق بود (بدون پیام مشخص)";
    }

    private void FinalizeDetail(
        BatchSettlementRequestDetail detail,
        BatchSettlementRequest request,
        string message,
        long? settlementId,
        System.Diagnostics.Stopwatch stopwatch)
    {
        stopwatch.Stop();
        detail.FinalMessage = message;
        detail.RunTimeinMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
        detail.DoDatetime = settlementId.HasValue ? DateTime.Now : detail.DoDatetime;
        if (settlementId.HasValue)
        {
            detail.EmployeeSettlementId = settlementId;
        }

        TrySaveDetail(detail);
        SafeLogDetailProgress(detail.Id, message, markEnd: true);
    }

    private void TrySaveDetail(BatchSettlementRequestDetail detail)
    {
        try
        {
            _unitOfWork.Context.Update(detail);
            _unitOfWork.Context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ذخیره EF ردیف {DetailId} ناموفق — تلاش با SQL مستقیم", detail.Id);
            SafeLogDetailProgress(detail.Id, detail.FinalMessage ?? "خطای ذخیره", markEnd: true);
        }
    }

    private void SafeLogDetailProgress(long detailId, string message, bool markEnd = false)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            return;
        }

        try
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var cmd = new SqlCommand(@"
UPDATE Payroll.Batch_Settlement_Request_Detail
SET FinalMessage = @Msg,
    LastTryDateTime = GETDATE(),
    LastModifiedDate = GETDATE()
WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Msg", message ?? string.Empty);
            cmd.Parameters.AddWithValue("@Id", detailId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SafeLogDetailProgress برای ردیف {DetailId} ناموفق", detailId);
        }
    }
}
