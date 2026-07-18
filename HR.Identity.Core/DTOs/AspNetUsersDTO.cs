using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class AspNetUsersDTO : BaseDTO
{
    public long? EmployeeId { get; set; }
    [StringLength(450)]
    public string FirstName { get; set; }
    [StringLength(450)]
    public string LastName { get; set; }
    [StringLength(256)]
    public string? Description { get; set; }
    [StringLength(512)]
    public string? DeactivationReason { get; set; }
    [StringLength(450)]
    public string? NormalizedUserName { get; set; }
    [StringLength(450)]
    public string? NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool Disabled { get; set; }
    public bool clearLastSession { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    [StringLength(450)]
    public string? PhoneNumber { get; set; }
    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; }
    //[DataType(DataType.Password)]
    public int AccessFailedCount { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }
    public string ExpiresOnTime
    {
        get
        {

            if (ExpiresOn == null)
            {
                return "";
            }
            else
            {
                var ExpiresOnUTC = ExpiresOn.Value.AddHours(3).AddMinutes(30);
                return ExpiresOnUTC.Hour.ToString().PadLeft(1, '0') + ":" + ExpiresOnUTC.Minute.ToString().PadLeft(1, '0') + ":" + ExpiresOnUTC.Second.ToString().PadLeft(1, '0');
            }
        }

    }

    [StringLength(1024)]

    public string? pasvord { get; set; }
    [Required]
    [StringLength(450)]
    public string UserName { get; set; }
    [MinLength(10)]
    [MaxLength(10)]
    [Required]
    public string NationalNo { get; set; }

    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastWrongAttemptDatetime { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? PasswordExpirationDate { get; set; }
    [StringLength(45)]
    public string? AllowedIP { get; set; }
    public long? ConfidentialityLevelId { get; set; }
    public string LastLoginDateTime
    {
        get
        {

            if (LastLoginDate == null)
            {
                return "";
            }
            else
            {
                return LastLoginDate.Value.Hour.ToString().PadLeft(1, '0') + ":" + LastLoginDate.Value.Minute.ToString().PadLeft(1, '0') + ":" + LastLoginDate.Value.Second.ToString().PadLeft(1, '0');
            }
        }

    }
}
