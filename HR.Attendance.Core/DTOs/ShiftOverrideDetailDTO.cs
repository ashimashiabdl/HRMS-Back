using HR.Attendance.Core.Enums;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class ShiftOverrideDetailDTO : BaseDTO
{
    public long ShiftOverrideId { get; set; }
    public int WeekDay { get; set; }
    public string? WeekDayTitle { get; set; }

    public bool IsHoliday { get; set; }
    public bool IsFlexible { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan? RestStart { get; set; }
    public TimeSpan? RestEnd { get; set; }
    public int RequiredWorkSeconds { get; set; }
    public bool NightShift { get; set; }
    public bool CrossDay { get; set; }
    public TimeSpan? MinInTime { get; set; }
    public TimeSpan? MaxInTime { get; set; }
    public TimeSpan? MinOutTime { get; set; }
    public TimeSpan? MaxOutTime { get; set; }
    public ShiftRoundType? RoundType { get; set; }
    public string? RoundTypeTitle { get; set; }
}
