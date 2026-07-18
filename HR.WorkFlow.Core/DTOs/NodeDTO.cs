using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.DTOs
{
    public class NodeDTO : BaseDTO
    {
        public long WorkFlowId { get; set; }
        public string? WorkFlow { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
    }
}
