using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class RoleReportableEntityDTO : BaseDTO
{
    public long RoleId { get; set; }
    public string? Role { get; set; }
    public long ReportableEntityId { get; set; }
    public string? ReportableEntity { get; set; }
}

