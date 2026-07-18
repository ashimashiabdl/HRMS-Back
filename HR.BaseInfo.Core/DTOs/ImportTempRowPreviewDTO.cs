using HR.SharedKernel.Import;

namespace HR.BaseInfo.Core.DTOs;

public class ImportTempRowPreviewDTO
{
    public long Id { get; set; }
    public int RowNumber { get; set; }
    public string? title { get; set; }
    public string? Description { get; set; }
    public ImportValidationStatus ValidationStatus { get; set; }
    public string? ValidationMessage { get; set; }
    public string? ResolvedPreview { get; set; }
}
