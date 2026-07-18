namespace HR.Employee.Core.DTOs;

public class EmployeeRequestFileDownloadDTO
{
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public string? Extension { get; set; }
    public string? ContentBase64 { get; set; }
}
