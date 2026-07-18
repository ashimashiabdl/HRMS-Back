using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    [Table("Complexity", Schema = "Org")]
    public class Complexity : HR.SharedKernel.Data.BaseEntity
    {
        public short Level { get; set; }
        [StringLength(128)]
        public string? Description { get; set; }
    }
}
