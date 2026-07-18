using HR.SharedKernel.Data;

namespace Hr.SystemSetting.Core.DTOs;

public class OrganisationEmployeeTypeSettlementItemDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public long EmployeeTypeId { get; set; }
    public string? EmployeeTypeTitle { get; set; }
    public long SettlementItemId { get; set; }
    public string? SettlementItemTitle { get; set; }
    /// <summary>
    /// پرداختی یا کسور
    /// </summary>
    public long PaymentTypeId { get; set; }
    public string? PaymentTypeTitle { get; set; }
    /// <summary>
    /// نحوه محاسبه (EnterTypeId / BaseTable 72)
    /// </summary>
    public long EnterTypeId { get; set; }
    public string? EnterTypeTitle { get; set; }
    public long? OrganisationFormulaId { get; set; }
    public string? OrganisationFormulaTitle { get; set; }
    public long? MeasurementUnitId { get; set; }
    public string? MeasurementUnitTitle { get; set; }
    /// <summary>
    /// مقدار ثابت وقتی نحوه محاسبه = عدد ثابت
    /// </summary>
    public int? FixValue { get; set; }
    public int? Priority { get; set; }
    public bool? IsEditAble { get; set; }
    public bool? ShowLoan { get; set; }
    public string? FinancialCode { get; set; }
    public bool? IsImport { get; set; }
    /// <summary>
    /// تنها در محاسبات لحاظ گردد (در جمع پرداختی/کسور لحاظ نمی‌شود)
    /// </summary>
    public bool? IsVirtual { get; set; }
}
