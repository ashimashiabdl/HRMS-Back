using HR.SharedKernel.Data;

namespace HR.WorkFlow.Core.DTOs;

public class WorkFlowInstanceDTO : BaseDTO
{
    public long WorkFlowId { get; set; }
    public string? WorkFlow { get; set; }
    public long? InterdictOrderId { get; set; }
    public string? InterdictOrder { get; set; }
    public long? EmployeeSettlementId { get; set; }
    public string? EmployeeSettlement { get; set; }
    public long LastActivityId { get; set; }
    public string? CreateBy { get; set; }
    public string? FormulaData { get; set; }
}
