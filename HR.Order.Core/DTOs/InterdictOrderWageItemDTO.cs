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
    public class InterdictOrderWageItemDTO : BaseDTO
    {
        public long InterdictOrderId { get; set; }
        public string? InterdictOrder { get; set; }
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public int Value { get; set; }
        public bool? IsDaily { get; set; }
    }
}
