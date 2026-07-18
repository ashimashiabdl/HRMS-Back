using HR.SharedKernel.Data;

namespace HR.BaseInfo.Core.DTOs;

public class RequestDocumentRequirementDetailDTO : BaseDTO
{
    public long RequestDocumentRequirementId { get; set; }
    public bool IsRequired { get; set; }
    public string? Description { get; set; }
}
