using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs;

public class BatchSettlementRequestDTO : BaseDTO
{
    public long UserId { get; set; }
    public long? OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long? RequestStateId { get; set; }
    public string? RequestState { get; set; }
    public long? RequestTypeId { get; set; }
    public string? RequestType { get; set; }
    public long SettlementCauseId { get; set; }
    public string? SettlementCause { get; set; }
    public long? PaymentPeriodId { get; set; }
    public string? PaymentPeriod { get; set; }
    public long? PayLocationId { get; set; }
    public string? PayLocation { get; set; }
    public long? CostCenterId { get; set; }
    public string? CostCenter { get; set; }
    public DateTime SettlementDate { get; set; }
    public DateTime SettlementStartDate { get; set; }
    public DateTime SettlementEndDate { get; set; }
    public int FiscalYear { get; set; }
    public bool IsYearLong { get; set; }
    public bool Loanincluded { get; set; }
    public bool Deductionincluded { get; set; }
    public bool SendToCartable { get; set; }
    public bool ProceedWithoutFiche { get; set; }
    [StringLength(256)]
    public string? Username { get; set; }
    [StringLength(4096)]
    public string? RequsetDescription { get; set; }
    public DateTime? LastPoolingTime { get; set; }
    public DateTime? FinishDateTime { get; set; }
    public bool? IsDone { get; set; }
    public int? EmployeeCount { get; set; }
    public int? SuccessCount { get; set; }
    public long? PoolingEmployeeId { get; set; }
    public string? ExeptionMessage { get; set; }
    public List<long>? EmployeeIds { get; set; }
}
