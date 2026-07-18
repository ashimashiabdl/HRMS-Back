using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Interdict_Order_CoefficientItem", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("CoefficientId", Name = "IX_Interdict_Order_CoefficientItem_CoefficientId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Interdict_Order_CoefficientItem_InterdictOrderId")]
public partial class InterdictOrderCoefficientItem
{
    [Key]
    public long Id { get; set; }

    public long InterdictOrderId { get; set; }

    public long CoefficientId { get; set; }

    public double? OutPutFactValue { get; set; }

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

    [ForeignKey("CoefficientId")]
    [InverseProperty("InterdictOrderCoefficientItems")]
    public virtual Coefficient Coefficient { get; set; } = null!;

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("InterdictOrderCoefficientItems")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;
}
