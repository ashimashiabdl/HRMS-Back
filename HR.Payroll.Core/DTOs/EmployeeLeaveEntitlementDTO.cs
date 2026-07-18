using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class EmployeeLeaveEntitlementDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long EmployeeId { get; set; }
    public string? Employee { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonelCode { get; set; }
    public string? NationalNo { get; set; }
    public long LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }
    public int Year { get; set; }
    public decimal LeaveAmount { get; set; }
}
