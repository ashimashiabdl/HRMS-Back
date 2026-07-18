using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Order.Core.Data;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.Organisation.Core.Entities;
using Hr.SystemSetting.Core.DTOs;
using System.Text.Json.Serialization;

namespace HR.Order.Core.DTOs
{
    public class InterdictOrderDTO : BaseDTO
    {
        
        public List<InterdictOrderCoefficientItemDTO>? CoefficientItemList { get; set; }
        public List<InterdictOrderWageItemDTO>? WageItemList { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long PayLocationId { get; set; }
        public string? PayLocation { get; set; }

        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string? OrganizationUnit { get; set; }
        public long? WorkPlaceId { get; set; }
        public string? WorkPlace { get; set; }

        public long? ProjectId { get; set; }
        public string? Project { get; set; }

        public long EmployeeStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long? OrganisationPositionId { get; set; }
        public string? OrganisationPosition { get; set; }
        public long RecruitOrderId { get; set; }
        public string? RecruitOrder { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        public decimal? SumWageFactors { get; set; }
        public short? Serial { get; set; }
        [StringLength(50)]
        public string? CreatorUserName { get; set; }

        public long OrderTypeId { get; set; }
        public string? OrderType { get; set; }

        public long StatusId { get; set; }
        public string? Status { get; set; }


        [StringLength(2048)]
        public string? Description { get; set; }

        public long? LastInterdictOrderId { get; set; }
        public string? LastInterdictOrder { get; set; }

        public long? CorrectedInterdictOrderId { get; set; }
        public string? CorrectedInterdictOrder { get; set; }
        public long IssueTypeId { get; set; }
        public string? IssueType { get; set; }



        public long EducatioFieldId { get; set; }
        public string? EducationField { get; set; }


        public long EducatioOrientationId { get; set; }
        public string? EducationOrientation { get; set; }


        public DateTime? BirthDate { get; set; }

        public long EmpEduID { get; set; }
        public string? EmpEdu { get; set; }

        public long? BirthPlaceId { get; set; }
        public string? BirthPlace { get; set; }

        [StringLength(64)]
        public string? DrivingLicenseNumber { get; set; }

        public long? IssuePlaceId { get; set; }
        public string? IssuePlace { get; set; }

        [StringLength(8)]
        public string? ExperienceRecorded { get; set; }
        [StringLength(8)]
        public string? RetiredRecorded { get; set; }
        [StringLength(8)]
        public string? YearRecorded { get; set; }
        public int? HistoryOut { get; set; }

        public int? HistoryStop { get; set; }

        public bool? RetiredFlagOk { get; set; }


        public long? MarriageStatusId { get; set; }
        public string? MarriageStatus { get; set; }

        public short? SponsorshipCount { get; set; }
        public byte? YearCoefficient { get; set; }


        public long? EducationGradeId { get; set; }
        public string? EducationGrade { get; set; }
        public long? EffectiveEducationGradeId { get; set; }
        public string? EffectiveEducationGrade { get; set; }
        public bool? IsWar { get; set; }
        public bool? IsCaptivity { get; set; }
        public bool? IsBasij { get; set; }
        public bool? IsIsar { get; set; }
        public float? IsarPercent { get; set; }
        public int? WarDuration { get; set; }
        public int? CaptivityDuration { get; set; }
        public int? BasijDuration { get; set; }
        public int? JobDegree { get; set; }
        public bool? IsMartyrs { get; set; }

        public int? WifeCount { get; set; }

        public int? GradScore { get; set; }

        public DateTime? EmployeeDate { get; set; }

        public string? ApproverSignatureGuid { get; set; }

        public long InsuranceTypeId { get; set; }
        public string? InsuranceType { get; set; }
        [StringLength(50)]
        public string? AccountNumber { get; set; }
        [StringLength(50)]
        public string? OtherVeterans { get; set; }

        public DateTime? ApproverSignatureDate { get; set; }

        public bool IsWomenHead { get; set; }
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(40)]
        public string? FatherName { get; set; }
        [StringLength(50)]
        public string? PersonelCode { get; set; }
        [StringLength(15)]
        public string? IdentityNo { get; set; }
        [StringLength(10)]
        public string? NationalNo { get; set; }
        public int? DrivingLicenseTypeId { get; set; }
        public int ChildCount { get; set; }
    }



