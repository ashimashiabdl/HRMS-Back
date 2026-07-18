using HR.SharedKernel.Data;

namespace HR.Report.Core.DTOs;

public class FieldDataTypeDTO : BaseDTO
{
    public string? TypeName { get; set; }
    public string? FriendlyName { get; set; }
}

