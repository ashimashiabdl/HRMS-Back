using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Abundance", Schema = "Org")]
public partial class Abundance
{
    [Key]
    public long Id { get; set; }

    public short Level { get; set; }

    [StringLength(128)]
    public string? Description { get; set; }

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

    [InverseProperty("Abundance")]
    public virtual ICollection<JobAbundanceJobScoringFactorQuestion> JobAbundanceJobScoringFactorQuestions { get; set; } = new List<JobAbundanceJobScoringFactorQuestion>();

    [InverseProperty("Abundance")]
    public virtual ICollection<JobScoreAbundanceComplexity> JobScoreAbundanceComplexities { get; set; } = new List<JobScoreAbundanceComplexity>();
}
