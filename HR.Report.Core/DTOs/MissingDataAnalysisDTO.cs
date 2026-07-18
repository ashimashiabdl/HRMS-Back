namespace HR.Report.Core.DTOs;

/// <summary>
/// درخواست تحلیل اطلاعات ناقص
/// </summary>
public class MissingDataAnalysisRequestDTO
{
    /// <summary>
    /// شناسه موجودیت
    /// </summary>
    public long EntityId { get; set; }

    /// <summary>
    /// لیست شناسه فیلدهای انتخابی - اگر null یا خالی باشد، همه فیلدها بررسی می‌شوند
    /// </summary>
    public List<long>? FieldIds { get; set; }

    /// <summary>
    /// شناسه‌های محل پرداخت برای فیلتر (اختیاری)
    /// </summary>
    public List<long>? PayLocationIds { get; set; }
}

/// <summary>
/// پاسخ تحلیل اطلاعات ناقص
/// </summary>
public class MissingDataAnalysisResponseDTO
{
    /// <summary>
    /// شناسه موجودیت
    /// </summary>
    public long EntityId { get; set; }

    /// <summary>
    /// نام موجودیت
    /// </summary>
    public string? EntityName { get; set; }

    /// <summary>
    /// تعداد کل رکوردهای با اطلاعات ناقص
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// نتایج تحلیل
    /// </summary>
    public List<MissingDataResultDTO>? Results { get; set; }
}

/// <summary>
/// نتیجه تحلیل برای یک رکورد
/// </summary>
public class MissingDataResultDTO
{
    /// <summary>
    /// شناسه رکورد
    /// </summary>
    public string? RecordId { get; set; }

    /// <summary>
    /// شناسه نمایشی رکورد (مثلاً PersonelCode یا FirstName + LastName)
    /// </summary>
    public string? RecordIdentifier { get; set; }

    /// <summary>
    /// شناسه فیلد
    /// </summary>
    public long FieldId { get; set; }

    /// <summary>
    /// نام فیلد
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// نام فنی فیلد
    /// </summary>
    public string? FieldTechnicalName { get; set; }

    /// <summary>
    /// نام موجودیت
    /// </summary>
    public string? EntityName { get; set; }

    // فیلدهای اضافی برای Employee
    /// <summary>
    /// نام کارمند (FirstName)
    /// </summary>
    public string? EmployeeFirstName { get; set; }

    /// <summary>
    /// نام خانوادگی کارمند (LastName)
    /// </summary>
    public string? EmployeeLastName { get; set; }

    /// <summary>
    /// کد ملی کارمند (NationalNo)
    /// </summary>
    public string? EmployeeNationalNo { get; set; }

    /// <summary>
    /// عنوان سازمان (BaseOrganisation Title)
    /// </summary>
    public string? EmployeeBaseOrganisationTitle { get; set; }

    /// <summary>
    /// جنسیت کارمند (Gender Title)
    /// </summary>
    public string? EmployeeGenderTitle { get; set; }

    /// <summary>
    /// تاهل کارمند (MaritalStatus Title)
    /// </summary>
    public string? EmployeeMaritalStatusTitle { get; set; }
}

/// <summary>
/// موجودیت قابل بررسی
/// </summary>
public class AvailableEntityDTO
{
    public long Id { get; set; }
    public string? FriendlyName { get; set; }
    public string? TechnicalName { get; set; }
    public string? Schema { get; set; }
    public string? TableName { get; set; }
    public string? Description { get; set; }
    public int FieldCount { get; set; }
}

/// <summary>
/// فیلد قابل بررسی
/// </summary>
public class AvailableFieldDTO
{
    public long Id { get; set; }
    public string? FriendlyName { get; set; }
    public string? TechnicalName { get; set; }
    public string? DataTypeName { get; set; }
    public string? NavigationPath { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsSelectable { get; set; }
}

