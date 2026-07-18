using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Claim", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_Claim_UserId")]
public partial class UserClaim
{
    [Key]
    public int Id { get; set; }

    public bool IsExcluded { get; set; }

    [StringLength(1024)]
    public string? Claim { get; set; }

    public long UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserClaims")]
    public virtual AspNetUser User { get; set; } = null!;
}
