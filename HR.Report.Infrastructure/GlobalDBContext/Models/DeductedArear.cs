using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Deducted_Arears", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("ArearId", Name = "IX_Deducted_Arears_ArearId")]
[Microsoft.EntityFrameworkCore.Index("StartDeductedPaymentPeriodId", Name = "IX_Deducted_Arears_StartDeductedPaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Deducted_Arears_WageItemId")]
public partial class DeductedArear
{
    [Key]
    public long Id { get; set; }

    public long ArearId { get; set; }

    public long WageItemId { get; set; }

    public long? AllAmount { get; set; }

    public long? RemainAmount { get; set; }

    public long? InstalmentAmount { get; set; }

    public int? InstalmentCount { get; set; }

    public long StartDeductedPaymentPeriodId { get; set; }

    public bool? IsActive { get; set; }

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

    [ForeignKey("ArearId")]
    [InverseProperty("DeductedArears")]
    public virtual Arear Arear { get; set; } = null!;

    [InverseProperty("DeductedArears")]
    public virtual ICollection<DeductedArearsDetail> DeductedArearsDetails { get; set; } = new List<DeductedArearsDetail>();

    [ForeignKey("StartDeductedPaymentPeriodId")]
    [InverseProperty("DeductedArears")]
    public virtual PaymentPeriod StartDeductedPaymentPeriod { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("DeductedArears")]
    public virtual WageItem WageItem { get; set; } = null!;
}
