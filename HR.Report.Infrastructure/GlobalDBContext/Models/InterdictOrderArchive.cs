using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Interdict_Order_Archive", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Interdict_Order_Archive_InterdictOrderId", IsUnique = true)]
public partial class InterdictOrderArchive
{
    [Key]
    public long Id { get; set; }

    public long InterdictOrderId { get; set; }

    public byte[]? PdfrawByteArray { get; set; }

    public byte[]? PdfbyteArray { get; set; }

    public int? BaseMrtid { get; set; }

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

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("InterdictOrderArchive")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;
}
