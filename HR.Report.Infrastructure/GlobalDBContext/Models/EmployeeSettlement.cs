using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Settlement", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_Settlement_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Employee_Settlement_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Employee_Settlement_FicheId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Employee_Settlement_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("LastInterdictOrderId", Name = "IX_Employee_Settlement_LastInterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_Settlement_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SettlementCauseId", Name = "IX_Employee_Settlement_SettlementCauseId")]
[Microsoft.EntityFrameworkCore.Index("SettlementStatusId", Name = "IX_Employee_Settlement_SettlementStatusId")]
public partial class EmployeeSettlement
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long EmployeeId { get; set; }

    /// <summary>
    /// تاریخ تنظیم و امضای فرم تسویه حساب
    /// </summary>
    public DateOnly SettlementDate { get; set; }

    public long SettlementCauseId { get; set; }

    public long InterdictOrderId { get; set; }

    public long LastInterdictOrderId { get; set; }

    public long? FicheId { get; set; }

    public int FiscalYear { get; set; }

    [StringLength(1024)]
    public string? Description { get; set; }

    [StringLength(6)]
    public string? Duration { get; set; }

    public long PaymentAmount { get; set; }

    public long PurePaymentAmount { get; set; }

    public long DeductionSum { get; set; }

    [StringLength(128)]
    public string? BankAccountNo { get; set; }

    public bool IsYearLong { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    public bool Loanincluded { get; set; }

    public bool Deductionincluded { get; set; }

    public long? SettlementStatusId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeSettlements")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("EmployeeSettlement")]
    public virtual ICollection<EmployeeSettlementItem> EmployeeSettlementItems { get; set; } = new List<EmployeeSettlementItem>();

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("EmployeeSettlements")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("FicheId")]
    [InverseProperty("EmployeeSettlements")]
    public virtual Fiche? Fiche { get; set; }

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("EmployeeSettlementInterdictOrders")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;

    [ForeignKey("LastInterdictOrderId")]
    [InverseProperty("EmployeeSettlementLastInterdictOrders")]
    public virtual InterdictOrder LastInterdictOrder { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeSettlements")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("SettlementCauseId")]
    [InverseProperty("EmployeeSettlements")]
    public virtual SettlementCause SettlementCause { get; set; } = null!;

    [ForeignKey("SettlementStatusId")]
    [InverseProperty("EmployeeSettlements")]
    public virtual SettlementStatus? SettlementStatus { get; set; }
}
