using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Node", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Node_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowId", Name = "IX_Node_WorkFlowId")]
public partial class Node
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    public long WorkFlowId { get; set; }

    [InverseProperty("FromNode")]
    public virtual ICollection<ActivityTemplate> ActivityTemplateFromNodes { get; set; } = new List<ActivityTemplate>();

    [InverseProperty("ToNode")]
    public virtual ICollection<ActivityTemplate> ActivityTemplateToNodes { get; set; } = new List<ActivityTemplate>();

    [InverseProperty("FromNode")]
    public virtual ICollection<Definition> DefinitionFromNodes { get; set; } = new List<Definition>();

    [InverseProperty("ToNode")]
    public virtual ICollection<Definition> DefinitionToNodes { get; set; } = new List<Definition>();

    [InverseProperty("Node")]
    public virtual ICollection<NodeRoleRel> NodeRoleRels { get; set; } = new List<NodeRoleRel>();

    [InverseProperty("Node")]
    public virtual ICollection<NodeUserRel> NodeUserRels { get; set; } = new List<NodeUserRel>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Nodes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WorkFlowId")]
    [InverseProperty("Nodes")]
    public virtual WorkFlow WorkFlow { get; set; } = null!;
}
