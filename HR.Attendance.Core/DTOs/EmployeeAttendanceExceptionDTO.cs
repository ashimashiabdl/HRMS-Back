using HR.SharedKernel.Data;

namespace HR.Attendance.Core.DTOs;

public class EmployeeAttendanceExceptionDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long EmployeeId { get; set; }
    public string? Employee { get; set; }

    public long AttendanceCalendarId { get; set; }
    public string? AttendanceCalendar { get; set; }

    public long AbsenceTypeId { get; set; }
    public string? AbsenceType { get; set; }

    public long ShiftId { get; set; }
    public string? Shift { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int DurationSeconds { get; set; }
    public int CalculationVersion { get; set; }
}
