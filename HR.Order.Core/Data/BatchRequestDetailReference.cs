using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data
{
    [Table("Batch_Request_Detail_Reference", Schema = "Order")]
    public class BatchRequestDetailReference : BaseEntity, IignoreDateRangeValidation
    {
        [ForeignKey("BatchRequestDetail")]
        public long BatchRequestDetailId { get; set; }
        public virtual BatchRequestDetail? BatchRequestDetail { get; set; }
        [StringLength(128)]
        public string? FinalMessage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DoDatetime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastTryDateTime { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
