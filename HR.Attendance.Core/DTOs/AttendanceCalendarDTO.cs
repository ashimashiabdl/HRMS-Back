using HR.SharedKernel.Data;

namespace HR.Attendance.Core.DTOs;

public class AttendanceCalendarDTO : BaseDTO
{
    public DateTime Date { get; set; }
    public bool IsHoliday { get; set; }
    public long? HolidayId { get; set; }
    public string? Holiday { get; set; }
    public int WeekDay { get; set; }
    public string? WeekDayTitle { get; set; }
}
