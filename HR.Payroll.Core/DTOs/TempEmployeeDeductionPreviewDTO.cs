namespace HR.Payroll.Core.DTOs;

/// <summary>
/// برای پیش‌نمایش سرور-ساید ردیف‌های موقت کسورات
/// </summary>
public class TempEmployeeDeductionPreviewDTO
{
    public long Id { get; set; }
    public string? NationalNo { get; set; }
    public long? EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; }
    public string? DeductionTypeTitle { get; set; }
    public long? InstallmentAmount { get; set; }
    public long? AllAmount { get; set; }
    public string? ParseErrorMessage { get; set; }
}
