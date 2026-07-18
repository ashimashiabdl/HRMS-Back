using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Agent_Of_Punishment_Encourage", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("AgentOfPunishmentEncourageGroupId", Name = "IX_Organisation_Agent_Of_Punishment_Encourage_AgentOfPunishmentEncourageGroupId")]
[Microsoft.EntityFrameworkCore.Index("AgentOfPunishmentEncourageId", Name = "IX_Organisation_Agent_Of_Punishment_Encourage_AgentOfPunishmentEncourageId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Agent_Of_Punishment_Encourage_OrganisationChartId")]
public partial class OrganisationAgentOfPunishmentEncourage
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long AgentOfPunishmentEncourageId { get; set; }

    public long AgentOfPunishmentEncourageGroupId { get; set; }

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

    [ForeignKey("AgentOfPunishmentEncourageId")]
    [InverseProperty("OrganisationAgentOfPunishmentEncourages")]
    public virtual AgentOfPunishmentEncourage AgentOfPunishmentEncourage { get; set; } = null!;

    [ForeignKey("AgentOfPunishmentEncourageGroupId")]
    [InverseProperty("OrganisationAgentOfPunishmentEncourages")]
    public virtual AgentOfPunishmentEncourageGroup AgentOfPunishmentEncourageGroup { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationAgentOfPunishmentEncourages")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
