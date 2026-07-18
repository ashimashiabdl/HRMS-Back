using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BankDisketteCostCenterDTO : BaseDTO
    {
        public long BankDisketteId { get; set; }
        public string? BankDiskette { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
    }
}
