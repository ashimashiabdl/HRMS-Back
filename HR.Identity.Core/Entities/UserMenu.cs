using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("User_Menu", Schema = "Identity")]
public class UserMenu : BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("User")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }

    [MaxLength(1024)]
    public string? Claim { get; set; }
}
