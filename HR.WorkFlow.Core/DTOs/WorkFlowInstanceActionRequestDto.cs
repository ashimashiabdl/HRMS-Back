namespace HR.WorkFlow.Core.DTOs;

public class WorkFlowInstanceActionRequestDto
{
    public int ActionId { get; set; }
    public long InstanceId { get; set; }
    public string? Comment { get; set; }
    public long? UserSignatureId { get; set; }
}
