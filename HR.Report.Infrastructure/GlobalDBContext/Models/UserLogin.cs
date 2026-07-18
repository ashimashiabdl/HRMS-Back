using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[PrimaryKey("LoginProvider", "ProviderKey")]
[Table("User_Login", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_Login_UserId")]
public partial class UserLogin
{
    [Key]
    public string LoginProvider { get; set; } = null!;

    [Key]
    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserLogins")]
    public virtual AspNetUser User { get; set; } = null!;
}
