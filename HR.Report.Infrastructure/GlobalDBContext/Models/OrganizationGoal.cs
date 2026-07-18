using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organization_Goal", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organization_Goal_OrganisationChartId")]
public partial class OrganizationGoal
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public string? GoalDescription { get; set; }

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
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganizationGoals")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
