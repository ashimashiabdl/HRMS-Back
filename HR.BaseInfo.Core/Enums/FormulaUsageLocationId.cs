namespace HR.BaseInfo.Core.Enums;

/// <summary>
/// محل استفاده فرمول (FK به bas.Formula_Usage_Location)
/// </summary>
public enum FormulaUsageLocationId : long
{
    /// <summary>
    /// عامل های حقوقی حکم
    /// </summary>
    OrderWageItems = 1,

    /// <summary>
    /// آیتم های فیش حقوقی
    /// </summary>
    PayrollFicheItems = 2,

    /// <summary>
    /// تسویه حساب
    /// </summary>
    Settlement = 3,

    /// <summary>
    /// ضرائب ( عوامل ) حکم
    /// </summary>
    OrderCoefficients = 4,

    /// <summary>
    /// فرمول چک
    /// </summary>
    CheckFormula = 5,

    /// <summary>
    /// محاسبه فیش حقوقی
    /// </summary>
    PayrollFicheCalculation = 6,

    /// <summary>
    /// فرمول بررسی صحت
    /// </summary>
    VerificationFormula = 7,
}
