using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class EmployeeAttendanceDailyResultDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long EmployeeId { get; set; }
    public string? Employee { get; set; }

    public long AttendanceCalendarId { get; set; }
    public string? AttendanceCalendar { get; set; }

    public long ShiftId { get; set; }
    public string? Shift { get; set; }

    public DateTime? FirstIn { get; set; }
    public DateTime? SecondIn { get; set; }
    public DateTime? ThirdIn { get; set; }
    public DateTime? FourthIn { get; set; }
    public DateTime? FifthIn { get; set; }
    public DateTime? SixthIn { get; set; }
    public DateTime? SeventhIn { get; set; }
    public DateTime? SecondOut { get; set; }
    public DateTime? ThirdOut { get; set; }
    public DateTime? FourthOut { get; set; }
    public DateTime? FifthOut { get; set; }
    public DateTime? SixthOut { get; set; }
    public DateTime? SeventhOut { get; set; }
    public DateTime? LastOut { get; set; }

    public int WorkedSeconds { get; set; }
    public int RequiredSeconds { get; set; }
    public int DelaySeconds { get; set; }
    public int EarlyLeaveSeconds { get; set; }
    public int AbsentSeconds { get; set; }
    public int OvertimeSeconds { get; set; }
    public int NightWorkSeconds { get; set; }
    public int HolidayWorkSeconds { get; set; }
    public int MissionSeconds { get; set; }
    public int LeaveSeconds { get; set; }
    public int BreakSeconds { get; set; }
    public int PaidBreakSeconds { get; set; }
    public int UnpaidBreakSeconds { get; set; }
    public int CalculationVersion { get; set; }
    public DateTime? CalculateDate { get; set; }
}
