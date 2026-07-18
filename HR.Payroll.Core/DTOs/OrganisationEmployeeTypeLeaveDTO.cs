using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class OrganisationEmployeeTypeLeaveDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }

    public long LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }

    public long EmployeeTypeId { get; set; }
    public string? EmployeeType { get; set; }

    public bool IsPaid { get; set; }
    public decimal? DefaultAnnualQuota { get; set; }
    public decimal? AnnualQuotaDays { get; set; }
    public decimal? CarryForwardLimit { get; set; }
    public bool Encashable { get; set; }
    public bool IsActive { get; set; }
    public bool IsDailyOrHourMinute { get; set; } 

}


