using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("AspNetUsers", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("ConfidentialityLevelId", Name = "IX_AspNetUsers_ConfidentialityLevelId")]
[Microsoft.EntityFrameworkCore.Index("NationalNo", Name = "IX_AspNetUsers_NationalNo", IsUnique = true)]
public partial class AspNetUser
{
    [Key]
    public long Id { get; set; }

    public long? EmployeeId { get; set; }

    [StringLength(450)]
    public string FirstName { get; set; } = null!;

    public string NationalNo { get; set; } = null!;

    [StringLength(450)]
    public string LastName { get; set; } = null!;

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastWrongAttemptDatetime { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    [Column("AllowedIP")]
    [StringLength(45)]
    public string? AllowedIp { get; set; }

    public bool MustChangePassword { get; set; }

    [Column("salt")]
    public byte[]? Salt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PasswordExpirationDate { get; set; }

    public long? ConfidentialityLevelId { get; set; }

    [StringLength(512)]
    public string? DeactivationReason { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<BatchPayRollRequest> BatchPayRollRequests { get; set; } = new List<BatchPayRollRequest>();

    [InverseProperty("AspNetUsers")]
    public virtual ICollection<BatchRequest> BatchRequests { get; set; } = new List<BatchRequest>();

    [ForeignKey("ConfidentialityLevelId")]
    [InverseProperty("AspNetUsers")]
    public virtual ConfidentialityLevel? ConfidentialityLevel { get; set; }

    [InverseProperty("AspNetUsers")]
    public virtual ICollection<InterdictOrder> InterdictOrders { get; set; } = new List<InterdictOrder>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<LoginCredentialLog> LoginCredentialLogs { get; set; } = new List<LoginCredentialLog>();

    [InverseProperty("Receiver")]
    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    [InverseProperty("Sender")]
    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    [InverseProperty("Employee")]
    public virtual ICollection<NodeUserRel> NodeUserRels { get; set; } = new List<NodeUserRel>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<PasswordChangeRateLimit> PasswordChangeRateLimits { get; set; } = new List<PasswordChangeRateLimit>();

    [InverseProperty("AspNetUsers")]
    public virtual ICollection<PersonnelFunctionExcelFile> PersonnelFunctionExcelFiles { get; set; } = new List<PersonnelFunctionExcelFile>();

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("User")]
    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    [InverseProperty("User")]
    public virtual ICollection<UserCostCenter> UserCostCenters { get; set; } = new List<UserCostCenter>();

    [InverseProperty("User")]
    public virtual ICollection<UserDefaultSetting> UserDefaultSettings { get; set; } = new List<UserDefaultSetting>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; } = new List<UserLoginHistory>();

    [InverseProperty("User")]
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();

    [InverseProperty("User")]
    public virtual ICollection<UserOrganizationUnit> UserOrganizationUnits { get; set; } = new List<UserOrganizationUnit>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<UserPasswordHistory> UserPasswordHistories { get; set; } = new List<UserPasswordHistory>();

    [InverseProperty("User")]
    public virtual ICollection<UserPayLocation> UserPayLocations { get; set; } = new List<UserPayLocation>();

    [InverseProperty("User")]
    public virtual ICollection<UserReportableEntity> UserReportableEntities { get; set; } = new List<UserReportableEntity>();

    [InverseProperty("User")]
    public virtual ICollection<UserReport> UserReports { get; set; } = new List<UserReport>();

    [InverseProperty("AspNetUsers")]
    public virtual ICollection<UserSignature> UserSignatures { get; set; } = new List<UserSignature>();

    [InverseProperty("User")]
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

    [InverseProperty("User")]
    public virtual ICollection<UserWorkPlace> UserWorkPlaces { get; set; } = new List<UserWorkPlace>();
}
