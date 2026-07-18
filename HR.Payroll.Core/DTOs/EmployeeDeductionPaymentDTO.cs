using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class EmployeeDeductionPaymentDTO : BaseDTO
{
    public long FicheId { get; set; }
    public string? Fiche { get; set; }
    public long EmployeeDeductionId { get; set; }
    public string? EmployeeDeduction { get; set; }
    public bool IsPaid { get; set; }
    public long PaymentAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public long PaymentTypeId { get; set; }
    public string? PaymentType { get; set; }
}
