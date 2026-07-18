using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Job_Scoring_Factor", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("GroupId", Name = "IX_Job_Scoring_Factor_GroupId")]
public partial class JobScoringFactor
{
    [Key]
    public long Id { get; set; }

    public long GroupId { get; set; }

    public int Percent { get; set; }

    public int MaximumScore { get; set; }

    public int Priority { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

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

    [InverseProperty("JobScoringFactor")]
    public virtual ICollection<JobAbundanceJobScoringFactorQuestion> JobAbundanceJobScoringFactorQuestions { get; set; } = new List<JobAbundanceJobScoringFactorQuestion>();

    [InverseProperty("JobScoringFactor")]
    public virtual ICollection<JobComplexityJobScoringFactorQuestion> JobComplexityJobScoringFactorQuestions { get; set; } = new List<JobComplexityJobScoringFactorQuestion>();

    [InverseProperty("JobScoringFactor")]
    public virtual ICollection<JobScoreAbundanceComplexity> JobScoreAbundanceComplexities { get; set; } = new List<JobScoreAbundanceComplexity>();
}
