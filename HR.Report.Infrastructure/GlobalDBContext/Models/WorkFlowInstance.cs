using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("WorkFlow_Instance", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_WorkFlow_Instance_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", "CreateDate", Name = "IX_WorkFlow_Instance_InterdictOrderId_CreateDate")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowId", Name = "IX_WorkFlow_Instance_WorkFlowId")]
public partial class WorkFlowInstance
{
    [Key]
    public long Id { get; set; }

    public long WorkFlowId { get; set; }

    public long? InterdictOrderId { get; set; }

    public long LastActivityId { get; set; }

    [StringLength(64)]
    public string? CreateBy { get; set; }

    public string? FormulaData { get; set; }

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

    public long? EmployeeSettlementId { get; set; }

    [InverseProperty("WorkFlowInstance")]
    public virtual ICollection<ActivityTemplate> ActivityTemplates { get; set; } = new List<ActivityTemplate>();

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("WorkFlowInstances")]
    public virtual InterdictOrder? InterdictOrder { get; set; }

    [ForeignKey("WorkFlowId")]
    [InverseProperty("WorkFlowInstances")]
    public virtual WorkFlow WorkFlow { get; set; } = null!;
}
