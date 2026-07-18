using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Punishment_Encourage", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("AgentOfPunishmentEncourageId", Name = "IX_Punishment_Encourage_AgentOfPunishmentEncourageId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Punishment_Encourage_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("GroupPunishmentEncourageId", Name = "IX_Punishment_Encourage_GroupPunishmentEncourageId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationAgentOfPunishmentEncourageScoreIntervalId", Name = "IX_Punishment_Encourage_OrganisationAgentOfPunishmentEncourageScoreIntervalId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Punishment_Encourage_OrganisationChartId")]
public partial class PunishmentEncourage
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long AgentOfPunishmentEncourageId { get; set; }

    public int UnitValue { get; set; }

    public string? Description { get; set; }

    public long? GroupPunishmentEncourageId { get; set; }

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

    public bool IsGroup { get; set; }

    public long? OrganisationAgentOfPunishmentEncourageScoreIntervalId { get; set; }

    public int Value { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("AgentOfPunishmentEncourageId")]
    [InverseProperty("PunishmentEncourages")]
    public virtual AgentOfPunishmentEncourage AgentOfPunishmentEncourage { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("PunishmentEncourages")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("GroupPunishmentEncourageId")]
    [InverseProperty("PunishmentEncourages")]
    public virtual GroupPunishmentEncourage? GroupPunishmentEncourage { get; set; }

    [ForeignKey("OrganisationAgentOfPunishmentEncourageScoreIntervalId")]
    [InverseProperty("PunishmentEncourages")]
    public virtual OrganisationAgentOfPunishmentEncourageScoreInterval? OrganisationAgentOfPunishmentEncourageScoreInterval { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PunishmentEncourages")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
