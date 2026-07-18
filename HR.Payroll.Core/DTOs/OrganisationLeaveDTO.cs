using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class OrganisationLeaveDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }

    public long LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }
}


