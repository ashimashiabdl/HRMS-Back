using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("University", Schema = "bas")]
    public class University : BaseEntity
    {
        [StringLength(256)]
        public string? Description { get; set; }
    }
}
