using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

public partial class AuditLog
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public string? OldValues { get; set; }

    public string NewValues { get; set; } = null!;

    public string? AffectedColumns { get; set; }

    public string PrimaryKey { get; set; } = null!;

    [Column("IP")]
    public string? Ip { get; set; }
}
