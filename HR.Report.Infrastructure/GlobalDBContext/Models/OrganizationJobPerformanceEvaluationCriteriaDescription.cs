using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Performance_Evaluation_Criteria_Description", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("CriteriaTypeId", Name = "IX_OrganizationJob_Performance_Evaluation_Criteria_Description_CriteriaTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Performance_Evaluation_Criteria_Description_OrganizationJobId")]
public partial class OrganizationJobPerformanceEvaluationCriteriaDescription
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long CriteriaTypeId { get; set; }

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
    [InverseProperty("OrganizationJobPerformanceEvaluationCriteriaDescriptions")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
