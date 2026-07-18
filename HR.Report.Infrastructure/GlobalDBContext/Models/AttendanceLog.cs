using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Log", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("AttendanceDeviceId", Name = "IX_Attendance_Log_AttendanceDeviceId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Attendance_Log_EmployeeId")]
public partial class AttendanceLog
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long AttendanceDeviceId { get; set; }

    /// <summary>
    /// شناسه کاربر در دستگاه
    /// </summary>
    [StringLength(128)]
    public string? DeviceUserId { get; set; }

    /// <summary>
    /// زمان ثبت در دستگاه
    /// </summary>
    public DateTime LogDateTime { get; set; }

    /// <summary>
    /// جهت تردد
    /// </summary>
    [StringLength(32)]
    public string? Direction { get; set; }

    /// <summary>
    /// نوع احراز هویت
    /// </summary>
    [StringLength(64)]
    public string? VerifyMode { get; set; }

    /// <summary>
    /// کد کار
    /// </summary>
    [StringLength(64)]
    public string? WorkCode { get; set; }

    /// <summary>
    /// دمای اندازه‌گیری شده
    /// </summary>
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Temperature { get; set; }

    /// <summary>
    /// ماسک
    /// </summary>
    public bool? Mask { get; set; }

    /// <summary>
    /// داده خام دستگاه
    /// </summary>
    public string? RawData { get; set; }

    /// <summary>
    /// زمان دریافت در سامانه
    /// </summary>
    public DateTime? ReceiveDate { get; set; }

    /// <summary>
    /// وضعیت پردازش
    /// </summary>
    [StringLength(64)]
    public string? Status { get; set; }

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

    [ForeignKey("AttendanceDeviceId")]
    [InverseProperty("AttendanceLogs")]
    public virtual AttendanceDevice AttendanceDevice { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("AttendanceLogs")]
    public virtual Employee Employee { get; set; } = null!;
}
