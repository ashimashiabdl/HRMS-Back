using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Order_Status", Schema = "bas")]
    public class OrderStatus : HR.SharedKernel.Data.BaseEntity
    {
        public int StatusCode { get; set; }
    }
}
