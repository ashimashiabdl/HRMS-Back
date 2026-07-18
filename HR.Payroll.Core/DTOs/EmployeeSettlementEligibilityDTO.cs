namespace HR.Payroll.Core.DTOs;

public class EmployeeSettlementEligibilityDTO
{
    public bool HasFinalInterdict { get; set; }

    public bool IsInCurrentOrgan { get; set; }

    public bool? IsEmployed { get; set; }

    /// <summary>
    /// از تنظیمات نوع استخدام/نوع حکم سازمان — فقط وقتی True باشد صلاحیت تسویه برقرار است
    /// </summary>
    public bool? NeedSettlement { get; set; }

    public bool IsEligibleForSettlement =>
        HasFinalInterdict && IsInCurrentOrgan && IsEmployed == false && NeedSettlement == true;

    public long? InterdictOrderId { get; set; }

    public DateTime? InterdictEndDate { get; set; }

    public string? InterdictEndDateShamsi { get; set; }

    /// <summary>
    /// تاریخ آغاز پیشنهادی آخرین بازه شاغل بودن (از کوچک‌ترین حکم مجاز غیر FinalOrder)
    /// </summary>
    public DateTime? SuggestedStartDate { get; set; }

    public string? SuggestedStartDateShamsi { get; set; }

    /// <summary>
    /// تاریخ پایان پیشنهادی آخرین بازه شاغل بودن (از بزرگ‌ترین حکم با IsEmployed=true)
    /// </summary>
    public DateTime? SuggestedEndDate { get; set; }

    public string? SuggestedEndDateShamsi { get; set; }

    public long? EmployeeStatusId { get; set; }

    public string? EmployeeStatusTitle { get; set; }

    public string? Message { get; set; }

    /// <summary>
    /// شناسه آیتم‌های تسویه مرتبط با وام‌های فعال کارمند
    /// </summary>
    public List<long> LoanSettlementItemIds { get; set; } = [];

    /// <summary>
    /// شناسه آیتم‌های تسویه مرتبط با کسورات فعال کارمند
    /// </summary>
    public List<long> DeductionSettlementItemIds { get; set; } = [];
}
