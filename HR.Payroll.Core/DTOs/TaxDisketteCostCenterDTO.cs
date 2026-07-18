using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class TaxDisketteCostCenterDTO : BaseDTO
    {
        public long TaxDisketteId { get; set; }
        public string? TaxDiskette { get; set; }
     
        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
    }
}
