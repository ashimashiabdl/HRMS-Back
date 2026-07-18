using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Diskette_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteGroupAndFileId", Name = "IX_Bank_Diskette_Item_BankDisketteGroupAndFileId")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteId", Name = "IX_Bank_Diskette_Item_BankDisketteId")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Bank_Diskette_Item_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Bank_Diskette_Item_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Bank_Diskette_Item_FicheId")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteGroupAndFileId", Name = "IX_Bank_Diskette_Item_GroupAndFileId")]
public partial class BankDisketteItem
{
    [Key]
    public long Id { get; set; }

    public long? BankDisketteGroupAndFileId { get; set; }

    public long EmployeeId { get; set; }

    public long? CostCenterId { get; set; }

    [StringLength(128)]
    public string? AccountNo { get; set; }

    public long Amount { get; set; }

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

    public long? FicheId { get; set; }

    public long BankDisketteId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankDisketteId")]
    [InverseProperty("BankDisketteItems")]
    public virtual BankDiskette BankDiskette { get; set; } = null!;

    [ForeignKey("BankDisketteGroupAndFileId")]
    [InverseProperty("BankDisketteItems")]
    public virtual BankDisketteGroupAndFile? BankDisketteGroupAndFile { get; set; }

    [ForeignKey("CostCenterId")]
    [InverseProperty("BankDisketteItems")]
    public virtual OrganisationChart? CostCenter { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("BankDisketteItems")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("FicheId")]
    [InverseProperty("BankDisketteItems")]
    public virtual Fiche? Fiche { get; set; }
}
