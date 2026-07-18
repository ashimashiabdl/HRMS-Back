using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Batch_Settlement_Request_Detail", Schema = "Payroll")]
public class BatchSettlementRequestDetail : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("BatchSettlementRequest")]
    public long BatchSettlementRequestId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BatchSettlementRequest? BatchSettlementRequest { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long InterdictOrderId { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long? EmployeeSettlementId { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? FinalMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DoDatetime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastTryDateTime { get; set; }

    public double RunTimeinMilliseconds { get; set; }
}
