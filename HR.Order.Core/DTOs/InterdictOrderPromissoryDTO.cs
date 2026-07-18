using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class InterdictOrderPromissoryDTO
    {
        [StringLength(70)]
        public string PromissoryNumber { get; set; } = null!;
        [StringLength(20)]
        public string PromissoryValue { get; set; } = null!;
        [StringLength(150)]
        public string? PromissoryNote { get; set; }
    }
}
