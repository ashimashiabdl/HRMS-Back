using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Organization_Type", Schema = "bas")]
    public class OrganizationType : HR.SharedKernel.Data.BaseEntity
    {
        public int Priority { get; set; }
    }
}
