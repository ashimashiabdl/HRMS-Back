using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("G20_Score_Domain_JobDegree", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_G20_Score_Domain_JobDegree_OrganisationChartId")]
public partial class G20ScoreDomainJobDegree
{
    [Key]
    public long Id { get; set; }

    public short JobDegree { get; set; }

    public int LowerLimit { get; set; }

    public int UpperLimit { get; set; }

    public float AmountOfRankIncrease { get; set; }

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

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("G20ScoreDomainJobDegrees")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
