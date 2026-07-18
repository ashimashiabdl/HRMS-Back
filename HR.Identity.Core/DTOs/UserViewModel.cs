using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs;

public class RegisterUserDTO : BaseDTO
{
    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? pasvord { get; set; }
 
    [MaxLength(11)]

    public string PhoneNumber { get; set; }

    [Required]
    [MinLength(6)]
    public string UserName { get; set; }
    [MinLength(10)]
    [MaxLength(10)]
    [Required]
    public string NationalNo { get; set; }
    
    public bool Disabled { get; set; }
    public int AccessFailedCount { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public long? EmployeeId { get; set; }
    
    [StringLength(150)]
    public string FirstName { get; set; }

    [StringLength(150)]
    public string LastName { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    [StringLength(45)]
    public string? AllowedIP { get; set; }

    public DateTime? PasswordExpirationDate { get; set; }
    public long? ConfidentialityLevelId { get; set; }

    [StringLength(512)]
    public string? DeactivationReason { get; set; }
    
    /// <summary>
    /// کلمه عبور فعلی کاربر برای تأیید هویت قبل از انجام عملیات حساس
    /// </summary>
    public string? CurrentPassword { get; set; }
}

public class UserViewModel
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Id { get; set; }
}

/// <summary>
/// DTO برای تأیید هویت دسترسی به فهرست کاربران
/// </summary>
public class VerifyUsersListAccessDTO
{
    /// <summary>
    /// کلمه عبور فعلی کاربر برای تأیید هویت (به صورت encrypted)
    /// </summary>
    [Required]
    public string CurrentPassword { get; set; }
}