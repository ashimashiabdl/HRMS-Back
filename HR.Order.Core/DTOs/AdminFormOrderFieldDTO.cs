using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class AdminFormOrderFieldDTO
    {
        public string? Property { get; set; }
        public object? Value { get; set; }
        public string? recruitId { get; set; }
        public string? interdictid { get; set; }
    }
}
