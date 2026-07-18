using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("EmployeeRefreshToken", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "RevokedAt", "ExpiresAt", Name = "IX_EmployeeRefreshToken_EmployeeId_RevokedAt_ExpiresAt")]
public partial class EmployeeRefreshToken
{
    [Key]
    public long Id { get; set; }

    [StringLength(512)]
    public string Token { get; set; } = null!;

    public long EmployeeId { get; set; }

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

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeRefreshTokens")]
    public virtual Employee Employee { get; set; } = null!;
}
