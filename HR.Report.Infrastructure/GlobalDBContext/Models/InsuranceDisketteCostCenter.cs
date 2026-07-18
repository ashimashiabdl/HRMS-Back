using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Insurance_Diskette_CostCenter", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Insurance_Diskette_CostCenter_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceDisketteId", Name = "IX_Insurance_Diskette_CostCenter_InsuranceDisketteId")]
public partial class InsuranceDisketteCostCenter
{
    [Key]
    public long Id { get; set; }

    public long InsuranceDisketteId { get; set; }

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
    [InverseProperty("InsuranceDisketteCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("InsuranceDisketteId")]
    [InverseProperty("InsuranceDisketteCostCenters")]
    public virtual InsuranceDiskette InsuranceDiskette { get; set; } = null!;
}
