using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities;

[Table("Family", Schema = "emp")]
public class Family : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation 
{
        public Family()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
    public long? OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string LastName { get; set; } = string.Empty;
    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? FatherName { get; set; } = string.Empty;
    public long? GenderTypeId { get; set; }
    public long? RelationshipTypeId { get; set; }
    public long? SponsorshipTypeId { get; set; }
    public long? DependentTypeId { get; set; }
    public long? SpecialStatusId { get; set; }
    [Column(TypeName = "date")]
    public DateTime? BirthDate { get; set; }
    [StringLength(10)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? NationalNo { get; set; } = string.Empty;
    public decimal? EffectivePercent { get; set; } = 0m;
    public float? MaintenanceCost { get; set; } = 0f;
    [Column(TypeName = "date")]
    public DateTime? MaintenanceEndDate { get; set; }
    public long? RemedialInsuranceTypeId { get; set; }
    public long? UseCompeleteInsuranceTypeId { get; set; }
    public long? CompeleteInsuranceTypeId { get; set; }
    public long? InsuranceTypeId { get; set; }
    public long? UseLifeAccidentInsuranceTypeId { get; set; }
    public long? LifeAccidentInsuranceTypeId { get; set; }
    [StringLength(10)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? IdentityNo { get; set; } = string.Empty;
    [StringLength(30)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? AccountNumber { get; set; } = string.Empty;
    public long? JobId { get; set; }
    [StringLength(500)]
    public string? Description { get; set; } = string.Empty;
    public long? MaritalStatusId { get; set; }
    [ForeignKey("EducationField")]
    public long? EducationFieldId { get; set; }
    public virtual EducationField? EducationField { get; set; }
    [ForeignKey("EducationOrientation")]
    public long? EducationOrientationId { get; set; }
    public virtual EducationOrientation? EducationOrientation { get; set; }
    [ForeignKey("EducationGrade")]
    public long? EducationGradeId { get; set; }
    public virtual EducationGrade? EducationGrade { get; set; }
    public bool? IsImperfective { get; set; } = false;
    [ForeignKey("BirthPlace")]
    public long? BirthPlaceId { get; set; }
    public virtual Places? BirthPlace { get; set; }
    public bool? IsLast { get; set; } = false;
    public bool IsVerify { get; set; } = false;
    public bool? UsedinOrder { get; set; } = false;
    public bool? IsPremierStudent { get; set; } = false;
    public bool? IsDependent { get; set; } = false;
    public long? SpecialDiseaseLevelTypeId { get; set; }
    [Column(TypeName = "date")]
    public DateTime? ImperfectiveStartDate { get; set; }
    public bool? IsCoveredInsurance { get; set; } = false;
    // ط­ع©ظ…طھ ظ…غŒع¯غŒط±ط¯
    public bool? IsHekmat { get; set; } = false;
    // ظ…ط²ط§غŒط§غŒ ظ†ظ‚ط¯غŒ ظ…غŒع¯غŒط±ط¯
    public bool? IsCashBenefits { get; set; } = false;
    // ط®ط¯ظ…ط§طھ ط±ظپط§ظ‡غŒ ظ…غŒع¯غŒط±ط¯طں
    public bool? IsWelfareServices { get; set; } = false;
    [Column(TypeName = "date")]
    public DateTime? MarriageDate { get; set; }
    public int? DisabilityPercent { get; set; } = 0;
    [Column(TypeName = "date")]
    public DateTime? DisabilityEndDate { get; set; }
    public long? DisabilityTypeId { get; set; }
    public bool? HasCertification { get; set; } = false;
    [Column(TypeName = "date")]
    public DateTime? MarriageEndDate { get; set; }
    [StringLength(128)]
    public string? InsuranceNumber { get; set; } = string.Empty;
    [NotMapped]
    private new string title { get; set; } = string.Empty;
}
