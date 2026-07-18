using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_Request_Detail_Reference", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("BatchRequestDetailId", Name = "IX_Batch_Request_Detail_Reference_BatchRequestDetailId")]
public partial class BatchRequestDetailReference
{
    [Key]
    public long Id { get; set; }

    public long BatchRequestDetailId { get; set; }

    [StringLength(128)]
    public string? FinalMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DoDatetime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastTryDateTime { get; set; }

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

    [ForeignKey("BatchRequestDetailId")]
    [InverseProperty("BatchRequestDetailReferences")]
    public virtual BatchRequestDetail BatchRequestDetail { get; set; } = null!;
}
