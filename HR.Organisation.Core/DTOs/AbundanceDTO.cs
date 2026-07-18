using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class AbundanceDTO : BaseDTO
    {
        public short Level { get; set; }
        [StringLength(128)]
        public string? Description { get; set; }
        public bool Selected { get; set; }
    }
}
