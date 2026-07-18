using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities;

[Table("Places", Schema = "bas")]
public class Places : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey("ParentPlace")]
    public long? ParentPlaceId { get; set; }
    public virtual Places? ParentPlace { get; set; }
    public virtual BaseTableValue? PlaceType { get; set; }
    public long? PlaceTypeId { get; set; }

    [StringLength(3)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? TaxCode { get; set; }
    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }
}
