using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Job_Series", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("JobSeriesId", Name = "IX_Organisation_Job_Series_JobSeriesId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Job_Series_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationJobCategoryId", Name = "IX_Organisation_Job_Series_OrganisationJobCategoryId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationJobGroupId", Name = "IX_Organisation_Job_Series_OrganisationJobGroupId")]
public partial class OrganisationJobSeries
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? JobSeriesId { get; set; }

    public long OrganisationJobCategoryId { get; set; }

    public long OrganisationJobGroupId { get; set; }

    [StringLength(50)]
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

    [ForeignKey("JobSeriesId")]
    [InverseProperty("OrganisationJobSeries")]
    public virtual JobSeries? JobSeries { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationJobSeries")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationJobCategoryId")]
    [InverseProperty("OrganisationJobSeries")]
    public virtual OrganisationJobCategory OrganisationJobCategory { get; set; } = null!;

    [ForeignKey("OrganisationJobGroupId")]
    [InverseProperty("OrganisationJobSeries")]
    public virtual OrganisationJobGroup OrganisationJobGroup { get; set; } = null!;
}
