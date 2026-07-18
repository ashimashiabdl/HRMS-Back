using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Calendar", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("HolidayId", Name = "IX_Attendance_Calendar_HolidayId")]
public partial class AttendanceCalendar
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// تاریخ
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    /// <summary>
    /// تعطیل
    /// </summary>
    public bool IsHoliday { get; set; }

    /// <summary>
    /// تعطیلات مرتبط
    /// </summary>
    public long? HolidayId { get; set; }

    /// <summary>
    /// روز هفته (مطابق DayOfWeek)
    /// </summary>
    public int WeekDay { get; set; }

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

    [ForeignKey("HolidayId")]
    [InverseProperty("AttendanceCalendars")]
    public virtual AttendanceHoliday? Holiday { get; set; }
}
