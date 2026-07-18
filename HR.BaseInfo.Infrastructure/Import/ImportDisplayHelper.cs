using HR.SharedKernel.Import;

namespace HR.BaseInfo.infrastructure.Import;

public static class ImportDisplayHelper
{
    public static string GetBatchStatusTitle(ImportBatchStatus status) => status switch
    {
        ImportBatchStatus.Draft => "پیش‌نویس",
        ImportBatchStatus.Parsed => "پردازش‌شده",
        ImportBatchStatus.PreviewReady => "آماده پیش‌نمایش",
        ImportBatchStatus.Finalizing => "در حال نهایی‌سازی",
        ImportBatchStatus.Completed => "تکمیل‌شده",
        ImportBatchStatus.Failed => "ناموفق",
        ImportBatchStatus.Cancelled => "لغوشده",
        _ => status.ToString()
    };

    public static string GetValidationStatusTitle(ImportValidationStatus status) => status switch
    {
        ImportValidationStatus.Valid => "معتبر",
        ImportValidationStatus.Warning => "هشدار",
        ImportValidationStatus.Error => "خطا",
        _ => status.ToString()
    };

    public static string GetHandlerTypeTitle(ImportHandlerType type) => type switch
    {
        ImportHandlerType.Simple => "ساده",
        ImportHandlerType.EmployeeLinked => "وابسته به کارمند",
        ImportHandlerType.Custom => "اختصاصی",
        _ => type.ToString()
    };

    public static string GetContextControlTypeTitle(ImportContextControlType type) => type switch
    {
        ImportContextControlType.Text => "متن",
        ImportContextControlType.Number => "عدد",
        ImportContextControlType.Date => "تاریخ",
        ImportContextControlType.Combo => "Combo",
        _ => type.ToString()
    };

    public static string GetFkLookupTypeTitle(FkLookupType type) => type switch
    {
        FkLookupType.None => "—",
        FkLookupType.ContextForm => "فرم Context",
        FkLookupType.NaturalKey => "کلید طبیعی",
        FkLookupType.NationalNo => "کد ملی",
        FkLookupType.ComboKeyValue => "Combo",
        FkLookupType.OrganScoped => "محدود به سازمان",
        _ => type.ToString()
    };
}
