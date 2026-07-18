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
    [Table("Employee_Status", Schema = "bas")]
    public class EmployeeStatus : HR.SharedKernel.Data.BaseEntity
    {
        [StringLength(450)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
    }
}
