using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class BatchSettlementRequestDetailDTO : BaseDTO
{
    public long EmployeeId { get; set; }
    public string? ActiveName { get; set; }
    public string? NationalNo { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonelCode { get; set; }
    public long BatchSettlementRequestId { get; set; }
    public string? BatchSettlementRequest { get; set; }
    public long InterdictOrderId { get; set; }
    public long? EmployeeSettlementId { get; set; }
    public string? FinalMessage { get; set; }
    public DateTime? DoDatetime { get; set; }
    public DateTime? LastTryDateTime { get; set; }
    public double RunTimeinMilliseconds { get; set; }
}
