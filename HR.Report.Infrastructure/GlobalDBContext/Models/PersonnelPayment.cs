using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Personnel_Payment", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankBranchId", Name = "IX_Personnel_Payment_BankBranchId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Personnel_Payment_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Personnel_Payment_FicheId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Personnel_Payment_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFicheItemId", Name = "IX_Personnel_Payment_PersonnelFicheItemId")]
public partial class PersonnelPayment
{
    [Key]
    public long Id { get; set; }

    public long PaymentTypeId { get; set; }

    public long? BankBranchId { get; set; }

    public long EmployeeId { get; set; }

    public long Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

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

    public long FicheId { get; set; }

    public long PersonnelFicheItemId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankBranchId")]
    [InverseProperty("PersonnelPayments")]
    public virtual BankBranch? BankBranch { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("PersonnelPayments")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("FicheId")]
    [InverseProperty("PersonnelPayments")]
    public virtual Fiche Fiche { get; set; } = null!;

    [InverseProperty("PersonnelPayment")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("PersonnelPayments")]
    public virtual PaymentType PaymentType { get; set; } = null!;

    [ForeignKey("PersonnelFicheItemId")]
    [InverseProperty("PersonnelPayments")]
    public virtual PersonnelFicheItem PersonnelFicheItem { get; set; } = null!;

    [InverseProperty("PersonnelPayment")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItems { get; set; } = new List<PersonnelFicheItem>();
}
