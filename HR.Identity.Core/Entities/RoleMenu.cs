
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("Role_Menu", Schema = "Identity")]
public class RoleMenu : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("Role")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long RoleId { get; set; }
    public virtual AspNetRoles? Role { get; set; }

    [MaxLength(1024)]
    public string? Claim { get; set; }
}
