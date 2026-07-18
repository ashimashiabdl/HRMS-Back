using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class EmployeeShiftAssignmentDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long EmployeeId { get; set; }
    public string? Employee { get; set; }

    public long ShiftId { get; set; }
    public string? Shift { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }
}
