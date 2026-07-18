using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("RequestDocumentRequirement", Schema = "bas")]
public class RequestDocumentRequirement : BaseEntity, IignoreDateRangeValidation
{
    public bool IsActive { get; set; } = true;

    [StringLength(1000)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }

    public virtual ICollection<RequestDocumentRequirementDetail> Details { get; set; } = new List<RequestDocumentRequirementDetail>();
}
