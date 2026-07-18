using HR.BaseInfo.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Order.Core.Data;

[Table("Interdict_Order_CoefficientItem", Schema = "Order")]
public class InterdictOrderCoefficientItem : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey("InterdictOrder")]
    public long InterdictOrderId { get; set; }
    public virtual InterdictOrder? InterdictOrder { get; set; }
    [ForeignKey("Coefficient")]
    public long CoefficientId { get; set; }
    public virtual Coefficient? Coefficient { get; set; }
    public double? OutPutFactValue { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
