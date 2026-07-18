namespace HR.BaseInfo.Core.DTOs;

public class ImportUploadResultDTO
{
    public long ImportBatchId { get; set; }
    public long FileId { get; set; }
    public int TotalRowsRead { get; set; }
    public int ValidCount { get; set; }
    public int WarningCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ImportFailedRowDTO> FailedRows { get; set; } = new();
}

public class ImportFailedRowDTO
{
    public int ExcelRowNumber { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RawPreview { get; set; }
}

public class ImportFinalizeResultDTO
{
    public long ImportBatchId { get; set; }
    public int InsertedCount { get; set; }
    public int SkippedCount { get; set; }
}
