using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Formula", Schema = "bas")]
    public class Formula : HR.SharedKernel.Data.BaseEntity
    {
    }
}
