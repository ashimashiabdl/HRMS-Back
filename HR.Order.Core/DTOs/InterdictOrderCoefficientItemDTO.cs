using HR.Order.Core.Data;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class InterdictOrderCoefficientItemDTO : BaseDTO
    {
        public long InterdictOrderId { get; set; }
        public string? InterdictOrder { get; set; }
        public long CoefficientId { get; set; }
        public string? Coefficient { get; set; }
        public double? OutPutFactValue { get; set; }
    }
}
