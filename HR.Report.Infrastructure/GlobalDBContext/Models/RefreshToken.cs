using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("RefreshTokens", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("Token", Name = "IX_RefreshTokens_Token")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_RefreshTokens_UserId")]
public partial class RefreshToken
{
    [Key]
    public long Id { get; set; }

    [StringLength(512)]
    public string Token { get; set; } = null!;

    public long UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RevokedAt { get; set; }

    [StringLength(45)]
    public string? CreatedByIp { get; set; }

    [StringLength(45)]
    public string? RevokedByIp { get; set; }

    [StringLength(512)]
    public string? ReplacedByToken { get; set; }

    [StringLength(256)]
    public string? RevocationReason { get; set; }

    [StringLength(256)]
    public string? SecurityStamp { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RefreshTokens")]
    public virtual AspNetUser User { get; set; } = null!;
}
