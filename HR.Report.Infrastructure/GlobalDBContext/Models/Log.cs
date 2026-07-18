using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

public partial class Log
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string StackTrace { get; set; } = null!;

    public string Exception { get; set; } = null!;

    public string Logger { get; set; } = null!;

    public string Url { get; set; } = null!;

    [Column("IP")]
    public string? Ip { get; set; }

    public string? User { get; set; }

    public bool? Success { get; set; }

    public int? StatusCode { get; set; }

    [StringLength(10)]
    public string? Method { get; set; }

    [StringLength(512)]
    public string? UserAgent { get; set; }

    public int? Port { get; set; }

    public int? DurationMs { get; set; }
}
