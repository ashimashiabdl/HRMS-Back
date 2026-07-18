using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs
{
    public class InsurancePositionDTO : BaseDTO
    {
        [StringLength(50)]
        public string? InsPositionCode { get; set; }
        public bool? InsPositionActive { get; set; }
        [StringLength(250)]
        public string? Description { get; set; }
    }
}
