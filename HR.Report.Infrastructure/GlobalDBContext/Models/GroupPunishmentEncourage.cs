using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Group_Punishment_Encourage", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("AgentOfPunishmentEncourageId", Name = "IX_Group_Punishment_Encourage_AgentOfPunishmentEncourageId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationAgentOfPunishmentEncourageScoreIntervalId", Name = "IX_Group_Punishment_Encourage_OrganisationAgentOfPunishmentEncourageScoreIntervalId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Group_Punishment_Encourage_OrganisationChartId")]
public partial class GroupPunishmentEncourage
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public string? LastModifiedUser { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long AgentOfPunishmentEncourageId { get; set; }

    public string? Description { get; set; }

    public long? OrganisationAgentOfPunishmentEncourageScoreIntervalId { get; set; }

    public int? EmPloyeeCount { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("AgentOfPunishmentEncourageId")]
    [InverseProperty("GroupPunishmentEncourages")]
    public virtual AgentOfPunishmentEncourage AgentOfPunishmentEncourage { get; set; } = null!;

    [InverseProperty("GroupPunishmentEncourage")]
    public virtual ICollection<GroupPunishmentEncourageFile> GroupPunishmentEncourageFiles { get; set; } = new List<GroupPunishmentEncourageFile>();

    [ForeignKey("OrganisationAgentOfPunishmentEncourageScoreIntervalId")]
    [InverseProperty("GroupPunishmentEncourages")]
    public virtual OrganisationAgentOfPunishmentEncourageScoreInterval? OrganisationAgentOfPunishmentEncourageScoreInterval { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("GroupPunishmentEncourages")]
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [InverseProperty("GroupPunishmentEncourage")]
    public virtual ICollection<PunishmentEncourage> PunishmentEncourages { get; set; } = new List<PunishmentEncourage>();
}
