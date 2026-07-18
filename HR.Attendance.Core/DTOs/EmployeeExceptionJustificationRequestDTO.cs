using HR.SharedKernel.Data;

namespace HR.Attendance.Core.DTOs;

public class EmployeeExceptionJustificationRequestDTO : BaseDTO
{
    public long EmployeeAttendanceExceptionId { get; set; }
    public string? EmployeeAttendanceException { get; set; }

    public long AbsenceTypeId { get; set; }
    public string? AbsenceType { get; set; }

    public long? LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }

    public long EmployeeExceptionJustificationRequestStateId { get; set; }
    public string? EmployeeExceptionJustificationRequestState { get; set; }

    public string? Description { get; set; }

    public long? EmployeeId { get; set; }
    public string? Employee { get; set; }
}
