using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Job_Complexity_JobScoringFactor_Question", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("ComplexityId", Name = "IX_Job_Complexity_JobScoringFactor_Question_ComplexityId")]
[Microsoft.EntityFrameworkCore.Index("JobScoringFactorId", Name = "IX_Job_Complexity_JobScoringFactor_Question_JobScoringFactorId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_Job_Complexity_JobScoringFactor_Question_OrganizationJobId")]
public partial class JobComplexityJobScoringFactorQuestion
{
    [Key]
    public long Id { get; set; }

    public long? OrganizationJobId { get; set; }

    public long? JobScoringFactorId { get; set; }

    public long? ComplexityId { get; set; }

    [StringLength(2048)]
    public string? Question { get; set; }

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

    [ForeignKey("ComplexityId")]
    [InverseProperty("JobComplexityJobScoringFactorQuestions")]
    public virtual Complexity? Complexity { get; set; }

    [ForeignKey("JobScoringFactorId")]
    [InverseProperty("JobComplexityJobScoringFactorQuestions")]
    public virtual JobScoringFactor? JobScoringFactor { get; set; }

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("JobComplexityJobScoringFactorQuestions")]
    public virtual OrganisationJob? OrganizationJob { get; set; }
}
