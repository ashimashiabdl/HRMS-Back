namespace HR.Report.Core.DTOs;

/// <summary>
/// متادیتای گزارش ساز - شامل تمام اطلاعات لازم برای ساخت گزارش
/// </summary>
public class ReportBuilderMetadataDTO
{
    /// <summary>
    /// موجودیت‌های قابل گزارش
    /// </summary>
    public List<ReportableEntityDTO> Entities { get; set; } = new();

    /// <summary>
    /// فیلدهای قابل گزارش (بر اساس موجودیت انتخابی)
    /// </summary>
    public List<ReportableFieldDTO> Fields { get; set; } = new();

    /// <summary>
    /// انواع داده
    /// </summary>
    public List<FieldDataTypeDTO> DataTypes { get; set; } = new();

    /// <summary>
    /// عملگرها
    /// </summary>
    public List<FieldOperatorDTO> Operators { get; set; } = new();

    /// <summary>
    /// حداکثر تعداد رکورد مجاز برای گزارش
    /// </summary>
    public int MaxRecordLimit { get; set; }
}

