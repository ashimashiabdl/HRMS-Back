using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Core.Entity;

[Table("Employee_Property", Schema = "rpt")]
public class EmployeeProperty : BaseEntity, IignoreDateRangeValidation
{


    #region Employee

    public long? GenderId { get; set; }

    public string? Gender { get; set; }

    public long? ReligeonId { get; set; }

    public string? Religeon { get; set; }

    public long? MazhabId { get; set; }

    public string? Mazhab { get; set; }

    public long? NationalityId { get; set; }

    public string? Nationality { get; set; }

    public long? CitizenshipId { get; set; }

    public string? Citizenship { get; set; }

    public long? ServicePlaceId { get; set; }

    public string? ServicePlace { get; set; }

    public long? BloodGroupId { get; set; }

    public string? BloodGroup { get; set; }

    [StringLength(50)]
    public string? ActiveName { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public long? PersonId { get; set; }

    public int? PrivateJobStatus { get; set; }

    [Column(TypeName = "date")]
    public DateTime? IssueDate { get; set; }

    public DateTime? ValidationCodeSendDate { get; set; }

    public int? IssueSerialChar { get; set; }

    [StringLength(10)]
    public string? IssueSerialString { get; set; }

    [StringLength(20)]
    public string? IssueSerialOrder { get; set; }

    public int? MaritalStatusId { get; set; }

    [StringLength(20)]
    public string? PassportNo { get; set; }

    [StringLength(20)]
    public string? InOutCard { get; set; }

    public int? SectId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVerify { get; set; }

    public long? BaseOrganisationId { get; set; }

    public string? BaseOrganisation { get; set; }

    public long? SkillLevelId { get; set; }

    public string? SkillLevel { get; set; }

    public bool? IsRetired { get; set; }

    public int? SubsystemId { get; set; }

    public int? Imperfective { get; set; }

    [Column(TypeName = "date")]
    public DateTime? StartWorkDate { get; set; }

    [StringLength(20)]
    public string? LostIssueSerialString { get; set; }

    public long? UseLifeAccidentInsuranceTypeId { get; set; }

    public long? LifeAccidentInsuranceTypeId { get; set; }

    public long? UseCompeleteInsuranceTypeId { get; set; }

    public long? CompeleteInsuranceTypeId { get; set; }

    public bool? IsHekmat { get; set; }

    public bool? IsCashBenefits { get; set; }

    public bool? IsWelfareBenefits { get; set; }

    [StringLength(250)]
    public string? ReleaseReason { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ReleaseDate { get; set; }

    public long? VehicleStatusId { get; set; }

    public bool? EmailConfirmed { get; set; }

    public bool? Disabled { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public bool? IsAdmin { get; set; }

    [StringLength(64)]
    public string? PhoneNumber { get; set; }

    public bool? PhoneNumberConfirmed { get; set; }

    public int? AccessFailedCount { get; set; }

    public byte[]? Salt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastWrongAttemptDatetime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }

    [StringLength(1024)]
    public string? LastToken { get; set; }

    public long? TaxExemptionTypeId { get; set; }

    public string? TaxExemptionType { get; set; }

    public long? ManagementAndStewardshipJobId { get; set; }

    public string? ManagementAndStewardshipJob { get; set; }

    public long? HeadquartersOrRowTypeId { get; set; }

    public string? HeadquartersOrRowType { get; set; }

    public long? MartyrRelationId { get; set; }

    public string? MartyrRelation { get; set; }

    [StringLength(100)]
    public string? MartyrChildTrackingCode { get; set; }

    public long? TaminInsuranceJobListId { get; set; }

    public string? TaminInsuranceJobList { get; set; }

    public long? ConfidentialityLevelId { get; set; }

    public string? ConfidentialityLevel { get; set; }

    /// <summary>
    /// کد تفضیلی کارمند
    /// </summary>
    [StringLength(50)]
    [DisplayName("کد تفضیلی کارمند")]
    public string? AccountingSystemEmployeeId { get; set; }

    #endregion Employee

    #region Education

    public long? EducationStateID { get; set; }

    public string? EducationState { get; set; }

    public long? UniversityLevelId { get; set; }

    public string? UniversityLevel { get; set; }

    public long? UniversityTypeID { get; set; }

    public string? UniversityType { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EducationFromDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EducationToDate { get; set; }

    [StringLength(8)]
    public string? EducationAverage { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EducationLicensePresentDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EducationLicenseImplDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EducationLicenseExpireDate { get; set; }

    public bool? IsInDutyTime { get; set; }

    public long? EducationPlacesId { get; set; }

    public string? EducationPlaces { get; set; }

    [StringLength(500)]
    public string? EducationDescriptions { get; set; }

    public bool? IsBoursie { get; set; }

    [StringLength(256)]
    public string? ThesisTitle { get; set; }

    public long? UniversityId { get; set; }

    public string? University { get; set; }

    public bool? IsDefaultEducation { get; set; }

    public bool? IsUsedInOrder { get; set; }

    public bool? SetByEmployee { get; set; }

    public long? LicenceTypeId { get; set; }

    [StringLength(128)]
    public string? LicenceNumber { get; set; }

    [StringLength(128)]
    public string? OtherUniversityName { get; set; }

    public long? KindOfGraduationId { get; set; }

    public long? ThesisGradeTypeId { get; set; }

    public long? ThesisGradeValueId { get; set; }

    #endregion Education

    #region BankAccount

    [StringLength(100)]
    public string? BankAccountNumber { get; set; }

    public long? AccountTypeId { get; set; }

    public string? AccountType { get; set; }

    public int? Priority { get; set; }

    public int? FromPrice { get; set; }

    public int? ToPrice { get; set; }

    public bool? Status { get; set; }

    [StringLength(512)]
    public string? BankAccountDescription { get; set; }

    public long? BankId { get; set; }

    public int? BankBranchId { get; set; }

    public int? OldId { get; set; }

    [StringLength(50)]
    public string? BonCardNumber { get; set; }

    [StringLength(50)]
    public string? CardNumber { get; set; }

    [StringLength(50)]
    public string? ShabaNumber { get; set; }

    #endregion BankAccount

    #region ContactInfo

    public long? AddressTypeId { get; set; }

    public string? AddressType { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(10)]
    public string? Zipcode { get; set; }

    [StringLength(32)]
    public string? Phone { get; set; }

    [StringLength(32)]
    public string? EmergencyPhone { get; set; }

    [StringLength(32)]
    public string? Fax { get; set; }

    [StringLength(128)]
    public string? Mail { get; set; }

    public long? LocationTypeId { get; set; }

    public string? LocationType { get; set; }

    [StringLength(64)]
    public string? MobileNo { get; set; }

    public bool? ContactInfoIsVerify { get; set; }

    public bool? IsLast { get; set; }

    #endregion ContactInfo

    #region Insurance

    public long? InsWorkShopTypeId { get; set; }

    public string? InsWorkShopType { get; set; }

    public long? InsuranceBranchId { get; set; }

    public long? InsuranceInsuranceTypeId { get; set; }

    public int? AccDay { get; set; }

    public bool? InsuranceIsLast { get; set; }

    [StringLength(32)]
    public string? InsuranceNumber { get; set; }

    public bool? HasSupplementaryInsurance { get; set; }

    public long? SupplementaryInsuranceTypeId { get; set; }

    public string? SupplementaryInsuranceType { get; set; }

    public long? InsuranceSubmissionCityId { get; set; }

    public string? InsuranceSubmissionCity { get; set; }

    #endregion Insurance

    #region MilitaryService

    public long? MilitaryStatusCodeId { get; set; }

    public string? MilitaryStatusCode { get; set; }

    public long? ImmunityTypeId { get; set; }

    public string? ImmunityType { get; set; }

    public long? ConfirmerOrganID { get; set; }

    [StringLength(256)]
    public string? NameOfPeriod { get; set; }

    [StringLength(128)]
    public string? MilitaryDuration { get; set; }

    [StringLength(128)]
    public string? MilitaryFullDuration { get; set; }

    [StringLength(128)]
    public string? MilitaryMinDuration { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ConfirmedLetterDate { get; set; }

    [StringLength(128)]
    public string? ConfirmedLetterNo { get; set; }

    [Column(TypeName = "date")]
    public DateTime? MilitaryStartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? MilitaryEndDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? MilitariIssueDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ImmunityValidDate { get; set; }

    public long? MilitariGradeTypeId { get; set; }

    public string? MilitariGradeType { get; set; }

    [StringLength(128)]
    public string? MilitariSerialNo { get; set; }

    [StringLength(512)]
    public string? MilitaryServiceDescriptions { get; set; }

    public bool? IsContinue { get; set; }

    public bool? MilitaryServiceIsLast { get; set; }

    public long? DueTypeId { get; set; }

    public string? DueType { get; set; }

    public long? MilitaryServiceEducationGradeId { get; set; }

    public string? MilitaryServiceEducationGrade { get; set; }

    public bool? IsComputable { get; set; }

    [Column(TypeName = "date")]
    public DateTime? FromDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ToDate { get; set; }

    #endregion MilitaryService

    #region InterdictOrder
    public long? InterdictOrderId { get; set; }

    public string? InterdictOrder { get; set; }

    public long? RecruitOrderId { get; set; }

    public string? RecruitOrder { get; set; }

    public Guid? UniqueId { get; set; }

    [StringLength(50)]
    public string? Code { get; set; }

    public decimal? SumWageFactors { get; set; }

    public short? Serial { get; set; }

    [StringLength(50)]
    public string? CreatorUserName { get; set; }

    public long? OrderTypeId { get; set; }

    public string? OrderType { get; set; }

    public long? IssueTypeId { get; set; }

    public string? IssueType { get; set; }

    public long? MarriageStatusId { get; set; }

    public string? MarriageStatus { get; set; }

    public long? InsuranceTypeId { get; set; }

    public string? InsuranceType { get; set; }

 

    public long? AspNetUsersId { get; set; }

    public string? AspNetUsers { get; set; }

    [StringLength(2048)]
    public string? Description { get; set; }

    public long? LastInterdictOrderId { get; set; }

    public string? LastInterdictOrder { get; set; }

    public long? CorrectedInterdictOrderId { get; set; }

    public string? CorrectedInterdictOrder { get; set; }

    public long? EducatioFieldId { get; set; }

    public string? EducationField { get; set; }

    public long? EducatioOrientationId { get; set; }

    public string? EducationOrientation { get; set; }

    [Column(TypeName = "date")]
    public DateTime? BirthDate { get; set; }

    public long? EmpEduID { get; set; }

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

    [Column(TypeName = "date")]
    public DateTime? EmployeeDate { get; set; }

    public string? ApproverSignatureGuid { get; set; }

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(50)]
    public string? OtherVeterans { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ApproverSignatureDate { get; set; }

    public bool? IsWomenHead { get; set; }

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

    [StringLength(2048)]
    public string? OrderReason { get; set; }

    public int? DrivingLicenseTypeId { get; set; }

    public int? ChildCount { get; set; }

    public bool? PayRollAprove { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PayRollAproveDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? PayRollRealExecuteDate { get; set; }

    public int? ItemCount { get; set; }

    public long? ArearsStatusId { get; set; }

    public bool? IsArrears { get; set; }

    public long? ApproveTimePaymentPeriod { get; set; }

    [StringLength(128)]
    public string? PayRollApproveUser { get; set; }

    #endregion InterdictOrder

    #region RecruitOrder

    public long? EmployeeId { get; set; }

    public string? Employee { get; set; }

    public long? PayLocationId { get; set; }

    public string? PayLocation { get; set; }

    public long? CostCenterId { get; set; }

    public string? CostCenter { get; set; }

    public int? CostCenterPercent { get; set; }

    public long? OrganizationUnitId { get; set; }

    public string? OrganizationUnit { get; set; }

    public long? WorkPlaceId { get; set; }

    public string? WorkPlace { get; set; }

    public long? ProjectId { get; set; }

    public string? Project { get; set; }

    public long? EmployeeStatusId { get; set; }

    public string? EmployeeStatus { get; set; }

    public long? EmployeeTypeId { get; set; }

    public string? EmployeeType { get; set; }

    public long? OrganizationJobId { get; set; }

    public string? OrganizationJob { get; set; }

    public long? OrganisationPositionId { get; set; }

    public string? OrganisationPosition { get; set; }

    #endregion RecruitOrder


}
