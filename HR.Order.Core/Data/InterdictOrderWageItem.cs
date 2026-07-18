using HR.BaseInfo.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data;

[Table("Interdict_Order_WageItem", Schema = "Order")]
public class InterdictOrderWageItem : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey("InterdictOrder")]
    public long InterdictOrderId { get; set; }
    public virtual InterdictOrder? InterdictOrder { get; set; }
    [ForeignKey("WageItem")]
    public long WageItemId { get; set; }
    public virtual WageItem? WageItem { get; set; }
    public int Value { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
