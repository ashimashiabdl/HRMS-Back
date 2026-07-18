using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.DTOs;

public class EmployeeDTO : BaseDTO
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EnglishFirstName { get; set; }

    public string? EnglishLastName { get; set; }

    public string? FatherName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PersonelCode { get; set; }

    /// <summary>
    /// کد تفضیلی کارمند
    /// </summary>
    public string? AccountingSystemEmployeeId { get; set; }

    public long? GenderId { get; set; }
    public long? EmployeeId
    {
        get
        {
            return this.Id;
        }
    }
    public string? GenderTitle { get; set; }
    public string? InOutCard { get; set; }
    public string? IdentityNo { get; set; }

    public long? BirthPlaceId { get; set; }
    public string? BirthPlaceTitle { get; set; }
    /// <summary>
    ///  صف یا ستاد
    /// </summary>
    public bool IsHeadquartersOrRow { get; set; }
    public long? ReligeonId { get; set; }
    public string? ReligeonTitle { get; set; }
    public long? MazhabId { get; set; }
    public string? MazhabTitle { get; set; }
    public string? NationalNo { get; set; }

    public long? NationalityId { get; set; }
    public string? NationalityTitle { get; set; }

    /// <summary>
    /// وضعیت مجوز اتباع — فقط برای ملیت غیر ایرانی معنا دارد.
    /// true = اتباع با مجوز، false = اتباع بدون مجوز، null = ایرانی یا ملیت نامشخص.
    /// </summary>
    public bool? IsAuthorizedForeigner { get; set; }

    public long? CitizenshipId { get; set; }
    public string? CitizenshipTitle { get; set; }

    public long? IssuePlaceId { get; set; }
    public string? IssuePlaceTitle { get; set; }
    public long? ServicePlaceId { get; set; }
    public string? ServicePlaceTitle { get; set; }
    public DateTime? BirthDate { get; set; }

    public long? BloodGroupId { get; set; }
    public string? BloodGroupTitle { get; set; }
    public string? ActiveName { get; set; }
    public string? Descriptions { get; set; }
    public long? PersonId { get; set; }
    public long? UseLifeAccidentInsuranceTypeId { get; set; }
    public long? LifeAccidentInsuranceTypeId { get; set; }
    public long? UseCompeleteInsuranceTypeId { get; set; }
    public long? CompeleteInsuranceTypeId { get; set; }
    public bool? IsHekmat { get; set; }
    public bool? IsCashBenefits { get; set; }
    public bool? IsWelfareBenefits { get; set; }
    public int? PrivateJobStatus { get; set; }
    
    public DateTime? IssueDate { get; set; }
    public int? IssueSerialChar { get; set; }
    public string? IssueSerialString { get; set; }
    public string? IssueSerialOrder { get; set; }
    public int? MaritalStatusId { get; set; }
    public string? PassportNo { get; set; }

    public int? SectId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVerify { get; set; }


    public long? BaseOrganisationId { get; set; }
    public string? BaseOrganisationTitle { get; set; }

    public long? SkillLevelId { get; set; }
    public string? SkillLevelTitle { get; set; }

    public bool? IsRetired { get; set; }

    public int? SubsystemId { get; set; }

    public int? Imperfective { get; set; }

    public bool? IsWomenHead { get; set; }

    public DateTime? StartWorkDate { get; set; }

    public string? LostIssueSerialString { get; set; }



    /// <summary>
    /// وضعیت وسیله نقلیه سازمانی
    /// </summary>
    public long? VehicleStatusId { get; set; }


    [Column(TypeName = "datetime")]
    public DateTime? LastLoginDate { get; set; }


    // علت پایان همکاری
    public string? ReleaseReason { get; set; }

    // تاریخ پایان همکاری
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// تاریخ فوت
    /// </summary>
    public DateTime? DeathDate { get; set; }

    /// <summary>
    /// علت فوت
    /// </summary>
    public string? DeathCause { get; set; }

    public long? TaxExemptionTypeId { get; set; }
    public string? TaxExemptionTypeTitle { get; set; }

    // Job relation (bas.Job)
    public long? JobId { get; set; }
    public string? JobTitle { get; set; }

    // Martyr relation
    public long? MartyrRelationId { get; set; }
    public string? MartyrRelationTitle { get; set; }

    /// <summary>
    /// کد رهگیری فرزند شهید - فقط زمانی که MartyrRelationId برابر با 21627 باشد
    /// </summary>
    public string? MartyrChildTrackingCode { get; set; }

    public long? ManagementAndStewardshipJobId { get; set; }
    public string? ManagementAndStewardshipJob { get; set; }

    /// <summary>
    ///  صف یا ستاد
    /// </summary>
    public long? HeadquartersOrRowTypeId { get; set; }
    public string? HeadquartersOrRowType { get; set; }

    public long? TaminInsuranceJobListId { get; set; }
    public string? TaminInsuranceJobListTitle { get; set; }
    public long? ConfidentialityLevelId { get; set; }
}
