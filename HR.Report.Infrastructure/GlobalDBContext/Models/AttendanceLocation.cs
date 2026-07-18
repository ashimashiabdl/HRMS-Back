using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Location", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Attendance_Location_OrganisationChartId")]
public partial class AttendanceLocation
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    /// <summary>
    /// واحد سازمانی مرتبط
    /// </summary>
    public long? RelatedOrganisationChartId { get; set; }

    /// <summary>
    /// کد محل حضور
    /// </summary>
    [StringLength(32)]
    public string? Code { get; set; }

    /// <summary>
    /// عرض جغرافیایی
    /// </summary>
    [Column(TypeName = "decimal(10, 7)")]
    public decimal Latitude { get; set; }

    /// <summary>
    /// طول جغرافیایی
    /// </summary>
    [Column(TypeName = "decimal(10, 7)")]
    public decimal Longitude { get; set; }

    /// <summary>
    /// شعاع مجاز (متر)
    /// </summary>
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Radius { get; set; }

    /// <summary>
    /// آدرس
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

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

    [InverseProperty("AttendanceLocation")]
    public virtual ICollection<AttendanceDevice> AttendanceDevices { get; set; } = new List<AttendanceDevice>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("AttendanceLocations")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("RelatedOrganisationChartId")]
    public virtual OrganisationChart? RelatedOrganisationChart { get; set; }
}
