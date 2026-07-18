using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Keyless]
public partial class VwInterdictOrder
{
    public long Id { get; set; }

    public long RecruitOrderId { get; set; }

    [StringLength(50)]
    public string? Code { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SumWageFactors { get; set; }

    public short? Serial { get; set; }

    [StringLength(50)]
    public string? CreatorUserName { get; set; }

    public long OrderTypeId { get; set; }

    [StringLength(256)]
    public string OrderType { get; set; } = null!;

    public long StatusId { get; set; }

    [StringLength(256)]
    public string? Status { get; set; }

    public long? AspNetUsersId { get; set; }

    [StringLength(450)]
    public string? AspNetUser { get; set; }

    [StringLength(2048)]
    public string? Description { get; set; }

    public long? LastInterdictOrderId { get; set; }

    public long? CorrectedInterdictOrderId { get; set; }

    public long IssueTypeId { get; set; }

    [StringLength(256)]
    public string? IssueType { get; set; }

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

    [StringLength(256)]
    public string? MarriageStatus { get; set; }

    public short? SponsorshipCount { get; set; }

    public byte? YearCoefficient { get; set; }

    public long? EducationGradeId { get; set; }

    [StringLength(256)]
    public string? EducationGrade { get; set; }

    public long? EffectiveEducationGradeId { get; set; }

    [StringLength(256)]
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

    public DateOnly? EmployeeDate { get; set; }

    public string? ApproverSignatureGuid { get; set; }

    public long InsuranceTypeId { get; set; }

    [StringLength(256)]
    public string? InsuranceType { get; set; }

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(50)]
    public string? OtherVeterans { get; set; }

    public DateOnly? ApproverSignatureDate { get; set; }

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

    [StringLength(256)]
    public string? DrivingLicenseType { get; set; }

    public int ChildCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public DateOnly? BirthDate { get; set; }

    public long? BirthPlaceId { get; set; }

    [StringLength(256)]
    public string? BirthPlace { get; set; }

    [StringLength(64)]
    public string? DrivingLicenseNumber { get; set; }

    public long? EducatioFieldId { get; set; }

    [StringLength(256)]
    public string? EducatioField { get; set; }

    public long? EducatioOrientationId { get; set; }

    [StringLength(256)]
    public string? EducatioOrientation { get; set; }

    [Column("EmpEduID")]
    public long? EmpEduId { get; set; }

    public long? IssuePlaceId { get; set; }

    [StringLength(256)]
    public string? IssuePlace { get; set; }

    [StringLength(2048)]
    public string? OrderReason { get; set; }

    public long PayLocationId { get; set; }

    [StringLength(256)]
    public string PayLocation { get; set; } = null!;

    public long EmployeeTypeId { get; set; }

    [StringLength(256)]
    public string EmployeeType { get; set; } = null!;

    public long EmployeeStatusId { get; set; }

    [StringLength(256)]
    public string EmployeeStatus { get; set; } = null!;

    public long CostCenterId { get; set; }

    [StringLength(256)]
    public string? CostCenter { get; set; }

    public long? ProjectId { get; set; }

    [StringLength(256)]
    public string? Project { get; set; }

    public long? OrganizationUnitId { get; set; }

    [StringLength(256)]
    public string? OrganisationUnit { get; set; }

    public long? WorkPlaceId { get; set; }

    [StringLength(256)]
    public string? WorkPlace { get; set; }

    public long? OrganisationPositionId { get; set; }

    [StringLength(256)]
    public string? PositionName { get; set; }

    public long? OrganizationJobId { get; set; }

    [StringLength(256)]
    public string? JobTitle { get; set; }

    public long EmployeeId { get; set; }

    public Guid? UniqueId { get; set; }

    public bool IsArrears { get; set; }

    public long? ArearsStatusId { get; set; }
}
