using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Diskette_CostCenter", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteId", Name = "IX_Bank_Diskette_CostCenter_BankDisketteId")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Bank_Diskette_CostCenter_CostCenterId")]
public partial class BankDisketteCostCenter
{
    [Key]
    public long Id { get; set; }

    public long BankDisketteId { get; set; }

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

    [ForeignKey("BankDisketteId")]
    [InverseProperty("BankDisketteCostCenters")]
    public virtual BankDiskette BankDiskette { get; set; } = null!;

    [ForeignKey("CostCenterId")]
    [InverseProperty("BankDisketteCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;
}
