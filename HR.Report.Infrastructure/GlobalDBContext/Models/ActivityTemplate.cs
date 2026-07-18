using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Activity_Template", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("ActionId", Name = "IX_Activity_Template_ActionId")]
[Microsoft.EntityFrameworkCore.Index("FromNodeId", Name = "IX_Activity_Template_FromNodeId")]
[Microsoft.EntityFrameworkCore.Index("ToNodeId", Name = "IX_Activity_Template_ToNodeId")]
[Microsoft.EntityFrameworkCore.Index("UserSignatureId", Name = "IX_Activity_Template_UserSignatureId")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowInstanceId", Name = "IX_Activity_Template_WorkFlowInstanceId")]
public partial class ActivityTemplate
{
    [Key]
    public long Id { get; set; }

    public long WorkFlowInstanceId { get; set; }

    public long? FromNodeId { get; set; }

    public long? ToNodeId { get; set; }

    public long ActionId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DoDate { get; set; }

    public bool Pending { get; set; }

    public bool IsFinalTransition { get; set; }

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

    public string? Comment { get; set; }

    public long? UserSignatureId { get; set; }

    [ForeignKey("ActionId")]
    [InverseProperty("ActivityTemplates")]
    public virtual Action Action { get; set; } = null!;

    [ForeignKey("FromNodeId")]
    [InverseProperty("ActivityTemplateFromNodes")]
    public virtual Node? FromNode { get; set; }

    [ForeignKey("ToNodeId")]
    [InverseProperty("ActivityTemplateToNodes")]
    public virtual Node? ToNode { get; set; }

    [ForeignKey("UserSignatureId")]
    [InverseProperty("ActivityTemplates")]
    public virtual UserSignature? UserSignature { get; set; }

    [ForeignKey("WorkFlowInstanceId")]
    [InverseProperty("ActivityTemplates")]
    public virtual WorkFlowInstance WorkFlowInstance { get; set; } = null!;
}
