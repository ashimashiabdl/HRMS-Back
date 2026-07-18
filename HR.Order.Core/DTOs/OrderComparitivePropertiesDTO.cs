using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class OrderComparitivePropertiesDTO
    {
        public string? Property { get; set; }
        public string? LastValue { get; set; }
        public string? NewValue { get; set; }
        public string? PostFix { get; set; }
    }
}
