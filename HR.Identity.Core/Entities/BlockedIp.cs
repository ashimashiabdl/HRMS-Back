using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("BlockedIp", Schema = "Identity")]
public class BlockedIp : BaseEntity, IignoreDateRangeValidation
{
    [StringLength(45)]
    [Required(ErrorMessage = "آدرس IP الزامی می باشد")]
    [Column("BlockedIpAddress")]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string IpAddress { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true;
}

