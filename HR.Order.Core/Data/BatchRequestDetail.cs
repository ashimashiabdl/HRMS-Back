using HR.SharedKernel.Attribute;
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
    [Table("Batch_Request_Detail", Schema = "Order")]
    public class BatchRequestDetail : BaseEntity, IignoreDateRangeValidation
    {
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
        [ForeignKey("BatchRequest")]
        public long BatchRequestId { get; set; }
        public virtual BatchRequest? BatchRequest { get; set; }
        [ForeignKey("InterdictOrder")]
        public long? InterdictId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }
        
        public string? FinalMessage { get; set; }      
        
        public string? PdfrawByteArrayFinalMessage { get; set; }       
        [StringLength(8096)]
        public string? PdfByteArrayFinalMessage { get; set; }   
        
        public string? ArchiveFinalMessage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DoDatetime { get; set; }    
        [Column(TypeName = "datetime")]
        public DateTime? LastTryDateTime { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
