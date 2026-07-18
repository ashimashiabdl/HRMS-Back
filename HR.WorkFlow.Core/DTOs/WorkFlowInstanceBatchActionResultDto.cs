namespace HR.WorkFlow.Core.DTOs;

public class WorkFlowInstanceBatchActionItemResultDto
{
    public long InstanceId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class WorkFlowInstanceBatchActionResultDto
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<WorkFlowInstanceBatchActionItemResultDto> Items { get; set; } = [];
}
