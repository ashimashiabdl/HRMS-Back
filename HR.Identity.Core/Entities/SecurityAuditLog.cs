using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

/// <summary>
/// Security audit log for tracking suspicious activities and authentication events
/// </summary>
[Table("Security_Audit_Logs", Schema = "Identity")]
public class SecurityAuditLog
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// User ID (nullable for failed login attempts where user may not exist)
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Username or attempted username
    /// </summary>
    [Required]
    [StringLength(256)]
    public string UserName { get; set; }

    /// <summary>
    /// Type of security activity (e.g., TokenIPMismatch, SuccessfulLogin, etc.)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ActivityType { get; set; }

    /// <summary>
    /// IP address from which the activity originated
    /// </summary>
    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; }

    /// <summary>
    /// User-Agent string of the client
    /// </summary>
    [StringLength(500)]
    public string UserAgent { get; set; }

    /// <summary>
    /// Detailed information about the activity
    /// </summary>
    [StringLength(2000)]
    public string Details { get; set; }

    /// <summary>
    /// API endpoint that was accessed (if applicable)
    /// </summary>
    [StringLength(500)]
    public string Endpoint { get; set; }

    /// <summary>
    /// When the activity occurred
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Severity level: Info, Low, Medium, High, Critical
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Severity { get; set; }

    /// <summary>
    /// Additional metadata in JSON format (optional; stored as empty string when not used)
    /// </summary>
    [StringLength(4000)]
    public string Metadata { get; set; } = "";
}

