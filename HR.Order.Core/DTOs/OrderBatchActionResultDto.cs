namespace HR.Order.Core.DTOs;

public class OrderBatchActionItemResultDto
{
    public long OrderId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class OrderBatchActionResultDto
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<OrderBatchActionItemResultDto> Items { get; set; } = [];
}
