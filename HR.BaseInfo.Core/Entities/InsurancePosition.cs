using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Insurance_Position", Schema = "bas")]
    public class InsurancePosition :  HR.SharedKernel.Data.BaseEntity
    {
        [StringLength(50)]
        public string? InsPositionCode { get; set; }
        public bool? InsPositionActive { get; set; }
        [StringLength(250)]
        public string? Description { get; set; }
    }
}
