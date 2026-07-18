using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Dynamic_Report", Schema = "rpt")]
[Microsoft.EntityFrameworkCore.Index("ExportTypeId", Name = "IX_Dynamic_Report_ExportTypeId")]
[Microsoft.EntityFrameworkCore.Index("FuctionTypeId", Name = "IX_Dynamic_Report_FuctionTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Dynamic_Report_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationMrtid", Name = "IX_Dynamic_Report_OrganisationMRTId")]
public partial class DynamicReport
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public string? SqlQuery { get; set; }

    [StringLength(256)]
    public string? EnglishName { get; set; }

    [StringLength(32)]
    public string? Schema { get; set; }

    [StringLength(255)]
    public string? FunctionName { get; set; }

    [StringLength(1024)]
    public string? Help { get; set; }

    public bool IsActive { get; set; }

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

    [Column("OrganisationMRTId")]
    public long? OrganisationMrtid { get; set; }

    public long FuctionTypeId { get; set; }

    /// <summary>
    /// base table value Id : 40286 (excel or pdf)
    /// </summary>
    public long ExportTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("DynamicReport")]
    public virtual ICollection<DynamicReportParameter> DynamicReportParameters { get; set; } = new List<DynamicReportParameter>();

    [ForeignKey("ExportTypeId")]
    [InverseProperty("DynamicReportExportTypes")]
    public virtual BaseTableValue ExportType { get; set; } = null!;

    [ForeignKey("FuctionTypeId")]
    [InverseProperty("DynamicReportFuctionTypes")]
    public virtual BaseTableValue FuctionType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("DynamicReports")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationMrtid")]
    [InverseProperty("DynamicReports")]
    public virtual OrganisationMrt? OrganisationMrt { get; set; }
}
