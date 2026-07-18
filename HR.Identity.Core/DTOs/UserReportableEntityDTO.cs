using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class UserReportableEntityDTO : BaseDTO
{
    public long UserId { get; set; }
    public string? User { get; set; }
    public long ReportableEntityId { get; set; }
    public string? ReportableEntity { get; set; }
}

