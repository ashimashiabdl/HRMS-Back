using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Node_Role_Rel", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("NodeId", Name = "IX_Node_Role_Rel_NodeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Node_Role_Rel_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("RoleId", Name = "IX_Node_Role_Rel_RoleId")]
[Microsoft.EntityFrameworkCore.Index("RoleId", "NodeId", "WorkFlowId", Name = "IX_Node_Role_Rel_RoleId_NodeId_WorkFlowId")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowId", Name = "IX_Node_Role_Rel_WorkFlowId")]
public partial class NodeRoleRel
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long WorkFlowId { get; set; }

    public long RoleId { get; set; }

    public long NodeId { get; set; }

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

    [ForeignKey("NodeId")]
    [InverseProperty("NodeRoleRels")]
    public virtual Node Node { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("NodeRoleRels")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("NodeRoleRels")]
    public virtual AspNetRole Role { get; set; } = null!;

    [ForeignKey("WorkFlowId")]
    [InverseProperty("NodeRoleRels")]
    public virtual WorkFlow WorkFlow { get; set; } = null!;
}
