namespace HR.WorkFlow.Core.DTOs;

public class WorkFlowInstanceBatchActionRequestDto
{
    public int ActionId { get; set; }
    public List<long> InstanceIds { get; set; } = [];
    public string? Comment { get; set; }
    public long? UserSignatureId { get; set; }
}
