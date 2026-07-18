using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Order.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class VwInterdict_Order 
    {
        public long Id { get; set; }
        public long RecruitOrderId { get; set; }
        public long EmployeeId { get; set; }
        public string Code { get; set; }
        public Nullable<decimal> SumWageFactors { get; set; }
        public Nullable<short> Serial { get; set; }
        public string CreatorUserName { get; set; }
        public long OrderTypeId { get; set; }
        public string OrderType { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public Nullable<long> AspNetUsersId { get; set; }
        public string AspNetUser { get; set; }
        public string Description { get; set; }
        public Nullable<long> LastInterdictOrderId { get; set; }
        public Nullable<long> CorrectedInterdictOrderId { get; set; }
        public long IssueTypeId { get; set; }
        public string IssueType { get; set; }
        public string ExperienceRecorded { get; set; }
        public string RetiredRecorded { get; set; }
        public string YearRecorded { get; set; }
        public Nullable<int> HistoryOut { get; set; }
        public Nullable<int> HistoryStop { get; set; }
        public Nullable<bool> RetiredFlagOk { get; set; }
        public Nullable<long> MarriageStatusId { get; set; }
        public string MarriageStatus { get; set; }
        public Nullable<short> SponsorshipCount { get; set; }
        public Nullable<byte> YearCoefficient { get; set; }
        public Nullable<long> EducationGradeId { get; set; }
        public string EducationGrade { get; set; }
        public Nullable<long> EffectiveEducationGradeId { get; set; }
        public string EffectiveEducationGrade { get; set; }
        public Nullable<bool> IsWar { get; set; }
        public Nullable<bool> IsCaptivity { get; set; }
        public Nullable<bool> IsBasij { get; set; }
        public Nullable<bool> IsIsar { get; set; }
        public Nullable<float> IsarPercent { get; set; }
        public Nullable<int> WarDuration { get; set; }
        public Nullable<int> CaptivityDuration { get; set; }
        public Nullable<int> BasijDuration { get; set; }
        public Nullable<int> JobDegree { get; set; }
        public Nullable<bool> IsMartyrs { get; set; }
        public Nullable<int> WifeCount { get; set; }
        public Nullable<int> GradScore { get; set; }
        public Nullable<System.DateTime> EmployeeDate { get; set; }
        public string ApproverSignatureGuid { get; set; }
        public long InsuranceTypeId { get; set; }
        public string InsuranceType { get; set; }
        public string AccountNumber { get; set; }
        public string OtherVeterans { get; set; }
        public Nullable<System.DateTime> ApproverSignatureDate { get; set; }
        public bool IsWomenHead { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string PersonelCode { get; set; }
        public string IdentityNo { get; set; }
        public string NationalNo { get; set; }
        public Nullable<int> DrivingLicenseTypeId { get; set; }
        public string DrivingLicenseType { get; set; }
        public int ChildCount { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public Nullable<long> BirthPlaceId { get; set; }
        public string BirthPlace { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public Nullable<long> EducatioFieldId { get; set; }
        public string EducatioField { get; set; }
        public Nullable<long> EducatioOrientationId { get; set; }
        public string EducatioOrientation { get; set; }
        public Nullable<long> EmpEduID { get; set; }
        public Nullable<long> IssuePlaceId { get; set; }
        public string IssuePlace { get; set; }
        public string OrderReason { get; set; }
        public long PayLocationId { get; set; }
        public string PayLocation { get; set; }
        public long EmployeeTypeId { get; set; }
        public string EmployeeType { get; set; }
        public long EmployeeStatusId { get; set; }
        public string EmployeeStatus { get; set; }
        public long CostCenterId { get; set; }
        public string CostCenter { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string Project { get; set; }
        public Nullable<long> OrganizationUnitId { get; set; }
        public string OrganisationUnit { get; set; }
        public Nullable<long> WorkPlaceId { get; set; }
        public string WorkPlace { get; set; }
        public Nullable<long> OrganisationPositionId { get; set; }
        public string PositionName { get; set; }
        public Nullable<long> OrganizationJobId { get; set; }
        public string JobTitle { get; set; }
    }
}
