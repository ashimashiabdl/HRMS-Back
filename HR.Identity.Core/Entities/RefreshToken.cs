using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

/// <summary>
/// Refresh Token entity for implementing secure token refresh mechanism
/// </summary>
[Table("RefreshTokens", Schema = "Identity")]
public class RefreshToken
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// The actual refresh token string (hashed for security)
    /// </summary>
    [Required]
    [StringLength(512)]
    public string Token { get; set; }

    /// <summary>
    /// Foreign key to AspNetUsers
    /// </summary>
    [Required]
    public long UserId { get; set; }

    /// <summary>
    /// Navigation property to user
    /// </summary>
    [ForeignKey("UserId")]
    public AspNetUsers User { get; set; }

    /// <summary>
    /// When this refresh token was created
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this refresh token expires
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When this refresh token was revoked (null if still active)
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// IP address that created this token
    /// </summary>
    [StringLength(45)]
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// IP address that revoked this token
    /// </summary>
    [StringLength(45)]
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Token that replaced this one (when refreshed)
    /// </summary>
    [StringLength(512)]
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Security stamp at token creation — invalidates refresh when user logs in elsewhere or changes password.
    /// </summary>
    [StringLength(256)]
    public string? SecurityStamp { get; set; }

    /// <summary>
    /// Reason for revocation
    /// </summary>
    [StringLength(256)]
    public string? RevocationReason { get; set; }

    /// <summary>
    /// Check if this refresh token is currently active
    /// </summary>
    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Check if this refresh token has expired
    /// </summary>
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}

