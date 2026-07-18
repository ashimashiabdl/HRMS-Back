using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Project", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Project_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("ProjectId", Name = "IX_Organisation_Project_ProjectId")]
public partial class OrganisationProject
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long ProjectId { get; set; }

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

    [StringLength(512)]
    public string? ProjectDescription { get; set; }

    public int? RefId { get; set; }

    public int? RefType { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationProjects")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("OrganisationProjects")]
    public virtual Project Project { get; set; } = null!;
}
