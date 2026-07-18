using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;

/// <summary>
/// Refresh token for employee portal authentication.
/// </summary>
[Table("EmployeeRefreshToken", Schema = "emp")]
public class EmployeeRefreshToken
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(512)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee? Employee { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RevokedAt { get; set; }

    [StringLength(45)]
    public string? CreatedByIp { get; set; } = string.Empty;

    [StringLength(45)]
    public string? RevokedByIp { get; set; } = string.Empty;

    [StringLength(512)]
    public string? ReplacedByToken { get; set; } = string.Empty;

    [StringLength(256)]
    public string? RevocationReason { get; set; } = string.Empty;

    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
