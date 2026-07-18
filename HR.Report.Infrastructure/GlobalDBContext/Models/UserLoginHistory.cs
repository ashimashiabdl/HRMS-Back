using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Login_History", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("AspNetUserId", Name = "IX_User_Login_History_AspNetUserId")]
[Microsoft.EntityFrameworkCore.Index("AspNetUserId", "IsSuccess", "CreateDate", Name = "IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate", IsDescending = new[] { false, false, true })]
public partial class UserLoginHistory
{
    [Key]
    public long Id { get; set; }

    public bool IsSuccess { get; set; }

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

    public long? AspNetUserId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    [StringLength(2048)]
    public string? FailReason { get; set; }

    public long? EmployeeId { get; set; }

    [ForeignKey("AspNetUserId")]
    [InverseProperty("UserLoginHistories")]
    public virtual AspNetUser? AspNetUser { get; set; }
}
