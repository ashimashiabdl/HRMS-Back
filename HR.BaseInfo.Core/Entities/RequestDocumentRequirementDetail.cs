using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("RequestDocumentRequirementDetail", Schema = "bas")]
public class RequestDocumentRequirementDetail : BaseEntity, IignoreDateRangeValidation
{
    [Required(ErrorMessage = "شناسه نوع سند الزامی می باشد")]
    public long RequestDocumentRequirementId { get; set; }

    public bool IsRequired { get; set; }

    [StringLength(1000)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }

    [ForeignKey("RequestDocumentRequirementId")]
    public virtual RequestDocumentRequirement? RequestDocumentRequirement { get; set; }
}
