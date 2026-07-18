using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Related_OrganizationJob_Description", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_Related_OrganizationJob_Description_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationRelatedJobId", Name = "IX_Related_OrganizationJob_Description_OrganizationRelatedJobId")]
public partial class RelatedOrganizationJobDescription
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long OrganizationRelatedJobId { get; set; }

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

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("RelatedOrganizationJobDescriptionOrganizationJobs")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;

    [ForeignKey("OrganizationRelatedJobId")]
    [InverseProperty("RelatedOrganizationJobDescriptionOrganizationRelatedJobs")]
    public virtual OrganisationJob OrganizationRelatedJob { get; set; } = null!;
}
