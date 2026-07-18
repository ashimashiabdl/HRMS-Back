using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class EmployeeSettlementAttachmentDTO : BaseDTO
{
    public long EmployeeSettlementId { get; set; }
    public long SettlementDocumentAttachmentTypeId { get; set; }
    public string? SettlementDocumentAttachmentTypeTitle { get; set; }
    public long FileId { get; set; }
    public string? FileTitle { get; set; }
    public string? FileExtension { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? Description { get; set; }
}
