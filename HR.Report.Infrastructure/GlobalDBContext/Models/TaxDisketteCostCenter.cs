using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_Diskette_CostCenter", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Tax_Diskette_CostCenter_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("TaxDisketteId", Name = "IX_Tax_Diskette_CostCenter_TaxDisketteId")]
public partial class TaxDisketteCostCenter
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// شناسه جدول دیسکت
    /// </summary>
    public long TaxDisketteId { get; set; }

    public long CostCenterId { get; set; }

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

    [ForeignKey("CostCenterId")]
    [InverseProperty("TaxDisketteCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("TaxDisketteId")]
    [InverseProperty("TaxDisketteCostCenters")]
    public virtual TaxDiskette TaxDiskette { get; set; } = null!;
}
