using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class GetBatchRequestArchiveStatus_Result
    {
        public long Id { get; set; }
        public int SuccessCount { get; set; }
        public int SuccessArchiveCount { get; set; }
    }
}
