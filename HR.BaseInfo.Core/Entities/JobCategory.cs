using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Job_Category", Schema = "bas")]
    public class JobCategory : HR.SharedKernel.Data.BaseEntity
    {
    }
}
