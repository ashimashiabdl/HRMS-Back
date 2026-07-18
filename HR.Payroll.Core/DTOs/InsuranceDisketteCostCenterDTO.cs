using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class InsuranceDisketteCostCenterDTO : BaseDTO
    {
        public long InsuranceDisketteId { get; set; }
        public string? InsuranceDiskette { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
    }
}
