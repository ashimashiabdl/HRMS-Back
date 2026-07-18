using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hr.SystemSetting.Core.Entities.OrganisationEmployeeTypes;

[Table("Organisation_EmployeeType_Settlement_Item", Schema = "Setting")]
public class OrganisationEmployeeTypeSettlementItem : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }
    [ForeignKey("SettlementItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long SettlementItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual SettlementItem? SettlementItem { get; set; }
    /// <summary>
    /// پرداختی یا کسور ؟
    /// </summary>
    public long PaymentTypeId { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? PaymentType { get; set; }

    /// <summary>
    /// نحوه محاسبه (BaseTable group 72 / EnterTypeId)
    /// </summary>
    [ForeignKey("EnterType")]
    public long EnterTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? EnterType { get; set; }

    [ForeignKey("OrganisationFormula")]
    public long? OrganisationFormulaId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual OrganisationFormula? OrganisationFormula { get; set; }

    [ForeignKey("MeasurementUnit")]
    public long? MeasurementUnitId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual MeasurementUnit? MeasurementUnit { get; set; }

    /// <summary>
    /// مقدار ثابت — وقتی نحوه محاسبه = عدد ثابت
    /// </summary>
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public int? FixValue { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public int? Priority { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public bool? IsEditAble { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public bool? ShowLoan { get; set; }
    [MaxLength(256)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? FinancialCode { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public bool? IsImport { get; set; }
    /// <summary>
    /// تنها در محاسبات لحاظ گردد (در جمع پرداختی/کسور لحاظ نمی‌شود)
    /// </summary>
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public bool? IsVirtual { get; set; }
}
