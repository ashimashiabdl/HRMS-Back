using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class CommonPasswordDTO : BaseDTO
{
    public string Password { get; set; } = string.Empty;
}

