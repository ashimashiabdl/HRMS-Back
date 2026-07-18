using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Job_Series", Schema = "bas")]
    public class JobSeries : HR.SharedKernel.Data.BaseEntity
    {
    }
}
