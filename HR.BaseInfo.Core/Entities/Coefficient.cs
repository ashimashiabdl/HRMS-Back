using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Coefficient", Schema = "bas")]
    public class Coefficient : HR.SharedKernel.Data.BaseEntity
    {
        [StringLength(450)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
    }
}
