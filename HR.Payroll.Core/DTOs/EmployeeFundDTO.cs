using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs;

public class EmployeeFundDTO : BaseDTO
{
    public long EmployeeId { get; set; }
    public string? Employee { get; set; }

    public long FundTypeId { get; set; }
    public string? FundType { get; set; }

    public long StartDeductPaymentPeriodId { get; set; }
    public string? StartDeductPaymentPeriod { get; set; }

    public bool IsActive { get; set; }
}

