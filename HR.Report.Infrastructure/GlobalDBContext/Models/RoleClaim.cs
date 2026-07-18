using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Role_Claim", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("RoleId", Name = "IX_Role_Claim_RoleId")]
public partial class RoleClaim
{
    [Key]
    public int Id { get; set; }

    public long RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("RoleClaims")]
    public virtual AspNetRole Role { get; set; } = null!;
}
