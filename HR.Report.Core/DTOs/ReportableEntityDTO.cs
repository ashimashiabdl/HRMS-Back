using HR.SharedKernel.Data;

namespace HR.Report.Core.DTOs;

public class ReportableEntityDTO : BaseDTO
{
    public string? TechnicalName { get; set; }
    public string? FriendlyName { get; set; }
    public string? Schema { get; set; }
    public string? TableName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

