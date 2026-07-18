using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Node_User_Rel", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Node_User_Rel_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "NodeId", "WorkFlowId", Name = "IX_Node_User_Rel_EmployeeId_NodeId_WorkFlowId")]
[Microsoft.EntityFrameworkCore.Index("NodeId", Name = "IX_Node_User_Rel_NodeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Node_User_Rel_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowId", Name = "IX_Node_User_Rel_WorkFlowId")]
public partial class NodeUserRel
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long WorkFlowId { get; set; }

    public long EmployeeId { get; set; }

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

    [ForeignKey("EmployeeId")]
    [InverseProperty("NodeUserRels")]
    public virtual AspNetUser Employee { get; set; } = null!;

    [ForeignKey("NodeId")]
    [InverseProperty("NodeUserRels")]
    public virtual Node Node { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("NodeUserRels")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WorkFlowId")]
    [InverseProperty("NodeUserRels")]
    public virtual WorkFlow WorkFlow { get; set; } = null!;
}
