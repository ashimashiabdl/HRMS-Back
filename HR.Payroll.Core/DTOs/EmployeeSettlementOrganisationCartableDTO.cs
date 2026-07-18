using System;

namespace HR.Payroll.Core.DTOs;

/// <summary>
/// ردیف کارتابل سازمانی تسویه حساب (بدون وابستگی به EmployeeId در URL).
/// </summary>
public class EmployeeSettlementOrganisationCartableDTO
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalNo { get; set; }
    public long? EmployeeTypeId { get; set; }
    public string? EmployeeTypeTitle { get; set; }
    public long? CostCenterId { get; set; }
    public string? CostCenterTitle { get; set; }
    public long? SettlementStatusId { get; set; }
    public string? SettlementStatusTitle { get; set; }
    public string? SettlementCauseTitle { get; set; }
    public DateTime SettlementDate { get; set; }
    public int FiscalYear { get; set; }
    public long PaymentAmount { get; set; }
    public long PurePaymentAmount { get; set; }
    public long DeductionSum { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
