using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Branch", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankId", Name = "IX_Bank_Branch_BankId")]
public partial class BankBranch
{
    [Key]
    public long Id { get; set; }

    public long BankId { get; set; }

    [StringLength(512)]
    public string? Code { get; set; }

    [StringLength(64)]
    public string? Phone { get; set; }

    [StringLength(1024)]
    public string? Address { get; set; }

    public bool? IsValid { get; set; }

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

    [ForeignKey("BankId")]
    [InverseProperty("BankBranches")]
    public virtual Bank Bank { get; set; } = null!;

    [InverseProperty("BankBranch")]
    public virtual ICollection<PersonnelLoan> PersonnelLoans { get; set; } = new List<PersonnelLoan>();

    [InverseProperty("BankBranch")]
    public virtual ICollection<PersonnelPayment> PersonnelPayments { get; set; } = new List<PersonnelPayment>();
}
