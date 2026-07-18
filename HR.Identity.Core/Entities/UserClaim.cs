using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("User_Claim", Schema = "Identity")]
public class UserClaim : IdentityUserClaim<long>
{
    public AspNetUsers User { get; set; }
    public bool IsExcluded { get; set; }
    [MaxLength(1024)]
    public string? Claim { get; set; }
}
