using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class FamilyDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        [StringLength(100)]
        public string FirstName { get; set; } = null!;
        [StringLength(100)]
        public string LastName { get; set; } = null!;
        [StringLength(100)]
        public string? FatherName { get; set; }
        public long? GenderTypeId { get; set; }
        public string? GenderType { get; set; }
        public long? RelationshipTypeId { get; set; }
        public string? RelationshipType { get; set; }
        public long? SponsorshipTypeId { get; set; }
        public string? SponsorshipType { get; set; }
        public long? DependentTypeId { get; set; }
        public string? DependentType { get; set; }
        public long? SpecialStatusId { get; set; }
        public string? SpecialStatus { get; set; }
        public DateTime? BirthDate { get; set; }
        [StringLength(10)]
        public string? NationalNo { get; set; }
        public decimal? EffectivePercent { get; set; }
        public float? MaintenanceCost { get; set; }
        public DateTime? MaintenanceEndDate { get; set; }
        public long? UseCompeleteInsuranceTypeId { get; set; }
        public string? UseCompeleteInsuranceType { get; set; }
        public long? RemedialInsuranceTypeId { get; set; }
        public string? RemedialInsuranceType { get; set; }
        public long? CompeleteInsuranceTypeId { get; set; }
        public string? CompeleteInsuranceType { get; set; }
        public long? InsuranceTypeId { get; set; }
        public string? InsuranceType { get; set; }
        public long? UseLifeAccidentInsuranceTypeId { get; set; }
        public string? UseLifeAccidentInsuranceType { get; set; }
        public long? LifeAccidentInsuranceTypeId { get; set; }
        public string? LifeAccidentInsuranceType { get; set; }
        [StringLength(10)]
        public string? IdentityNo { get; set; }
        [StringLength(30)]
        public string? AccountNumber { get; set; }
        public long? JobId { get; set; }
        public string? Job { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public long? MaritalStatusId { get; set; }
        public string? MaritalStatus { get; set; }
        public long? EducationFieldId { get; set; }
        public string? EducationField { get; set; }
        public long? EducationOrientationId { get; set; }
        public string? EducationOrientation { get; set; }
        public long? EducationGroupId { get; set; }
        public long? EducationGradeId { get; set; }
        public string? EducationGrade { get; set; }
        public bool? IsImperfective { get; set; }
        public long? BirthPlaceId { get; set; }
        public string? BirthPlace { get; set; }
        public bool? IsLast { get; set; }
        public bool IsVerify { get; set; }
        public bool? UsedinOrder { get; set; }
        public bool? IsPremierStudent { get; set; }
        public bool? IsDependent { get; set; }
        public long? SpecialDiseaseLevelTypeId { get; set; }
        public string? SpecialDiseaseLevelType { get; set; }
        public DateTime? ImperfectiveStartDate { get; set; }
        public bool? IsCoveredInsurance { get; set; }
        public bool? IsHekmat { get; set; }
        public bool? IsCashBenefits { get; set; }
        public bool? IsWelfareServices { get; set; }
        public DateTime? MarriageDate { get; set; }
        public int? DisabilityPercent { get; set; }
        public DateTime? DisabilityEndDate { get; set; }
        public long? DisabilityTypeId { get; set; }
        public string? DisabilityType { get; set; }
        public bool? HasCertification { get; set; }
        public DateTime? MarriageEndDate { get; set; }
        [StringLength(128)]
        public string? InsuranceNumber { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
