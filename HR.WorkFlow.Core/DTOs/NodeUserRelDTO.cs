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
    public class NodeUserRelDTO : BaseDTO
    {
        public long WorkFlowId { get; set; }
        public string? WorkFlow { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long NodeId { get; set; }
        public string? Node { get; set; }
    }
}
