using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("SystemGuide", Schema = "bas")]
public class SystemGuide : BaseEntity, IignoreDateRangeValidation
{


    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Required(ErrorMessage = "محتوا الزامی است")]
    [Column(TypeName = "nvarchar(MAX)")]
    public string Body { get; set; } = string.Empty;
}
