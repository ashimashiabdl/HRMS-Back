using HR.Organisation.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data
{
    [Table("Interdict_Order_Copy", Schema = "Order")]
    public class InterdictOrderCopy : HR.SharedKernel.Data.BaseEntity
    {
        [ForeignKey("InterdictOrder")]
        public long InterdictOrderId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }
        [ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        public bool FinalSend { get; set; }
        [StringLength(32)]
        public string? AutomationLetterNo { get; set; }   
        [Column(TypeName = "date")]
        public DateTime? AutomationLetterDate { get; set; }   
        [StringLength(32)]
        public string? AutomationPostNo { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
