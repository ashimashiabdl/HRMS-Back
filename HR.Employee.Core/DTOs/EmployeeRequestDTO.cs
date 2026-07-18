using HR.SharedKernel.Data;

namespace HR.Employee.Core.DTOs;

public class EmployeeRequestDTO : BaseDTO
{
    public long? OrganisationChartId { get; set; }
    public long EmployeeId { get; set; }
    public string? EmployeeTitle { get; set; }
    public string? EmployeeFirstName { get; set; }
    public string? EmployeeLastName { get; set; }
    public string? EmployeeNationalNo { get; set; }
    public long RequestDocumentRequirementId { get; set; }
    public string? RequestDocumentRequirementTitle { get; set; }
    public long EmployeeRequestStatusId { get; set; }
    public string? EmployeeRequestStatusTitle { get; set; }
    public int? EmployeeRequestStatusCode { get; set; }
    public string? Description { get; set; }
    public int DocumentCount { get; set; }
    public List<EmployeeRequestDetailDTO>? Details { get; set; }
}
