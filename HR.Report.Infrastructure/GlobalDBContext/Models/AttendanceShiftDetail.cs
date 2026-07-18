using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Shift_Detail", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("ShiftId", Name = "IX_Attendance_Shift_Detail_ShiftId")]
public partial class AttendanceShiftDetail
{
    [Key]
    public long Id { get; set; }

    public long ShiftId { get; set; }

    /// <summary>
    /// روز هفته (مطابق DayOfWeek)
    /// </summary>
    public int WeekDay { get; set; }

    /// <summary>
    /// شیفت منعطف
    /// </summary>
    public bool IsFlexible { get; set; }

    /// <summary>
    /// ساعت شروع کار
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// ساعت پایان کار
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// شروع استراحت
    /// </summary>
    public TimeOnly? RestStart { get; set; }

    /// <summary>
    /// پایان استراحت
    /// </summary>
    public TimeOnly? RestEnd { get; set; }

    /// <summary>
    /// مدت کار مورد نیاز (ثانیه)
    /// </summary>
    public int RequiredWorkSeconds { get; set; }

    /// <summary>
    /// شیفت شب
    /// </summary>
    public bool NightShift { get; set; }

    /// <summary>
    /// عبور از نیمه‌شب
    /// </summary>
    public bool CrossDay { get; set; }

    /// <summary>
    /// حداقل زمان ورود
    /// </summary>
    public TimeOnly? MinInTime { get; set; }

    /// <summary>
    /// حداکثر زمان ورود
    /// </summary>
    public TimeOnly? MaxInTime { get; set; }

    /// <summary>
    /// حداقل زمان خروج
    /// </summary>
    public TimeOnly? MinOutTime { get; set; }

    /// <summary>
    /// حداکثر زمان خروج
    /// </summary>
    public TimeOnly? MaxOutTime { get; set; }

    /// <summary>
    /// نوع گرد کردن زمان
    /// </summary>
    public int? RoundType { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [ForeignKey("ShiftId")]
    [InverseProperty("AttendanceShiftDetails")]
    public virtual AttendanceShift Shift { get; set; } = null!;
}
