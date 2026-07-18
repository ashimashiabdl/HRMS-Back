using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("LeaveType", Schema = "bas")]
public class LeaveType : BaseEntity
{
    public int? Duration { get; set; }

    public bool IsPaid { get; set; }

    [StringLength(256)]
    public string? PaymentReference { get; set; }

    [StringLength(256)]
    public string? LegalArticle { get; set; }

    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }
}


