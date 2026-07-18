using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Definition", Schema = "wf")]
[Microsoft.EntityFrameworkCore.Index("ActionId", Name = "IX_Definition_ActionId")]
[Microsoft.EntityFrameworkCore.Index("FromNodeId", Name = "IX_Definition_FromNodeId")]
[Microsoft.EntityFrameworkCore.Index("ToNodeId", Name = "IX_Definition_ToNodeId")]
[Microsoft.EntityFrameworkCore.Index("WorkFlowId", Name = "IX_Definition_WorkFlowId")]
public partial class Definition
{
    [Key]
    public long Id { get; set; }

    public long WorkFlowId { get; set; }

    public long? FromNodeId { get; set; }

    public long? ToNodeId { get; set; }

    public long ActionId { get; set; }

    public bool? AllowComment { get; set; }

    public bool? IsCommentRequired { get; set; }

    public bool NeedSignature { get; set; }

    public bool IsFinalTransition { get; set; }

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

    [ForeignKey("ActionId")]
    [InverseProperty("Definitions")]
    public virtual Action Action { get; set; } = null!;

    [ForeignKey("FromNodeId")]
    [InverseProperty("DefinitionFromNodes")]
    public virtual Node? FromNode { get; set; }

    [ForeignKey("ToNodeId")]
    [InverseProperty("DefinitionToNodes")]
    public virtual Node? ToNode { get; set; }

    [ForeignKey("WorkFlowId")]
    [InverseProperty("Definitions")]
    public virtual WorkFlow WorkFlow { get; set; } = null!;
}
