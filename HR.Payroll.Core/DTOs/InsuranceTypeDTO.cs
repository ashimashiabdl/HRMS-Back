using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HR.Payroll.Core.DTOs
{
    public class InsuranceTypeDTO : BaseDTO
    {
        [StringLength(256)]
        public string? InsuranceCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
