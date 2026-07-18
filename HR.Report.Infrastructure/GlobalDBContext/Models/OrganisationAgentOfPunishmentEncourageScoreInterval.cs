using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Agent_Of_Punishment_Encourage_Score_Interval", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Agent_Of_Punishment_Encourage_Score_Interval_OrganisationChartId")]
public partial class OrganisationAgentOfPunishmentEncourageScoreInterval
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public int FromValue { get; set; }

    public int ToValue { get; set; }

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

    [InverseProperty("OrganisationAgentOfPunishmentEncourageScoreInterval")]
    public virtual ICollection<GroupPunishmentEncourage> GroupPunishmentEncourages { get; set; } = new List<GroupPunishmentEncourage>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationAgentOfPunishmentEncourageScoreIntervals")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("OrganisationAgentOfPunishmentEncourageScoreInterval")]
    public virtual ICollection<PunishmentEncourage> PunishmentEncourages { get; set; } = new List<PunishmentEncourage>();
}
