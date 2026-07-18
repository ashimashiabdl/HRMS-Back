using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Base_WorkPlace", Schema = "bas")]
    public class BaseWorkPlace : HR.SharedKernel.Data.BaseEntity
    {
        [StringLength(256)]
        public string? WorkPlaceCode { get; set; }
    }
}
