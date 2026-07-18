using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Education_Group", Schema = "bas")]
    public class EducationGroup : BaseEntity
    {
        public int OldID { get; set; }
    }
}
