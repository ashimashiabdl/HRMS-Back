using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Report", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_User_Report_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_Report_UserId")]
public partial class UserReport
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long UserId { get; set; }

    /// <summary>
    /// fill from DynamicReport Table in schema rpt
    /// </summary>
    public long DynamicReportId { get; set; }

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
    [InverseProperty("UserReports")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserReports")]
    public virtual AspNetUser User { get; set; } = null!;
}
