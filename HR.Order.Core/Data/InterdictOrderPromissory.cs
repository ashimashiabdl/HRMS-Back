using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data
{
    [Table("Interdict_Order_Promissory", Schema = "Order")]
    public class InterdictOrderPromissory : HR.SharedKernel.Data.BaseEntity
    {
        [ForeignKey("InterdictOrder")]
        public long InterdictOrderId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }
        [StringLength(70)]
        public string PromissoryNumber { get; set; } = null!;
        [StringLength(20)]
        public string PromissoryValue { get; set; } = null!;
        [StringLength(150)]
        public string? PromissoryNote { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
