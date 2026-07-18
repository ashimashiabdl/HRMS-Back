using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BaseResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
