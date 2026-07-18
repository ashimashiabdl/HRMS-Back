using HR.SharedKernel.Data;

namespace HR.Report.Core.DTOs;

public class ReportableFieldDTO : BaseDTO
{
    public long ReportableEntityId { get; set; }
    public string? TechnicalName { get; set; }
    public string? FriendlyName { get; set; }
    public long FieldDataTypeId { get; set; }
    public string? NavigationPath { get; set; }
    public long? BaseTableId { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsSelectable { get; set; }
    public bool IsSortable { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public string? ReportableEntityTitle { get; set; }
    public string? FieldDataTypeTitle { get; set; }
    /// <summary>
    /// آیا فیلد در جدول اصلی از نوع Long است (نه از FieldDataType)
    /// این برای تشخیص FK columns مثل BloodGroupId استفاده می‌شود
    /// </summary>
    public bool IsLongColumn { get; set; }
    /// <summary>
    /// نام جدول مقصد برای FK fields (مثل Places برای BirthPlaceId)
    /// این برای ساخت URL GetAsKeyValuePair استفاده می‌شود
    /// </summary>
    public string? TargetTableName { get; set; }
}

