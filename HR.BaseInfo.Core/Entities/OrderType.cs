using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.Entities;

[Table("Order_Type", Schema = "bas")]
public class OrderType : HR.SharedKernel.Data.BaseEntity
{
 
    [StringLength(450)]
    public string? Description { get; set; }
}
