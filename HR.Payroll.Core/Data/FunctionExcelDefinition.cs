using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

/// <summary>
/// تعریف اکسل کارکرد جهت آپلود
/// </summary>
[Table("Function_Excel_Definition", Schema = "Payroll")]
public class FunctionExcelDefinition : BaseEntity, IOrganisationChartId
{
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long MappedExcelColumnId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? MappedExcelColumn { get; set; }
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long? PersonnelFunctionColumnId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? PersonnelFunctionColumn { get; set; }
    [ForeignKey("ExcelDefinitionType")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long ExcelDefinitionTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual ExcelDefinitionType? ExcelDefinitionType { get; set; }
    public bool IsMandatory { get; set; }
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
    /// 
    /// </summary>
    public bool IsLeave { get; set; }
    public bool IsDaily { get; set; }
    /// <summary>
    /// نوع مرخصی
    /// </summary>
    [ForeignKey("LeaveType")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long? LeaveTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual LeaveType? LeaveType { get; set; }
    /// <summary>
    /// نوع استخدام
    /// </summary>
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }

    [NotMapped]
    private new string title { get; set; }
}
