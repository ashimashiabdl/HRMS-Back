using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("BaseOrganisationId", Name = "IX_Employee_BaseOrganisationId")]
[Microsoft.EntityFrameworkCore.Index("BaseOrganisationId", "IsActive", "Disabled", Name = "IX_Employee_BaseOrganisation_Status")]
[Microsoft.EntityFrameworkCore.Index("BirthPlaceId", Name = "IX_Employee_BirthPlaceId")]
[Microsoft.EntityFrameworkCore.Index("BloodGroupId", Name = "IX_Employee_BloodGroupId")]
[Microsoft.EntityFrameworkCore.Index("CitizenshipId", Name = "IX_Employee_CitizenshipId")]
[Microsoft.EntityFrameworkCore.Index("ConfidentialityLevelId", Name = "IX_Employee_ConfidentialityLevelId")]
[Microsoft.EntityFrameworkCore.Index("GenderId", "MaritalStatusId", "TaxExemptionTypeId", "SkillLevelId", Name = "IX_Employee_FK_Group1")]
[Microsoft.EntityFrameworkCore.Index("GenderId", Name = "IX_Employee_GenderId")]
[Microsoft.EntityFrameworkCore.Index("HeadquartersOrRowTypeId", Name = "IX_Employee_HeadquartersOrRowTypeId")]
[Microsoft.EntityFrameworkCore.Index("IsDeleted", "NationalNo", Name = "IX_Employee_IsDeleted_NationalNo")]
[Microsoft.EntityFrameworkCore.Index("IssuePlaceId", Name = "IX_Employee_IssuePlaceId")]
[Microsoft.EntityFrameworkCore.Index("ManagementAndStewardshipJobId", Name = "IX_Employee_ManagementAndStewardshipJobId")]
[Microsoft.EntityFrameworkCore.Index("MaritalStatusId", Name = "IX_Employee_MaritalStatusId")]
[Microsoft.EntityFrameworkCore.Index("MartyrRelationId", Name = "IX_Employee_MartyrRelationId")]
[Microsoft.EntityFrameworkCore.Index("MazhabId", Name = "IX_Employee_MazhabId")]
[Microsoft.EntityFrameworkCore.Index("NationalityId", Name = "IX_Employee_NationalityId")]
[Microsoft.EntityFrameworkCore.Index("ReligeonId", Name = "IX_Employee_ReligeonId")]
[Microsoft.EntityFrameworkCore.Index("ServicePlaceId", Name = "IX_Employee_ServicePlaceId")]
[Microsoft.EntityFrameworkCore.Index("SkillLevelId", Name = "IX_Employee_SkillLevelId")]
[Microsoft.EntityFrameworkCore.Index("TaminInsuranceJobListId", Name = "IX_Employee_TaminInsuranceJobListId")]
[Microsoft.EntityFrameworkCore.Index("TaxExemptionTypeId", Name = "IX_Employee_TaxExemptionTypeId")]
public partial class Employee
{
    [Key]
    public long Id { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(40)]
    public string? FatherName { get; set; }

    [StringLength(50)]
    public string? PersonelCode { get; set; }

    public long? GenderId { get; set; }

    [StringLength(15)]
    public string? IdentityNo { get; set; }

    public long? BirthPlaceId { get; set; }

    public long? ReligeonId { get; set; }

    [StringLength(10)]
    public string? NationalNo { get; set; }

    public long? NationalityId { get; set; }

    public long? IssuePlaceId { get; set; }

    public DateOnly? BirthDate { get; set; }

    public long? BloodGroupId { get; set; }

