using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Device", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("AttendanceLocationId", Name = "IX_Attendance_Device_AttendanceLocationId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Attendance_Device_OrganisationChartId")]
public partial class AttendanceDevice
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    /// <summary>
    /// کد دستگاه
    /// </summary>
    [StringLength(32)]
    public string? Code { get; set; }

    /// <summary>
    /// آدرس IP دستگاه
    /// </summary>
    [Column("DeviceIP")]
    [StringLength(45)]
    public string? DeviceIp { get; set; }

    /// <summary>
    /// پورت
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// شماره سریال
    /// </summary>
    [StringLength(128)]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// نوع دستگاه
    /// </summary>
    [StringLength(128)]
    public string? DeviceType { get; set; }

    /// <summary>
    /// base table value Id : 40300 (برند دستگاه)
    /// </summary>
    public long? BrandId { get; set; }

    /// <summary>
    /// مدل دستگاه
    /// </summary>
    [StringLength(128)]
    public string? Model { get; set; }

    public long AttendanceLocationId { get; set; }

    /// <summary>
    /// منطقه زمانی
    /// </summary>
    [StringLength(64)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// فاصله همگام‌سازی (دقیقه)
    /// </summary>
    public int SyncInterval { get; set; }

    /// <summary>
    /// آخرین تاریخ همگام‌سازی
    /// </summary>
    public DateTime? LastSyncDate { get; set; }

    /// <summary>
    /// base table value Id : 40299 (وضعیت دستگاه)
    /// </summary>
    public long? StatusId { get; set; }

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

    [ForeignKey("AttendanceLocationId")]
    [InverseProperty("AttendanceDevices")]
    public virtual AttendanceLocation AttendanceLocation { get; set; } = null!;

    [InverseProperty("AttendanceDevice")]
    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("AttendanceDevices")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
