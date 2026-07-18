namespace HR.Payroll.Core.DTOs;

public class GetEmployeeSettlementEligibility_Result
{
    public bool HasFinalInterdict { get; set; }

    public bool IsInCurrentOrgan { get; set; }

    public bool? IsEmployed { get; set; }

    /// <summary>
    /// از تنظیمات Setting.Organisation_EmployeeType_OrderType برای نوع حکم جاری
    /// </summary>
    public bool? NeedSettlement { get; set; }

    public bool IsEligibleForSettlement { get; set; }

    public long? InterdictOrderId { get; set; }

    public DateTime? InterdictEndDate { get; set; }

    public long? EmployeeStatusId { get; set; }

    public string? EmployeeStatusTitle { get; set; }

    public long? PayLocationId { get; set; }
}
