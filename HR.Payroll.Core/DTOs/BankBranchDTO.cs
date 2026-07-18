using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BankBranchDTO : BaseDTO
    {
        public long BankId { get; set; }
        public string? Bank { get; set; }
        [StringLength(512)]
        public string? Code { get; set; }
        [StringLength(64)]
        public string? Phone { get; set; }
        [StringLength(1024)]
        public string? Address { get; set; }
        public bool? IsValid { get; set; }
    }
}
