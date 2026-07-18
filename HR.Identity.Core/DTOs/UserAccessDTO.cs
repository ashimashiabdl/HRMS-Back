
using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class UserAccessDTO : BaseDTO
{
    public long UserId { get; set; }
    public long? RoleId { get; set; }
    public string? User { get; set; }
    public string claimType { get; set; }
    public bool claimValue { get; set; }
}
