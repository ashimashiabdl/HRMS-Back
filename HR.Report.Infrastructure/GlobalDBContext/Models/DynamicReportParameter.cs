using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Dynamic_Report_Parameter", Schema = "rpt")]
[Microsoft.EntityFrameworkCore.Index("DynamicReportId", Name = "IX_Dynamic_Report_Parameter_DynamicReportId")]
[Microsoft.EntityFrameworkCore.Index("ParameterId", Name = "IX_Dynamic_Report_Parameter_ParameterId")]
public partial class DynamicReportParameter
{
    [Key]
    public long Id { get; set; }

    public long DynamicReportId { get; set; }

    public bool Optional { get; set; }

    [StringLength(256)]
    public string? DefaultValue { get; set; }

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

    public long ParameterId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("DynamicReportId")]
    [InverseProperty("DynamicReportParameters")]
    public virtual DynamicReport DynamicReport { get; set; } = null!;

    [ForeignKey("ParameterId")]
    [InverseProperty("DynamicReportParameters")]
    public virtual BaseTableValue Parameter { get; set; } = null!;
}
