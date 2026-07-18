using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Job_Category", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("JobCategoryId", Name = "IX_Organisation_Job_Category_JobCategoryId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Job_Category_OrganisationChartId")]
public partial class OrganisationJobCategory
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? JobCategoryId { get; set; }

    [StringLength(10)]
    public string? Code { get; set; }

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

    [ForeignKey("JobCategoryId")]
    [InverseProperty("OrganisationJobCategories")]
    public virtual JobCategory? JobCategory { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationJobCategories")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("OrganisationJobCategory")]
    public virtual ICollection<OrganisationJobGroup> OrganisationJobGroups { get; set; } = new List<OrganisationJobGroup>();

    [InverseProperty("OrganisationJobCategory")]
    public virtual ICollection<OrganisationJobSeries> OrganisationJobSeries { get; set; } = new List<OrganisationJobSeries>();
}
