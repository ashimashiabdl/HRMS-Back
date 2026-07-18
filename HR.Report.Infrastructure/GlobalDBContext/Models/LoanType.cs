using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Loan_Type", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Loan_Type_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SettlementItemId", Name = "IX_Loan_Type_SettlementItemId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Loan_Type_WageItemId")]
public partial class LoanType
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(128)]
    public string? Code { get; set; }

    [StringLength(512)]
    public string? Comment { get; set; }

    public bool? IsActive { get; set; }

    public long? MaxAmount { get; set; }

    [StringLength(128)]
    public string? LoanDocNo { get; set; }

    [StringLength(128)]
    public string? LoanDocDesc { get; set; }

    public bool? IsTradeBalance { get; set; }

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

    public long? SettlementItemId { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("LoanTypes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("LoanType")]
    public virtual ICollection<PersonnelLoan> PersonnelLoans { get; set; } = new List<PersonnelLoan>();

    [ForeignKey("SettlementItemId")]
    [InverseProperty("LoanTypes")]
    public virtual SettlementItem? SettlementItem { get; set; }

    [ForeignKey("WageItemId")]
    [InverseProperty("LoanTypes")]
    public virtual WageItem WageItem { get; set; } = null!;
}
