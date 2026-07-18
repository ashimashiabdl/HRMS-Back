using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;

namespace HR.Payroll.Core.DTOs;

public class EmployeeSettlementCalculatedItemDTO : BaseDTO
{
    public int Index { get; set; }
    public long SettlementItemId { get; set; }
    public string? SettlementItemTitle { get; set; }
    public long PaymentTypeId { get; set; }
    public string? PaymentTypeTitle { get; set; }
    public int? Priority { get; set; }
    public bool IsEditAble { get; set; }
    public bool IsVirtual { get; set; }
    public bool UsedManualAmount { get; set; }
    public long Amount { get; set; }
    /// <summary>
    /// مبلغ محاسبه‌شده توسط سیستم قبل از اعمال مبلغ دستی کاربر.
    /// </summary>
    public long SystemCalculatedAmount { get; set; }
    public string? Description { get; set; }
    public long? OrganisationFormulaId { get; set; }
    public string? OrganisationFormula { get; set; }
    public bool IsRowSuccess { get; set; }
    public string? FormulaRowMessage { get; set; }
    public int SuccessRunTimeInmilliseconds { get; set; }
    public string? FormulaFriendlyText { get; set; }
    public Dictionary<string, string?>? VariableFriendlyList { get; set; }
    public string? FormulaText { get; set; }
    public FormulaExecutionTree? FormulaTreeParser { get; set; }
    public string? FormulaHelpDesc { get; set; }
}
