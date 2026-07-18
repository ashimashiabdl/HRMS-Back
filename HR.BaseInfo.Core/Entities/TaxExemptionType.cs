using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    [Table("Tax_Exemption_Type", Schema = "bas")]
    public class TaxExemptionType : BaseEntity
    {
        [StringLength(450)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
    }
}


