using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;

/// <summary>
/// One-time password codes for employee SMS login (stored hashed).
/// </summary>
[Table("EmployeeOtp", Schema = "emp")]
public class EmployeeOtp
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee? Employee { get; set; }

    [Required]
    [StringLength(128)]
    public string CodeHash { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime SentAt { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    [StringLength(45)]
    public string? CreatedByIp { get; set; } = string.Empty;

    [StringLength(32)]
    public string Purpose { get; set; } = "Login";
}
