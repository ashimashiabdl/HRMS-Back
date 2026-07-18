using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BankDTO : BaseDTO
    {
        public bool? IsValid { get; set; }
        public int? BankCode { get; set; }
    }
}
