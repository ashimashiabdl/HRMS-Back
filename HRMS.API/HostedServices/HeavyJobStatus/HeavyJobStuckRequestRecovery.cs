using HR.Order.Core.Data;
using HR.Order.Infrastructure.Data;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.HostedServices.HeavyJobStatus;

/// <summary>
/// بازیابی درخواست‌های گروهی گیرکرده در وضعیت Running تا پس از قطع/کندی بتوانند ادامه یابند.
/// </summary>
public sealed class HeavyJobStuckRequestRecovery(
    IUnitOfWork<PayrollContext> payrollUnitOfWork,
    IUnitOfWork<OrderContext> orderUnitOfWork,
    IConfiguration configuration,
    ILogger<HeavyJobStuckRequestRecovery> logger)
{
    private readonly IUnitOfWork<PayrollContext> _payrollUnitOfWork = payrollUnitOfWork;
    private readonly IUnitOfWork<OrderContext> _orderUnitOfWork = orderUnitOfWork;
    private readonly ILogger<HeavyJobStuckRequestRecovery> _logger = logger;
    private readonly int _timeoutMinutes = Math.Max(
        1,
        configuration.GetValue<int?>("HeavyJobWorker:StuckTimeoutMinutes") ?? 30);

    public HeavyJobRecoveryResult RecoverStuckRequests()
    {
        var result = new HeavyJobRecoveryResult();
        var cutoff = DateTime.Now.AddMinutes(-_timeoutMinutes);
        var now = DateTime.Now;

        try
        {
            var payrollMsg =
                $"پردازش قبلی بیش از {_timeoutMinutes} دقیقه پاسخ نداد (بازیابی خودکار Worker). درخواست برای «تلاش مجدد» آماده شد.";

            var payrollTypes = new long[]
            {
                (long)Enums.BatchPayRollRequestType.NormalFicheCalculation,
                (long)Enums.BatchPayRollRequestType.ArearsFicheCalculation,
                (long)Enums.BatchPayRollRequestType.BankDisketteCalculation,
                (long)Enums.BatchPayRollRequestType.InsuranceDisketteCalculation,
                (long)Enums.BatchPayRollRequestType.TaxDisketteCalculation,
            };

            result.PayrollBatchRecovered = _payrollUnitOfWork.Context.BatchPayRollRequests
                .Where(r => payrollTypes.Contains(r.RequestTypeId)
                            && r.RequestStateId == (long)Enums.BatchPayRollRequestState.Running
                            && r.LastPoolingTime.HasValue
                            && r.LastPoolingTime.Value < cutoff)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.RequestStateId, (long)Enums.BatchPayRollRequestState.TryAgain)
                    .SetProperty(r => r.ExeptionMessage, payrollMsg)
                    .SetProperty(r => r.LastModifiedDate, now)
                    .SetProperty(r => r.LastPoolingTime, now));
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            _logger.LogError(ex, "خطا در بازیابی درخواست‌های Payroll گیرکرده");
        }

        try
        {
            // احکام گروهی: Runing → Initial تا در چرخه بعد دوباره برداشته شوند
            result.OrderBatchRecovered = _orderUnitOfWork.Context.BatchRequests
                .Where(r => r.RequestStateId == (long)Enums.BatchRequestState.Runing
                            && r.LastPoolingTime.HasValue
                            && r.LastPoolingTime.Value < cutoff)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.RequestStateId, (long)Enums.BatchRequestState.Initial)
                    .SetProperty(r => r.LastModifiedDate, now)
                    .SetProperty(r => r.LastPoolingTime, now));
        }
        catch (Exception ex)
        {
            result.Error = string.IsNullOrEmpty(result.Error)
                ? ex.Message
                : result.Error + " | " + ex.Message;
            _logger.LogError(ex, "خطا در بازیابی درخواست‌های Order گیرکرده");
        }

        if (result.PayrollBatchRecovered > 0 || result.OrderBatchRecovered > 0)
        {
            _logger.LogWarning(
                "بازیابی درخواست‌های گیرکرده: Payroll={Payroll}, Order={Order}, Timeout={Timeout}m",
                result.PayrollBatchRecovered, result.OrderBatchRecovered, _timeoutMinutes);
        }
        else if (string.IsNullOrEmpty(result.Error))
        {
            _logger.LogInformation(
                "درخواست گیرکرده‌ای برای بازیابی یافت نشد (Timeout={Timeout}m)",
                _timeoutMinutes);
        }

        return result;
    }
}

public sealed class HeavyJobRecoveryResult
{
    public int PayrollBatchRecovered { get; set; }
    public int OrderBatchRecovered { get; set; }
    public string? Error { get; set; }

    public int Total => PayrollBatchRecovered + OrderBatchRecovered;
}
