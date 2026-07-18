using HR.SharedKernel.Data;

namespace HR.BaseInfo.Core.DTOs;

public class RequestDocumentRequirementDTO : BaseDTO
{
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public List<RequestDocumentRequirementDetailDTO>? Details { get; set; }
}
