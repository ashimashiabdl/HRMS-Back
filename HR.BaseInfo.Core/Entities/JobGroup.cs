using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Job_Group", Schema = "bas")]
    public class JobGroup : HR.SharedKernel.Data.BaseEntity
    {
    }
}
