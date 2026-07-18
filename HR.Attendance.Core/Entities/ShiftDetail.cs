using HR.Attendance.Core.Enums;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

[Table("Attendance_Shift_Detail", Schema = "Attendance")]
public class ShiftDetail : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("Shift")]
    public long ShiftId { get; set; }
    public virtual Shift? Shift { get; set; }

    [Comment("روز هفته (مطابق DayOfWeek)")]
    public int WeekDay { get; set; }

    [Comment("روز تعطیل")]
    public bool IsHoliday { get; set; }

    [Comment("شیفت منعطف")]
    public bool IsFlexible { get; set; }

    [Comment("ساعت شروع کار")]
    public TimeSpan StartTime { get; set; }

    [Comment("ساعت پایان کار")]
    public TimeSpan EndTime { get; set; }

    [Comment("شروع استراحت")]
    public TimeSpan? RestStart { get; set; }

    [Comment("پایان استراحت")]
    public TimeSpan? RestEnd { get; set; }

    [Comment("مدت کار مورد نیاز (ثانیه)")]
    public int RequiredWorkSeconds { get; set; }

    [Comment("شیفت شب")]
    public bool NightShift { get; set; }

    [Comment("عبور از نیمه‌شب")]
    public bool CrossDay { get; set; }

    [Comment("حداقل زمان ورود")]
    public TimeSpan? MinInTime { get; set; }

    [Comment("حداکثر زمان ورود")]
    public TimeSpan? MaxInTime { get; set; }

    [Comment("حداقل زمان خروج")]
    public TimeSpan? MinOutTime { get; set; }

    [Comment("حداکثر زمان خروج")]
    public TimeSpan? MaxOutTime { get; set; }

    [Comment("نوع گرد کردن زمان")]
    public ShiftRoundType? RoundType { get; set; }
}
