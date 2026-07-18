using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Login_Credential_Log", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("AspNetUserId", Name = "IX_Login_Credential_Log_AspNetUserId")]
public partial class LoginCredentialLog
{
    [Key]
    public long Id { get; set; }

    [StringLength(2000)]
    public string EncryptedUsername { get; set; } = null!;

    [StringLength(2000)]
    public string EncryptedPassword { get; set; } = null!;

    [StringLength(500)]
    public string? UserAgent { get; set; }

    public long? AspNetUserId { get; set; }

    public bool IsSuccess { get; set; }

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
    [InverseProperty("LoginCredentialLogs")]
    public virtual AspNetUser? AspNetUser { get; set; }
}
