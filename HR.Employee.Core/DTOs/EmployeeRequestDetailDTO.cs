using HR.SharedKernel.Data;

namespace HR.Employee.Core.DTOs;

public class EmployeeRequestDetailDTO : BaseDTO
{
    public long EmployeeRequestId { get; set; }
    public long RequestDocumentRequirementDetailId { get; set; }
    public string? RequestDocumentRequirementDetailTitle { get; set; }
    public long? FileId { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
}
