using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class PaymentTypeDTO : BaseDTO
    {
         [StringLength(2)]
        public string? TaxCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeducted { get; set; }
        [StringLength(50)]
        public string? EnglishName { get; set; }
        public bool IsReward { get; set; }
    }
}
