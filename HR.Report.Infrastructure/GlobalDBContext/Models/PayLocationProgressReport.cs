using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("PayLocation_Progress_Report", Schema = "rpt")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_PayLocation_Progress_Report_OrganisationChartId")]
public partial class PayLocationProgressReport
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? UploadedByUserId { get; set; }

    public string? ReportDesc { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PayLocationProgressReports")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
