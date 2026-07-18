using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Account", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("AccountTypeId", Name = "IX_Bank_Account_AccountTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Bank_Account_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "OrganisationChartId", "Status", "IsDeleted", "Id", Name = "IX_Bank_Account_EmployeeId_OrgChart_Status", IsDescending = new[] { false, false, false, false, true })]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Bank_Account_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PayrollStatusId", Name = "IX_Bank_Account_PayrollStatusId")]
public partial class BankAccount
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    [StringLength(100)]
    public string? AccountNumber { get; set; }

    public long? AccountTypeId { get; set; }

    public int? Priority { get; set; }

    public int? FromPrice { get; set; }

    public int? ToPrice { get; set; }

    public bool Status { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    public long? BankId { get; set; }

    public int? BankBranchId { get; set; }

    public int? OldId { get; set; }

    public long? PayrollStatusId { get; set; }

    [StringLength(50)]
    public string? BonCardNumber { get; set; }

    [StringLength(50)]
    public string? ShabaNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(50)]
    public string? CardNumber { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("BankAccounts")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("BankAccounts")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
