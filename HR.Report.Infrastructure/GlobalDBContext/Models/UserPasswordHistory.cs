using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Password_History", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("AspNetUserId", Name = "IX_User_Password_History_AspNetUserId")]
public partial class UserPasswordHistory
{
    [Key]
    public long Id { get; set; }

    public long AspNetUserId { get; set; }

    [StringLength(512)]
    public string PasswordHash { get; set; } = null!;

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

    [MaxLength(64)]
    public byte[]? Salt { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("AspNetUserId")]
    [InverseProperty("UserPasswordHistories")]
    public virtual AspNetUser AspNetUser { get; set; } = null!;
}
