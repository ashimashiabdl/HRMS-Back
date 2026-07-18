namespace HR.Report.Core.DTOs;

/// <summary>
/// پاسخ گزارش ساز پویا
/// </summary>
public class ReportBuilderResponseDTO
{
    /// <summary>
    /// ستون‌های گزارش
    /// </summary>
    public List<ReportColumnDTO> Columns { get; set; } = new();

    /// <summary>
    /// داده‌های گزارش
    /// </summary>
    public List<Dictionary<string, object?>> Data { get; set; } = new();

    /// <summary>
    /// تعداد کل رکوردها
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// صفحه جاری
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// تعداد رکورد در هر صفحه
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// تعداد کل صفحات
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}

/// <summary>
/// ستون گزارش
/// </summary>
public class ReportColumnDTO
{
    /// <summary>
    /// شناسه فیلد
    /// </summary>
    public long FieldId { get; set; }

    /// <summary>
    /// نام فنی
    /// </summary>
    public string? TechnicalName { get; set; }

    /// <summary>
    /// نام نمایشی
    /// </summary>
    public string? FriendlyName { get; set; }

    /// <summary>
    /// نوع داده
    /// </summary>
    public string? DataType { get; set; }
}

