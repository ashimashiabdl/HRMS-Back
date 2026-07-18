namespace HR.Payroll.Core.DTOs;

public class BatchSettlementCandidateDTO
{
    public int Id { get; set; }
    public long EmployeeId { get; set; }
    public long InterdictOrderId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalNo { get; set; }
    public string? PayLocation { get; set; }
    public string? CostCenter { get; set; }
    public string? EmployeeStatus { get; set; }
    public string? OrderType { get; set; }
    public DateTime? OrderStartDate { get; set; }
    public DateTime? OrderEndDate { get; set; }
    public bool IsEligible { get; set; }
    public string? EligibilityMessage { get; set; }
}
