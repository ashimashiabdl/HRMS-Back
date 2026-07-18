using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Core.DTOs
{
    public class ApiResultDTO
    {
        public bool Success { get; set; }
        public long Id { get; set; }
        public string Message { get; set; }
    }
}
