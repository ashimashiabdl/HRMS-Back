using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Job_Group", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("JobGroupId", Name = "IX_Organisation_Job_Group_JobGroupId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Job_Group_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationJobCategoryId", Name = "IX_Organisation_Job_Group_OrganisationJobCategoryId")]
[Microsoft.EntityFrameworkCore.Index("StateId", Name = "IX_Organisation_Job_Group_StateId")]
public partial class OrganisationJobGroup
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? StateId { get; set; }

    public long? JobGroupId { get; set; }

    public long OrganisationJobCategoryId { get; set; }

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public int Order { get; set; }

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

    [ForeignKey("JobGroupId")]
    [InverseProperty("OrganisationJobGroups")]
    public virtual JobGroup? JobGroup { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationJobGroups")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationJobCategoryId")]
    [InverseProperty("OrganisationJobGroups")]
    public virtual OrganisationJobCategory OrganisationJobCategory { get; set; } = null!;

    [InverseProperty("OrganisationJobGroup")]
    public virtual ICollection<OrganisationJobSeries> OrganisationJobSeries { get; set; } = new List<OrganisationJobSeries>();
}
