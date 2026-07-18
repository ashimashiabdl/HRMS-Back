using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Position", Schema = "bas")]
    public class Position : HR.SharedKernel.Data.BaseEntity
    {

    }
   
}
