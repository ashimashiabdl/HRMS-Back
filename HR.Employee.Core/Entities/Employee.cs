using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;

[Table("Employee", Schema = "emp")]
public class Employee : BaseEntity, IignoreDateRangeValidation
{
    public Employee()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
        title = string.Empty;
    }

    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? FirstName { get; set; } = string.Empty;
    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? LastName { get; set; } = string.Empty;

    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [DisplayName("نام انگلیسی")]
    public string? EnglishFirstName { get; set; } = string.Empty;

    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [DisplayName("نام خانوادگی انگلیسی")]
    public string? EnglishLastName { get; set; } = string.Empty;

    [NotMapped]
    public string? FullName
    {
        get
        { return FirstName + " " + LastName; }
        set
        {

        }
    }
    [StringLength(40)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? FatherName { get; set; } = string.Empty;
    [StringLength(50)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? PersonelCode { get; set; } = string.Empty;

    /// <summary>
    /// ع©ط¯ طھظپط¶غŒظ„غŒ ع©ط§ط±ظ…ظ†ط¯
    /// </summary>
    [StringLength(50)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [DisplayName("ع©ط¯ طھظپط¶غŒظ„غŒ ع©ط§ط±ظ…ظ†ط¯")]
    public string? AccountingSystemEmployeeId { get; set; } = string.Empty;
    [ForeignKey("Gender")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long? GenderId { get; set; }
    public virtual BaseTableValue? Gender { get; set; }
    [StringLength(15)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? IdentityNo { get; set; } = string.Empty;
    [ForeignKey("BirthPlace")]
    public long? BirthPlaceId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Places? BirthPlace { get; set; }
    [ForeignKey("Religeon")]
    public long? ReligeonId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? Religeon { get; set; }

    [ForeignKey("Mazhab")]

    // Mazhab (Sect) similar to Religeon; BaseTableId = 9
    public long? MazhabId { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? Mazhab { get; set; }
    [StringLength(10)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? NationalNo { get; set; } = string.Empty;
    public long? NationalityId { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? Nationality { get; set; }

    /// <summary>
    /// وضعیت مجوز اتباع — فقط برای ملیت غیر ایرانی معنا دارد.
    /// true = اتباع با مجوز، false = اتباع بدون مجوز، null = ایرانی یا ملیت نامشخص.
    /// </summary>
    public bool? IsAuthorizedForeigner { get; set; }

    public long? CitizenshipId { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? Citizenship { get; set; }
    [ForeignKey("IssuePlace")]
    public long? IssuePlaceId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Places? IssuePlace { get; set; }

    // ط´ظ‡ط± ظ…ط­ظ„ ط®ط¯ظ…طھ
    [ForeignKey("ServicePlace")]
    public long? ServicePlaceId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Places? ServicePlace { get; set; }
    [Column(TypeName = "date")]
    public DateTime? BirthDate { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? BloodGroup { get; set; }
    [StringLength(50)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? ActiveName { get; set; } = string.Empty;
    [StringLength(500)]
    public string? Descriptions { get; set; } = string.Empty;
    public long? PersonId { get; set; }
    public int? PrivateJobStatus { get; set; } = 0;
    
    [Column(TypeName = "date")]
    public DateTime? IssueDate { get; set; }

    public int? IssueSerialChar { get; set; } = 0;
    [StringLength(10)]
    public string? IssueSerialString { get; set; } = string.Empty;
    [StringLength(20)]
    public string? IssueSerialOrder { get; set; } = string.Empty;

    [ForeignKey("MaritalStatus")]
    public long? MaritalStatusId { get; set; }
    public virtual BaseTableValue? MaritalStatus { get; set; }

    [StringLength(20)]
    public string? PassportNo { get; set; } = string.Empty;

    [StringLength(20)]
    public string? InOutCard { get; set; } = string.Empty;

    public int? SectId { get; set; } = 0;

    public bool IsActive { get; set; } = false;

    public bool IsVerify { get; set; } = false;

    [ForeignKey("BaseOrganisation")]
    public long? BaseOrganisationId { get; set; }
    public virtual OrganisationChart? BaseOrganisation { get; set; }
    [ForeignKey("SkillLevel")]
    public long? SkillLevelId { get; set; }
    public virtual SkillLevel? SkillLevel { get; set; }

    public bool IsRetired { get; set; } = false;

    public int SubsystemId { get; set; } = 0;

    public int? Imperfective { get; set; } = 0;

    public bool IsWomenHead { get; set; } = false;
    [Column(TypeName = "date")]
    public DateTime? StartWorkDate { get; set; }
    [StringLength(20)]
    public string? LostIssueSerialString { get; set; } = string.Empty;
    
    public long? BloodGroupId { get; set; }


    public long? UseLifeAccidentInsuranceTypeId { get; set; }
    public long? LifeAccidentInsuranceTypeId { get; set; }

    public long? UseCompeleteInsuranceTypeId { get; set; }
    public long? CompeleteInsuranceTypeId { get; set; }
    public bool? IsHekmat { get; set; } = false;
    public bool? IsCashBenefits { get; set; } = false;

    // ظ…ط²ط§غŒط§غŒ ط±ظپط§ظ‡غŒ ط¯ط±غŒط§ظپطھ ظ…غŒ ع©ظ†ط¯
    public bool? IsWelfareBenefits { get; set; } = false;

    // ط¹ظ„طھ ظ¾ط§غŒط§ظ† ظ‡ظ…ع©ط§ط±غŒ
    [StringLength(250)]
    public string? ReleaseReason { get; set; } = string.Empty;

    // طھط§ط±غŒط® ظ¾ط§غŒط§ظ† ظ‡ظ…ع©ط§ط±غŒ
    [Column(TypeName = "date")]
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// تاریخ فوت
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? DeathDate { get; set; }

    /// <summary>
    /// علت فوت
    /// </summary>
    [StringLength(250)]
    public string? DeathCause { get; set; } = string.Empty;

    /// <summary>
    /// ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½
    /// </summary>
    public long? VehicleStatusId { get; set; }

    public bool EmailConfirmed { get; set; } = false;
    public bool Disabled { get; set; } = false;
    public string? PasswordHash { get; set; } = string.Empty;
    public string? SecurityStamp { get; set; } = string.Empty;
    public string? ConcurrencyStamp { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; } = false;
    public bool IsAdmin { get; set; } = false;

    [StringLength(64)]
    public string? PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; } = false;

    public int AccessFailedCount { get; set; } = 0;
    #region Login

    public byte[]? salt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? LastLoginDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastWrongAttemptDatetime { get; set; }

    [ForeignKey("TaxExemptionType")]
    public long? TaxExemptionTypeId { get; set; }
    public virtual HR.BaseInfo.Core.Entities.TaxExemptionType? TaxExemptionType { get; set; }
    #endregion Login

    [NotMapped]
    private new string title { get; set; } = string.Empty;

    // Foreign key to bas.Job
    //[ForeignKey("Job")]
    //public long? JobId { get; set; }
    //public virtual HR.BaseInfo.Core.Entities.Job? Job { get; set; }


    [ForeignKey("ManagementAndStewardshipJob")]
    public long? ManagementAndStewardshipJobId { get; set; }
    public virtual ManagementAndStewardshipJob? ManagementAndStewardshipJob { get; set; }

    /// <summary>
    ///  طµظپ غŒط§ ط³طھط§ط¯
    /// </summary>
    public long? HeadquartersOrRowTypeId { get; set; }
    public virtual BaseTableValue? HeadquartersOrRowType { get; set; }

    [ForeignKey("MartyrRelation")]

    // Martyr relation (BaseTableValue id = 173)
    public long? MartyrRelationId { get; set; }
    public virtual BaseTableValue? MartyrRelation { get; set; }

    /// <summary>
    /// ع©ط¯ ط±ظ‡ع¯غŒط±غŒ ظپط±ط²ظ†ط¯ ط´ظ‡غŒط¯ - ظپظ‚ط· ط²ظ…ط§ظ†غŒ ع©ظ‡ MartyrRelationId ط¨ط±ط§ط¨ط± ط¨ط§ 21627 ط¨ط§ط´ط¯
    /// </summary>
    [StringLength(100)]
    public string? MartyrChildTrackingCode { get; set; } = string.Empty;

    [ForeignKey("TaminInsuranceJobList")]
    public long? TaminInsuranceJobListId { get; set; }
    public virtual TaminInsuranceJobList? TaminInsuranceJobList { get; set; }

    public long? ConfidentialityLevelId { get; set; }
    [ForeignKey("ConfidentialityLevelId")]
    public ConfidentialityLevel? ConfidentialityLevel { get; set; }


}
