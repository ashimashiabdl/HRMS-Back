using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Security_Audit_Logs", Schema = "Identity")]
public partial class SecurityAuditLog
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [StringLength(100)]
    public string ActivityType { get; set; } = null!;

    [StringLength(45)]
    public string IpAddress { get; set; } = null!;

    [StringLength(500)]
    public string UserAgent { get; set; } = null!;

    [StringLength(2000)]
    public string Details { get; set; } = null!;

    [StringLength(500)]
    public string Endpoint { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    [StringLength(20)]
    public string Severity { get; set; } = null!;

    [StringLength(4000)]
    public string Metadata { get; set; } = null!;
}
