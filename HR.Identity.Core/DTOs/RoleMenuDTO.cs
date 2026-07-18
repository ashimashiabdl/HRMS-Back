using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;


namespace HR.Identity.Core.DTOs;

public class RoleMenuDTO : BaseDTO
{
    public long RoleId { get; set; }
    public string? Role { get; set; }
    public string? Claim { get; set; }
}
