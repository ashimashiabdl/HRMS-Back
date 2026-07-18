using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data.EmployeeRelated;

/// <summary>
/// آرشیو PDF تسویه حساب. هر <see cref="EmployeeSettlementId"/> فقط یک رکورد مجاز است.
/// </summary>
[Table("Employee_Settlement_Archive", Schema = "Payroll")]
public class EmployeeSettlementArchive : HR.SharedKernel.Data.BaseEntity
{
    /// <summary>
    /// شناسه تسویه حساب — یونیک در سطح دیتابیس و اپلیکیشن.
    /// </summary>
    [ForeignKey("EmployeeSettlement")]
    public long EmployeeSettlementId { get; set; }
    public virtual EmployeeSettlement? EmployeeSettlement { get; set; }
    public byte[]? PdfrawByteArray { get; set; } = null!;
    public byte[]? PdfbyteArray { get; set; } = null!;
    public int? BaseMrtid { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
