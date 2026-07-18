using HR.SharedKernel.Attribute;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Base_Table", Schema = "bas")]
    public class BaseTable : HR.SharedKernel.Data.BaseEntity
    {
        [StringLength(450)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
        [StringLength(512)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? MetaData { get; set; }
        public virtual ICollection<BaseTableValue> BaseTableValues { get; set; }

    }
}
