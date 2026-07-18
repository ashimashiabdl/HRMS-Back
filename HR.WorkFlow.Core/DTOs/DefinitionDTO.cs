using HR.SharedKernel.Data;
using HR.WorkFlow.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.DTOs
{
    public class DefinitionDTO : BaseDTO
    {
        public long WorkFlowId { get; set; }
        public string? WorkFlow { get; set; }
        public long? FromNodeId { get; set; }
        public string? FromNode { get; set; }
        public long? ToNodeId { get; set; }
        public string? ToNode { get; set; }
        public long ActionId { get; set; }
        public string? ActionDesc { get; set; }
        public bool? AllowComment { get; set; }
        public bool IsFinalTransition { get; set; }
        public bool? IsCommentRequired { get; set; }
        public bool NeedSignature { get; set; }
    }
}
