using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data
{
    /// <summary>
    /// آرشیو PDF حکم. هر <see cref="InterdictOrderId"/> فقط یک رکورد مجاز است.
    /// </summary>
    [Table("Interdict_Order_Archive", Schema = "Order")]
    public class InterdictOrderArchive : HR.SharedKernel.Data.BaseEntity
    {
        /// <summary>
        /// شناسه حکم — یونیک در سطح دیتابیس و اپلیکیشن.
        /// </summary>
        [ForeignKey("InterdictOrder")]
        public long InterdictOrderId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }
        public byte[]? PdfrawByteArray { get; set; } = null!;
        public byte[]? PdfbyteArray { get; set; } = null!;
        public int? BaseMrtid { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
