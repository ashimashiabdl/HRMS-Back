using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("PasswordChangeRateLimit", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("AspNetUserId", Name = "IX_PasswordChangeRateLimit_AspNetUserId")]
public partial class PasswordChangeRateLimit
{
    [Key]
    public long Id { get; set; }

    public long AspNetUserId { get; set; }

    [Column("RequestIPAddress")]
    [StringLength(45)]
    public string RequestIpaddress { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime RequestDateTime { get; set; }

    public bool IsSuccess { get; set; }

    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [ForeignKey("AspNetUserId")]
    [InverseProperty("PasswordChangeRateLimits")]
    public virtual AspNetUser AspNetUser { get; set; } = null!;
}
