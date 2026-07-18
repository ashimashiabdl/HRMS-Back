namespace HR.Payroll.Core.DTOs;

public class EmployeeSettlementCalculationResultDTO
{
    public EmployeeSettlementDTO Settlement { get; set; } = new();
    public List<EmployeeSettlementCalculatedItemDTO> CalculatedItems { get; set; } = [];
    public bool HasCalculationErrors { get; set; }
    public int FailedItemCount { get; set; }
    public List<string> ErrorMessages { get; set; } = [];
    public int? InterdictOrderSerial { get; set; }
    public long InterdictOrderId { get; set; }
    public long LastInterdictOrderId { get; set; }
    /// <summary>
    /// همیشه true — این خروجی فقط پیش‌نمایش است و هیچ رکوردی ذخیره نمی‌شود.
    /// </summary>
    public bool IsPreviewOnly { get; set; } = true;
}
