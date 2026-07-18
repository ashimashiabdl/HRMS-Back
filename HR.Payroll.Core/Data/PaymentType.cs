using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Payment_Type", Schema = "Payroll")]
public class PaymentType : BaseEntity
{
    [StringLength(2)]
    public string? TaxCode { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeducted { get; set; }
    [StringLength(50)]
    public string? EnglishName { get; set; }
    public bool IsReward { get; set; }
}
