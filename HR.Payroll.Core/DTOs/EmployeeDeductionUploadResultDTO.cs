namespace HR.Payroll.Core.DTOs;

/// <summary>
/// نتیجه آپلود و پردازش اکسل کسورات
/// </summary>
public class EmployeeDeductionUploadResultDTO
{
    public long EmployeeDeductionUploadBatchId { get; set; }
    public long FileId { get; set; }
    /// <summary>
    /// تعداد کل ردیف‌های خوانده‌شده از اکسل (بدون احتساب هدر)
    /// </summary>
    public int TotalRowsRead { get; set; }
    /// <summary>
    /// تعداد استخراج موفق (کارمند یافت شد و مبلغ معتبر)
    /// </summary>
    public int SuccessCount { get; set; }
    /// <summary>
    /// تعداد استخراج ناموفق
    /// </summary>
    public int FailedCount { get; set; }
    /// <summary>
    /// ردیف‌های ناموفق با دلیل خطا
    /// </summary>
    public List<EmployeeDeductionUploadFailedRowDTO> FailedRows { get; set; } = new();
}

public class EmployeeDeductionUploadFailedRowDTO
{
    public int ExcelRowNumber { get; set; }
    public string? NationalNoRaw { get; set; }
    public string? AmountRaw { get; set; }
    public string ErrorMessage { get; set; } = "";
}
