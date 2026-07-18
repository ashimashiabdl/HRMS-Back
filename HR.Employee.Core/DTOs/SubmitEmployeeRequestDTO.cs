namespace HR.Employee.Core.DTOs;

public class SubmitEmployeeRequestDTO
{
    public long RequestDocumentRequirementId { get; set; }
    public string? Description { get; set; }
    public List<SubmitEmployeeRequestDetailDTO> Details { get; set; } = new();
}

public class SubmitEmployeeRequestDetailDTO
{
    public long RequestDocumentRequirementDetailId { get; set; }
    public long? FileId { get; set; }
    public string? Description { get; set; }
}
