using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class PaymentPeriodEmployeeBonusDTO : BaseDTO
{
    public long PaymentPeriodId { get; set; }
    public string? PaymentPeriod { get; set; }
    
    public long EmployeeId { get; set; }
    
    public string? Employee { get; set; }
    public long CoefficientId { get; set; }
    public string? Coefficient { get; set; }
    public double Value { get; set; }
}