    [StringLength(50)]
    public string? ActiveName { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public long? PersonId { get; set; }

    public int? PrivateJobStatus { get; set; }

    public DateOnly? IssueDate { get; set; }

    public int? IssueSerialChar { get; set; }

    [StringLength(10)]
    public string? IssueSerialString { get; set; }

    [StringLength(20)]
    public string? IssueSerialOrder { get; set; }

    public long? MaritalStatusId { get; set; }

    [StringLength(20)]
    public string? PassportNo { get; set; }

    public int? SectId { get; set; }

    public bool IsActive { get; set; }

    public bool IsVerify { get; set; }

    public long? BaseOrganisationId { get; set; }

    public bool IsRetired { get; set; }

    public int SubsystemId { get; set; }

    public int? Imperfective { get; set; }

    public bool IsWomenHead { get; set; }

    public DateOnly? StartWorkDate { get; set; }

    [StringLength(20)]
    public string? LostIssueSerialString { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    public long? VehicleStatusId { get; set; }

    public int AccessFailedCount { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public bool Disabled { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool IsAdmin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastWrongAttemptDatetime { get; set; }

    public string? PasswordHash { get; set; }

    [StringLength(64)]
    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public string? SecurityStamp { get; set; }

    public bool TwoFactorEnabled { get; set; }

    [Column("salt")]
    public byte[]? Salt { get; set; }

    [StringLength(20)]
    public string? InOutCard { get; set; }

    public long? MazhabId { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    [StringLength(250)]
    public string? ReleaseReason { get; set; }

    public long? ServicePlaceId { get; set; }

    public long? LifeAccidentInsuranceTypeId { get; set; }

    public long? UseLifeAccidentInsuranceTypeId { get; set; }

    public long? CompeleteInsuranceTypeId { get; set; }

    public bool? IsCashBenefits { get; set; }

    public bool? IsHekmat { get; set; }

    public long? UseCompeleteInsuranceTypeId { get; set; }

    public bool? IsWelfareBenefits { get; set; }

    public long? TaxExemptionTypeId { get; set; }

    public long? ManagementAndStewardshipJobId { get; set; }

    public long? MartyrRelationId { get; set; }

    public long? HeadquartersOrRowTypeId { get; set; }

    public long? TaminInsuranceJobListId { get; set; }

    public long? ConfidentialityLevelId { get; set; }

    [StringLength(100)]
    public string? MartyrChildTrackingCode { get; set; }

    public long? SkillLevelId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(50)]
    public string? AccountingSystemEmployeeId { get; set; }

    public long? CitizenshipId { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();

    [InverseProperty("Employee")]
    public virtual ICollection<AbsenceRecord> AbsenceRecords { get; set; } = new List<AbsenceRecord>();

    [InverseProperty("Employee")]
    public virtual ICollection<Appearance> Appearances { get; set; } = new List<Appearance>();

    [InverseProperty("Employee")]
    public virtual ICollection<Arear> Arears { get; set; } = new List<Arear>();

    [InverseProperty("Employee")]
    public virtual ICollection<AttendanceEmployeeShiftAssignment> AttendanceEmployeeShiftAssignments { get; set; } = new List<AttendanceEmployeeShiftAssignment>();

    [InverseProperty("Employee")]
    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    [InverseProperty("Employee")]
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    [InverseProperty("Employee")]
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

    [InverseProperty("Employee")]
    public virtual ICollection<BankDisketteItem> BankDisketteItems { get; set; } = new List<BankDisketteItem>();

    [ForeignKey("BaseOrganisationId")]
    [InverseProperty("Employees")]
    public virtual OrganisationChart? BaseOrganisation { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<BasijGrade> BasijGrades { get; set; } = new List<BasijGrade>();

    [InverseProperty("Employee")]
    public virtual ICollection<Basij> Basijs { get; set; } = new List<Basij>();

    [InverseProperty("Employee")]
    public virtual ICollection<BatchLog> BatchLogs { get; set; } = new List<BatchLog>();

    [InverseProperty("Employee")]
    public virtual ICollection<BatchPayRollRequestDetail> BatchPayRollRequestDetails { get; set; } = new List<BatchPayRollRequestDetail>();

    [InverseProperty("Employee")]
    public virtual ICollection<BatchRequestDetail> BatchRequestDetails { get; set; } = new List<BatchRequestDetail>();

    [ForeignKey("BirthPlaceId")]
    [InverseProperty("EmployeeBirthPlaces")]
    public virtual Place? BirthPlace { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<BlackList> BlackLists { get; set; } = new List<BlackList>();

    [InverseProperty("Employee")]
    public virtual ICollection<BlockedAccount> BlockedAccounts { get; set; } = new List<BlockedAccount>();

    [ForeignKey("BloodGroupId")]
    [InverseProperty("EmployeeBloodGroups")]
    public virtual BaseTableValue? BloodGroup { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Captivity> Captivities { get; set; } = new List<Captivity>();

    [InverseProperty("Employee")]
    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    [ForeignKey("CitizenshipId")]
    [InverseProperty("EmployeeCitizenships")]
    public virtual BaseTableValue? Citizenship { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Coefficient1> Coefficient1s { get; set; } = new List<Coefficient1>();

    [InverseProperty("Employee")]
    public virtual ICollection<Competency> Competencies { get; set; } = new List<Competency>();

    [ForeignKey("ConfidentialityLevelId")]
    [InverseProperty("Employees")]
    public virtual ConfidentialityLevel? ConfidentialityLevel { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();

    [InverseProperty("Employee")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("Employee")]
    public virtual ICollection<Disability> Disabilities { get; set; } = new List<Disability>();

    [InverseProperty("Employee")]
    public virtual ICollection<DrivingLicense> DrivingLicenses { get; set; } = new List<DrivingLicense>();

    [InverseProperty("Employee")]
    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeFile> EmployeeFiles { get; set; } = new List<EmployeeFile>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeFund> EmployeeFunds { get; set; } = new List<EmployeeFund>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeLeaveEntitlement> EmployeeLeaveEntitlements { get; set; } = new List<EmployeeLeaveEntitlement>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeLoginHistory> EmployeeLoginHistories { get; set; } = new List<EmployeeLoginHistory>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeOtp> EmployeeOtps { get; set; } = new List<EmployeeOtp>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeRefreshToken> EmployeeRefreshTokens { get; set; } = new List<EmployeeRefreshToken>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeRequest> EmployeeRequests { get; set; } = new List<EmployeeRequest>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlements { get; set; } = new List<EmployeeSettlement>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeSoftware> EmployeeSoftwares { get; set; } = new List<EmployeeSoftware>();

    [InverseProperty("Employee")]
    public virtual ICollection<EvaluationResult> EvaluationResults { get; set; } = new List<EvaluationResult>();

    [InverseProperty("Employee")]
    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    [InverseProperty("Employee")]
    public virtual ICollection<Family> Families { get; set; } = new List<Family>();

    [InverseProperty("Employee")]
    public virtual ICollection<FichePdfArchive> FichePdfArchives { get; set; } = new List<FichePdfArchive>();

    [InverseProperty("Employee")]
    public virtual ICollection<FicheReportArchive> FicheReportArchives { get; set; } = new List<FicheReportArchive>();

    [InverseProperty("Employee")]
    public virtual ICollection<Fiche> Fiches { get; set; } = new List<Fiche>();

    [InverseProperty("Employee")]
    public virtual ICollection<ForeignLanguage> ForeignLanguages { get; set; } = new List<ForeignLanguage>();

    [InverseProperty("Employee")]
    public virtual ICollection<ForeignTravel> ForeignTravels { get; set; } = new List<ForeignTravel>();

    [ForeignKey("GenderId")]
    [InverseProperty("EmployeeGenders")]
    public virtual BaseTableValue? Gender { get; set; }

    [ForeignKey("HeadquartersOrRowTypeId")]
    [InverseProperty("EmployeeHeadquartersOrRowTypes")]
    public virtual BaseTableValue? HeadquartersOrRowType { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<HistoryStop> HistoryStops { get; set; } = new List<HistoryStop>();

    [InverseProperty("Employee")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Employee")]
    public virtual ICollection<InsuranceDisketteItem> InsuranceDisketteItems { get; set; } = new List<InsuranceDisketteItem>();

    [InverseProperty("Employee")]
    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();

    [InverseProperty("Employee")]
    public virtual ICollection<Isar> Isars { get; set; } = new List<Isar>();

    [ForeignKey("IssuePlaceId")]
    [InverseProperty("EmployeeIssuePlaces")]
    public virtual Place? IssuePlace { get; set; }

    [ForeignKey("ManagementAndStewardshipJobId")]
    [InverseProperty("Employees")]
    public virtual ManagementAndStewardshipJob? ManagementAndStewardshipJob { get; set; }

    [ForeignKey("MaritalStatusId")]
    [InverseProperty("EmployeeMaritalStatuses")]
    public virtual BaseTableValue? MaritalStatus { get; set; }

    [ForeignKey("MartyrRelationId")]
    [InverseProperty("EmployeeMartyrRelations")]
    public virtual BaseTableValue? MartyrRelation { get; set; }

    [ForeignKey("MazhabId")]
    [InverseProperty("EmployeeMazhabs")]
    public virtual BaseTableValue? Mazhab { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<MilitaryService> MilitaryServices { get; set; } = new List<MilitaryService>();

    [ForeignKey("NationalityId")]
    [InverseProperty("EmployeeNationalities")]
    public virtual BaseTableValue? Nationality { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<OtherVeteran> OtherVeterans { get; set; } = new List<OtherVeteran>();

    [InverseProperty("Employee")]
    public virtual ICollection<PaymentPeriodEmployeeBonu> PaymentPeriodEmployeeBonus { get; set; } = new List<PaymentPeriodEmployeeBonu>();

    [InverseProperty("Employee")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItems { get; set; } = new List<PersonnelFicheItem>();

    [InverseProperty("Employee")]
    public virtual ICollection<PersonnelLeave> PersonnelLeaves { get; set; } = new List<PersonnelLeave>();

    [InverseProperty("Employee")]
    public virtual ICollection<PersonnelLoan> PersonnelLoans { get; set; } = new List<PersonnelLoan>();

    [InverseProperty("Employee")]
    public virtual ICollection<PersonnelManagerList> PersonnelManagerLists { get; set; } = new List<PersonnelManagerList>();

    [InverseProperty("Employee")]
    public virtual ICollection<PersonnelPayment> PersonnelPayments { get; set; } = new List<PersonnelPayment>();

    [InverseProperty("Employee")]
    public virtual ICollection<PunishmentEncourage> PunishmentEncourages { get; set; } = new List<PunishmentEncourage>();

    [InverseProperty("Employee")]
    public virtual ICollection<RecruitOrder> RecruitOrders { get; set; } = new List<RecruitOrder>();

    [ForeignKey("ReligeonId")]
    [InverseProperty("EmployeeReligeons")]
    public virtual BaseTableValue? Religeon { get; set; }

    [ForeignKey("ServicePlaceId")]
    [InverseProperty("EmployeeServicePlaces")]
    public virtual Place? ServicePlace { get; set; }

    [ForeignKey("SkillLevelId")]
    [InverseProperty("Employees")]
    public virtual SkillLevel? SkillLevel { get; set; }

    [ForeignKey("TaminInsuranceJobListId")]
    [InverseProperty("Employees")]
    public virtual TaminInsuranceJobList? TaminInsuranceJobList { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<TaxDisketteWh> TaxDisketteWhs { get; set; } = new List<TaxDisketteWh>();

    [InverseProperty("Employee")]
    public virtual ICollection<TaxDisketteWp> TaxDisketteWps { get; set; } = new List<TaxDisketteWp>();

    [ForeignKey("TaxExemptionTypeId")]
    [InverseProperty("Employees")]
    public virtual TaxExemptionType? TaxExemptionType { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<TaxNonCashPayment> TaxNonCashPayments { get; set; } = new List<TaxNonCashPayment>();

    [InverseProperty("Employee")]
    public virtual ICollection<TaxableIncome> TaxableIncomes { get; set; } = new List<TaxableIncome>();

    [InverseProperty("Employee")]
    public virtual ICollection<TempEmployeeDeduction> TempEmployeeDeductions { get; set; } = new List<TempEmployeeDeduction>();

    [InverseProperty("Employee")]
    public virtual ICollection<TempPersonnelLeave> TempPersonnelLeaves { get; set; } = new List<TempPersonnelLeave>();

    [InverseProperty("Employee")]
    public virtual ICollection<TempPunishmentEncourage> TempPunishmentEncourages { get; set; } = new List<TempPunishmentEncourage>();

    [InverseProperty("Employee")]
    public virtual ICollection<War> Wars { get; set; } = new List<War>();

    [InverseProperty("Employee")]
    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
