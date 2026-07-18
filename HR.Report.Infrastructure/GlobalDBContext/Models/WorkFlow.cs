using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("WorkFlow", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_WorkFlow_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowTypeId", Name = "IX_WorkFlow_WorkFlowTypeId")]
public partial class WorkFlow
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? WorkFlowTypeId { get; set; }

    public bool IsActive { get; set; }

    public bool IsSystem { get; set; }

    [StringLength(512)]
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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [InverseProperty("WorkFlow")]
    public virtual ICollection<Definition> Definitions { get; set; } = new List<Definition>();

    [InverseProperty("WorkFlow")]
    public virtual ICollection<NodeRoleRel> NodeRoleRels { get; set; } = new List<NodeRoleRel>();

    [InverseProperty("WorkFlow")]
    public virtual ICollection<NodeUserRel> NodeUserRels { get; set; } = new List<NodeUserRel>();

    [InverseProperty("WorkFlow")]
    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("WorkFlows")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("WorkFlow")]
    public virtual ICollection<WorkFlowInstance> WorkFlowInstances { get; set; } = new List<WorkFlowInstance>();

    [ForeignKey("WorkFlowTypeId")]
    [InverseProperty("WorkFlows")]
    public virtual WorkFlowType? WorkFlowType { get; set; }
}
