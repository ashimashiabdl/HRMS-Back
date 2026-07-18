using HR.SharedKernel.Attribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Tax_Occupation", Schema = "bas")]
    public class TaxOccupation : HR.SharedKernel.Data.BaseEntity
    {
        [StringLength(450)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
    }
}


