namespace HRMS.API.Models;

/// <summary>
/// Form models for multipart uploads. Swashbuckle requires IFormFile and form fields
/// on a single [FromForm] model rather than mixed action parameters.
/// </summary>
public class EmployeeDeductionUploadForm
{
    public string Title { get; set; } = "";
    public long DeductionTypeId { get; set; }
    public long StartDeductPaymentPeriodId { get; set; }
    public DateTime PaymentDate { get; set; }
    public IFormFile? File { get; set; }
}

public class ImportDetectColumnsForm
{
    public long ImportProfileId { get; set; }
    public IFormFile? File { get; set; }
    public int ContextMode { get; set; } = 1;
}

public class ImportUploadForm
{
    public long ImportProfileId { get; set; }
    public string Title { get; set; } = "";
    public IFormFile? File { get; set; }
    public string? ColumnMappingJson { get; set; }
    public string? ContextJson { get; set; }
    public int ContextMode { get; set; } = 1;
}

public class TaxResponseUploadForm
{
    public IFormFile? File { get; set; }
    public long BatchPayRollRequestId { get; set; }
}

public class ImageAttachmentForm
{
    public IFormFile? File { get; set; }
    public string? Title { get; set; }
}

public class EmployeeSettlementAttachmentUploadForm
{
    public long SettlementId { get; set; }
    public long SettlementDocumentAttachmentTypeId { get; set; }
    public string? Description { get; set; }
    public IFormFile? File { get; set; }
}
