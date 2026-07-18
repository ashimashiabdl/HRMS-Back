using HR.SharedKernel.Data;

namespace HR.Report.Core.DTOs;

/// <summary>
/// درخواست ساخت گزارش پویا
/// </summary>
public class ReportBuilderRequestDTO
{
    /// <summary>
    /// شناسه موجودیت اصلی
    /// </summary>
    public long EntityId { get; set; }

    /// <summary>
    /// فیلدهای انتخابی برای نمایش
    /// </summary>
    public List<long> SelectedFieldIds { get; set; } = new();

    /// <summary>
    /// فیلترهای اعمال شده
    /// </summary>
    public List<ReportFilterDTO> Filters { get; set; } = new();

    /// <summary>
    /// مرتب‌سازی
    /// </summary>
    public List<ReportSortDTO> Sorts { get; set; } = new();

    /// <summary>
    /// صفحه جاری
    /// </summary>
    public int CurrentPage { get; set; } = 0;

    /// <summary>
    /// تعداد رکورد در هر صفحه
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// نمایش آیتم های حکمی در گزارش (InterdictOrderWageItem)
    /// </summary>
    public bool IncludeInterdictOrderWageItems { get; set; } = false;

    /// <summary>
    /// نمایش ضریب ها در گزارش (InterdictOrderCoefficientItem)
    /// </summary>
    public bool IncludeInterdictOrderCoefficientItems { get; set; } = false;

    /// <summary>
    /// نمایش آیتم های فیش در گزارش (FicheItem)
    /// </summary>
    public bool IncludeFicheItems { get; set; } = false;

    /// <summary>
    /// سال دوره پرداخت برای فیلتر فیش
    /// </summary>
    public int? FichePaymentPeriodYear { get; set; }

    /// <summary>
    /// ماه دوره پرداخت برای فیلتر فیش
    /// </summary>
    public int? FichePaymentPeriodMonth { get; set; }

    /// <summary>
    /// نمایش آیتم های کار کرد در گزارش (PersonnelFunction)
    /// </summary>
    public bool IncludePersonnelFunctionItems { get; set; } = false;

    /// <summary>
    /// نمایش مرخصی ها در گزارش (FicheLeaveItem)
    /// </summary>
    public bool IncludeFicheLeaveItems { get; set; } = false;

    /// <summary>
    /// فیلدهای GroupBy برای گروه‌بندی داده‌ها
    /// </summary>
    public List<long> GroupByFieldIds { get; set; } = new();

    /// <summary>
    /// شناسه‌های محل پرداخت برای فیلتر کردن نتایج
    /// </summary>
    public List<long>? PayLocationIds { get; set; }

    /// <summary>
    /// کلمه عبور کاربر جاری برای تأیید هویت
    /// </summary>
    public string? CurrentUserPassword { get; set; }

    /// <summary>
    /// رمز عبور برای رمزنگاری فایل اکسل
    /// </summary>
    public string? FileEncryptionPassword { get; set; }

    /// <summary>
    /// کد کپچا
    /// </summary>
    public string? Captcha { get; set; }

    /// <summary>
    /// شناسه کپچا
    /// </summary>
    public string? CaptchaId { get; set; }
}

/// <summary>
/// فیلتر گزارش
/// </summary>
public class ReportFilterDTO
{
    /// <summary>
    /// شناسه فیلد
    /// </summary>
    public long FieldId { get; set; }

    /// <summary>
    /// شناسه عملگر
    /// </summary>
    public long OperatorId { get; set; }

    /// <summary>
    /// مقدار فیلتر
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// مقدار دوم (برای بین دو مقدار)
    /// </summary>
    public string? Value2 { get; set; }
}

/// <summary>
/// مرتب‌سازی گزارش
/// </summary>
public class ReportSortDTO
{
    /// <summary>
    /// شناسه فیلد
    /// </summary>
    public long FieldId { get; set; }

    /// <summary>
    /// جهت مرتب‌سازی (asc, desc)
    /// </summary>
    public string Direction { get; set; } = "asc";
}

