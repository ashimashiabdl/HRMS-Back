using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.Data
{
    [Table("Activity_Template", Schema = "wf")]
    public class ActivityTemplate : BaseEntity
    {
        [ForeignKey("WorkFlowInstance")]
        public long WorkFlowInstanceId { get; set; }
        public virtual WorkFlowInstance? WorkFlowInstance { get; set; }

        [ForeignKey("FromNode")]
        public long? FromNodeId { get; set; }
        public virtual Node? FromNode { get; set; }

        [ForeignKey("ToNode")]
        public long? ToNodeId { get; set; }
        public virtual Node? ToNode { get; set; }

        [ForeignKey("Action")]
        public long ActionId { get; set; }
        public virtual Action? Action { get; set; }

        [ForeignKey("UserSignature")]
        public long? UserSignatureId { get; set; }
        public virtual UserSignature? UserSignature { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DoDate { get; set; }
        public bool Pending { get; set; }
        public bool IsFinalTransition { get; set; }
        public string? Comment { get; set; }
    }
}
