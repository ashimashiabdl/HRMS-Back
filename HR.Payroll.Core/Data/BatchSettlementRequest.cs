using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Batch_Settlement_Request", Schema = "Payroll")]
public class BatchSettlementRequest : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("User")]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }

    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    public long RequestStateId { get; set; }
    public long RequestTypeId { get; set; }

    [ForeignKey("SettlementCause")]
    public long SettlementCauseId { get; set; }
    public virtual SettlementCause? SettlementCause { get; set; }

    [ForeignKey("PaymentPeriod")]
    public long? PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }

    public long? PayLocationId { get; set; }
    public long? CostCenterId { get; set; }

    [Column(TypeName = "date")]
    public DateTime SettlementDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime SettlementStartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime SettlementEndDate { get; set; }

    public int FiscalYear { get; set; }
    public bool IsYearLong { get; set; }
    public bool Loanincluded { get; set; }
    public bool Deductionincluded { get; set; }
    public bool SendToCartable { get; set; }
    public bool ProceedWithoutFiche { get; set; }

    [StringLength(256)]
    public string? Username { get; set; }

    [StringLength(4096)]
    public string? RequsetDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastPoolingTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinishDateTime { get; set; }

    public bool IsDone { get; set; }
    public int EmployeeCount { get; set; }
    public int SuccessCount { get; set; }
    public long? PoolingEmployeeId { get; set; }

    public string? ExeptionMessage { get; set; }
}