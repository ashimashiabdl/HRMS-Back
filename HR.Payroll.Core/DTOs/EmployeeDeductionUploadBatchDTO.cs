namespace HR.Payroll.Core.DTOs;

/// <summary>
/// برای نمایش در لیست دسته‌های آپلود کسورات
/// </summary>
public class EmployeeDeductionUploadBatchDTO
{
    public long Id { get; set; }
    public string? title { get; set; }
    public DateTime? CreateDate { get; set; }
    public long OrganisationChartId { get; set; }
    public long FileId { get; set; }
    public string? FileTitle { get; set; }
    public string? UploaderUserName { get; set; }
    public string? UploaderDisplayName { get; set; }
    public int TotalRowsRead { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
}
