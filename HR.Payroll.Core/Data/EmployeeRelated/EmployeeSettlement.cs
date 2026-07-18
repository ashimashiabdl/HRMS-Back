using HR.BaseInfo.Core.Entities;
using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HR.Payroll.Core.Data.EmployeeRelated;


/// <summary>
/// تسویه حساب کارکنان
/// </summary>
[Table("Employee_Settlement", Schema = "Payroll")]
public class EmployeeSettlement : BaseEntity , IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }

    [ForeignKey("Employee")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeId { get; set; }
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }
    [Comment("تاریخ تنظیم و امضای فرم تسویه حساب")]
    [Column(TypeName = "date")]
    public DateTime SettlementDate { get; set; }

    [ForeignKey("InterdictOrder")]
    public long InterdictOrderId { get; set; }
    public virtual InterdictOrder? InterdictOrder { get; set; }

    [ForeignKey("SettlementCause")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long SettlementCauseId { get; set; }
    public virtual SettlementCause? SettlementCause { get; set; }

    /// <summary>
    /// وضعیت تسویه (FK به bas.Settlement_Status) — مقادیر <see cref="HR.SharedKernel.Share.Enums.SettlementStatus"/>.
    /// </summary>
    [ForeignKey("SettlementStatus")]
    public long? SettlementStatusId { get; set; }
    public virtual SettlementStatus? SettlementStatus { get; set; }


    [ForeignKey("LastInterdictOrder")]
    public long LastInterdictOrderId { get; set; }
    public virtual InterdictOrder? LastInterdictOrder { get; set; }


    [ForeignKey("Fiche")]
    public long? FicheId { get; set; }
    public virtual Fiche? Fiche { get; set; }

    /// <summary>
    /// سال مالی
    /// </summary>
    public int FiscalYear { get; set; }

    [MaxLength(1024)]
    public string? Description { get; set; }
    /// <summary>
    /// بازه زمانی تسویه حساب
    /// </summary>
    [StringLength(6)]
    public string? Duration { get; set; }
    /// <summary>
    /// جمع پرداختی ها
    /// </summary>
    public long PaymentAmount { get; set; }
    /// <summary>
    /// مبلغ خالص پرداختی
    /// </summary>
    public long PurePaymentAmount { get; set; }
    /// <summary>
    /// جمع کسورات
    /// </summary>
    public long DeductionSum { get; set; }
    [StringLength(128)]
    public string? BankAccountNo { get; set; }
    /// <summary>
    /// سالانه است ؟ یا وسط سال
    /// </summary>
    public bool IsYearLong { get; set; }

    /// <summary>
    /// آیا وام‌ها در تسویه حساب لحاظ می‌شوند؟
    /// </summary>
    public bool Loanincluded { get; set; }

    /// <summary>
    /// آیا کسورات در تسویه حساب لحاظ می‌شوند؟
    /// </summary>
    public bool Deductionincluded { get; set; }
}
