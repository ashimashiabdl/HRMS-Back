using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Interdict_Order_Promissory", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Interdict_Order_Promissory_InterdictOrderId")]
public partial class InterdictOrderPromissory
{
    [Key]
    public long Id { get; set; }

    public long InterdictOrderId { get; set; }

    [StringLength(70)]
    public string PromissoryNumber { get; set; } = null!;

    [StringLength(20)]
    public string PromissoryValue { get; set; } = null!;

    [StringLength(150)]
    public string? PromissoryNote { get; set; }

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
    [InverseProperty("InterdictOrderPromissories")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;
}
