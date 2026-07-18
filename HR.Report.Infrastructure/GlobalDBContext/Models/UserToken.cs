using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Token", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_Token_UserId")]
public partial class UserToken
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserTokens")]
    public virtual AspNetUser User { get; set; } = null!;
}
