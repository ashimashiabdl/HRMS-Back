using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bill_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BillId", Name = "IX_Bill_Item_BillId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Bill_Item_WageItemId")]
public partial class BillItem
{
    [Key]
    public long Id { get; set; }

    public long BillId { get; set; }

    public long WageItemId { get; set; }

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
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BillId")]
    [InverseProperty("BillItems")]
    public virtual Bill Bill { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("BillItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
