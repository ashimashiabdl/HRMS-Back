using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Job_Score_Abundance_Complexity", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("AbundanceId", Name = "IX_Job_Score_Abundance_Complexity_AbundanceId")]
[Microsoft.EntityFrameworkCore.Index("ComplexityId", Name = "IX_Job_Score_Abundance_Complexity_ComplexityId")]
[Microsoft.EntityFrameworkCore.Index("JobScoringFactorId", Name = "IX_Job_Score_Abundance_Complexity_JobScoringFactorId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_Job_Score_Abundance_Complexity_OrganizationJobId")]
public partial class JobScoreAbundanceComplexity
{
    [Key]
    public long Id { get; set; }

    public long? OrganizationJobId { get; set; }

    public long? JobScoringFactorId { get; set; }

    public long? AbundanceId { get; set; }

    public long? ComplexityId { get; set; }

    public int Score { get; set; }

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

    public bool Selected { get; set; }

    public bool SelectedFromQuestion { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("AbundanceId")]
    [InverseProperty("JobScoreAbundanceComplexities")]
    public virtual Abundance? Abundance { get; set; }

    [ForeignKey("ComplexityId")]
    [InverseProperty("JobScoreAbundanceComplexities")]
    public virtual Complexity? Complexity { get; set; }

    [ForeignKey("JobScoringFactorId")]
    [InverseProperty("JobScoreAbundanceComplexities")]
    public virtual JobScoringFactor? JobScoringFactor { get; set; }

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("JobScoreAbundanceComplexities")]
    public virtual OrganisationJob? OrganizationJob { get; set; }
}
