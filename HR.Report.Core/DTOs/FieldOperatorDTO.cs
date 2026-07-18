using HR.SharedKernel.Data;

namespace HR.Report.Core.DTOs;

public class FieldOperatorDTO : BaseDTO
{
    public long FieldDataTypeId { get; set; }
    public string? Operator { get; set; }
    public string? FriendlyName { get; set; }
    public string? FieldDataTypeTitle { get; set; }
}

