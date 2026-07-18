using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Project", Schema = "bas")]
    public class Project : HR.SharedKernel.Data.BaseEntity
    {
    }
}
