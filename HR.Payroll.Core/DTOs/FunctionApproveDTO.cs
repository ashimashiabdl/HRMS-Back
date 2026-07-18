using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class FunctionApproveDTO
    {
        public long Id { get; set; }
        public List<long>? FunctionIdList { get; set; }
    }
}
