using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class BlockedIpDTO : BaseDTO
{
    public string IpAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

