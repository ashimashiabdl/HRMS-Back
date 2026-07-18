using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_Request_Detail", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("BatchRequestId", Name = "IX_Batch_Request_Detail_BatchRequestId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Batch_Request_Detail_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("InterdictId", Name = "IX_Batch_Request_Detail_InterdictId")]
public partial class BatchRequestDetail
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long BatchRequestId { get; set; }

    public long? InterdictId { get; set; }

    public string? FinalMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DoDatetime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastTryDateTime { get; set; }

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

    public string? ArchiveFinalMessage { get; set; }

    public string? PdfByteArrayFinalMessage { get; set; }

    public string? PdfrawByteArrayFinalMessage { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BatchRequestId")]
    [InverseProperty("BatchRequestDetails")]
    public virtual BatchRequest BatchRequest { get; set; } = null!;

    [InverseProperty("BatchRequestDetail")]
    public virtual ICollection<BatchRequestDetailReference> BatchRequestDetailReferences { get; set; } = new List<BatchRequestDetailReference>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("BatchRequestDetails")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("InterdictId")]
    [InverseProperty("BatchRequestDetails")]
    public virtual InterdictOrder? Interdict { get; set; }
}