    public class InterdictOrderFlatDTO 
    {
        public long? Id { get; set; }
        public string? title { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ImpleDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }
        public List<InterdictOrderCoefficientItemDTO>? CoefficientItemList { get; set; }
        public List<InterdictOrderWageItemDTO>? WageItemList { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long PayLocationId { get; set; }
        public string? PayLocation { get; set; }

        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string? OrganizationUnit { get; set; }
        public long? WorkPlaceId { get; set; }
        public string? WorkPlace { get; set; }

        public long? ProjectId { get; set; }
        public string? Project { get; set; }

        public long EmployeeStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long? OrganisationPositionId { get; set; }
        public string? OrganisationPosition { get; set; }
        public long RecruitOrderId { get; set; }
        public string? RecruitOrder { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        public decimal? SumWageFactors { get; set; }
        public short? Serial { get; set; }
        [StringLength(50)]
        public string? CreatorUserName { get; set; }

        public long OrderTypeId { get; set; }
        public string? OrderType { get; set; }

        public long StatusId { get; set; }
        public string? Status { get; set; }


        [StringLength(2048)]
        public string? Description { get; set; }

        public long? LastInterdictOrderId { get; set; }
        public string? LastInterdictOrder { get; set; }

        public long? CorrectedInterdictOrderId { get; set; }
        public string? CorrectedInterdictOrder { get; set; }
        public long IssueTypeId { get; set; }
        public string? IssueType { get; set; }



        public long EducatioFieldId { get; set; }
        public string? EducationField { get; set; }


        public long EducatioOrientationId { get; set; }
        public string? EducationOrientation { get; set; }


        public DateTime? BirthDate { get; set; }

        public long EmpEduID { get; set; }
        public string? EmpEdu { get; set; }

        public long? BirthPlaceId { get; set; }
        public string? BirthPlace { get; set; }

        [StringLength(64)]
        public string? DrivingLicenseNumber { get; set; }

        public long? IssuePlaceId { get; set; }
        public string? IssuePlace { get; set; }

        [StringLength(8)]
        public string? ExperienceRecorded { get; set; }
        [StringLength(8)]
        public string? RetiredRecorded { get; set; }
        [StringLength(8)]
        public string? YearRecorded { get; set; }
        public int? HistoryOut { get; set; }

        public int? HistoryStop { get; set; }

        public bool? RetiredFlagOk { get; set; }


        public long? MarriageStatusId { get; set; }
        public string? MarriageStatus { get; set; }

        public short? SponsorshipCount { get; set; }
        public byte? YearCoefficient { get; set; }


        public long? EducationGradeId { get; set; }
        public string? EducationGrade { get; set; }
        public long? EffectiveEducationGradeId { get; set; }
        public string? EffectiveEducationGrade { get; set; }
        public bool? IsWar { get; set; }
        public bool? IsCaptivity { get; set; }
        public bool? IsBasij { get; set; }
        public bool? IsIsar { get; set; }
        public float? IsarPercent { get; set; }
        public int? WarDuration { get; set; }
        public int? CaptivityDuration { get; set; }
        public int? BasijDuration { get; set; }
        public int? JobDegree { get; set; }
        public bool? IsMartyrs { get; set; }

        public int? WifeCount { get; set; }

        public int? GradScore { get; set; }

        public DateTime? EmployeeDate { get; set; }

        public string? ApproverSignatureGuid { get; set; }

        public long InsuranceTypeId { get; set; }
        public string? InsuranceType { get; set; }
        [StringLength(50)]
        public string? AccountNumber { get; set; }
        [StringLength(50)]
        public string? OtherVeterans { get; set; }

        public DateTime? ApproverSignatureDate { get; set; }

        public bool IsWomenHead { get; set; }
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(40)]
        public string? FatherName { get; set; }
        [StringLength(50)]
        public string? PersonelCode { get; set; }
        [StringLength(15)]
        public string? IdentityNo { get; set; }
        [StringLength(10)]
        public string? NationalNo { get; set; }
        public int? DrivingLicenseTypeId { get; set; }
        public int ChildCount { get; set; }
    }
}
