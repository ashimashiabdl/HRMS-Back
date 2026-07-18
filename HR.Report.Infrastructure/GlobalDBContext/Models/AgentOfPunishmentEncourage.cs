using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Agent_Of_Punishment_Encourage", Schema = "bas")]
public partial class AgentOfPunishmentEncourage
{
    [Key]
    public long Id { get; set; }

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

    public bool IsPunishment { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("AgentOfPunishmentEncourage")]
    public virtual ICollection<GroupPunishmentEncourage> GroupPunishmentEncourages { get; set; } = new List<GroupPunishmentEncourage>();

    [InverseProperty("AgentOfPunishmentEncourage")]
    public virtual ICollection<OrganisationAgentOfPunishmentEncourage> OrganisationAgentOfPunishmentEncourages { get; set; } = new List<OrganisationAgentOfPunishmentEncourage>();

    [InverseProperty("AgentOfPunishmentEncourage")]
    public virtual ICollection<PunishmentEncourage> PunishmentEncourages { get; set; } = new List<PunishmentEncourage>();
}
