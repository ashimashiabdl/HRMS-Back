using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Deducted_Arears_Detail", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("ArearFicheId", Name = "IX_Deducted_Arears_Detail_ArearFicheId")]
[Microsoft.EntityFrameworkCore.Index("DeductedArearsId", Name = "IX_Deducted_Arears_Detail_DeductedArearsId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Deducted_Arears_Detail_PaymentPeriodId")]
public partial class DeductedArearsDetail
{
    [Key]
    public long Id { get; set; }

    public long DeductedArearsId { get; set; }

    public long? PaymentAmount { get; set; }

    public long PaymentPeriodId { get; set; }

    public long ArearFicheId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

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

    [ForeignKey("DeductedArearsId")]
    [InverseProperty("DeductedArearsDetails")]
    public virtual DeductedArear DeductedArears { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("DeductedArearsDetails")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;
}
