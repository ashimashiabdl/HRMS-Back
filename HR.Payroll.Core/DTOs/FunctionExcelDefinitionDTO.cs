using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class FunctionExcelDefinitionDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long ExcelDefinitionTypeId { get; set; }
    public string? ExcelDefinitionType { get; set; }
    public long MappedExcelColumnId { get; set; }
    public string? MappedExcelColumn { get; set; }
    public long? PersonnelFunctionColumnId { get; set; }
    public string? PersonnelFunctionColumn { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsDaily { get; set; }

    /// <summary>
    /// فرمت وارده ساعت و دقیقه است ؟
    /// </summary>
    public bool IsHourMinute { get; set; }
    /// <summary>
    /// بخش اول استفاده شود یا دوم
    /// </summary>
    public bool IsFirstOrSecondSection { get; set; }

    public bool NeedMinuteNormalization { get; set; }

    /// <summary>
    /// آیا ستون مربوط به مرخصی است ؟
    /// </summary>
    public bool IsLeave { get; set; }

    /// <summary>
    /// نوع مرخصی
    /// </summary>
    public long? LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }

    /// <summary>
    /// نوع استخدام
    /// </summary>
    public long EmployeeTypeId { get; set; }
    public string? EmployeeType { get; set; }

}
