using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Interdict_Order", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("LastInterdictOrderId", "CorrectedInterdictOrderId", Name = "IX_InterdictOrder_Last_Corrected")]
[Microsoft.EntityFrameworkCore.Index("PersonelCode", "NationalNo", Name = "IX_InterdictOrder_PersonelCode_NationalNo")]
[Microsoft.EntityFrameworkCore.Index("RecruitOrderId", "StatusId", "OrderTypeId", Name = "IX_InterdictOrder_Recruit_Status_Type")]
[Microsoft.EntityFrameworkCore.Index("AspNetUsersId", Name = "IX_Interdict_Order_AspNetUsersId")]
[Microsoft.EntityFrameworkCore.Index("BirthPlaceId", Name = "IX_Interdict_Order_BirthPlaceId")]
[Microsoft.EntityFrameworkCore.Index("CorrectedInterdictOrderId", Name = "IX_Interdict_Order_CorrectedInterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("EducatioFieldId", Name = "IX_Interdict_Order_EducatioFieldId")]
[Microsoft.EntityFrameworkCore.Index("EducatioOrientationId", Name = "IX_Interdict_Order_EducatioOrientationId")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Interdict_Order_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EffectiveEducationGradeId", Name = "IX_Interdict_Order_EffectiveEducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmpEduId", Name = "IX_Interdict_Order_EmpEduID")]
[Microsoft.EntityFrameworkCore.Index("Id", "StatusId", Name = "IX_Interdict_Order_Id_StatusId_RecruitOrder")]
[Microsoft.EntityFrameworkCore.Index("InsuranceTypeId", Name = "IX_Interdict_Order_InsuranceTypeId")]
[Microsoft.EntityFrameworkCore.Index("IssuePlaceId", Name = "IX_Interdict_Order_IssuePlaceId")]
[Microsoft.EntityFrameworkCore.Index("IssueTypeId", Name = "IX_Interdict_Order_IssueTypeId")]
[Microsoft.EntityFrameworkCore.Index("LastInterdictOrderId", Name = "IX_Interdict_Order_LastInterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("MarriageStatusId", Name = "IX_Interdict_Order_MarriageStatusId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Interdict_Order_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("RecruitOrderId", Name = "IX_Interdict_Order_RecruitOrderId")]
[Microsoft.EntityFrameworkCore.Index("RecruitOrderId", "IsDeleted", Name = "IX_Interdict_Order_RecruitOrderId_IsDeleted_Lookup")]
[Microsoft.EntityFrameworkCore.Index("RecruitOrderId", "StatusId", Name = "IX_Interdict_Order_RecruitOrderId_StatusId")]
[Microsoft.EntityFrameworkCore.Index("RecruitOrderId", "StatusId", "IsDeleted", "CreateDate", Name = "IX_Interdict_Order_RecruitOrderId_Status_IsDeleted_CreateDate", IsDescending = new[] { false, false, false, true })]
[Microsoft.EntityFrameworkCore.Index("StatusId", Name = "IX_Interdict_Order_StatusId")]
[Microsoft.EntityFrameworkCore.Index("StatusId", "NationalNo", Name = "IX_Interdict_Order_StatusId_NationalNo")]
[Microsoft.EntityFrameworkCore.Index("StatusId", "StartDate", "RecruitOrderId", Name = "IX_Interdict_Order_StatusId_StartDate_RecruitOrderId")]
public partial class InterdictOrder
{
    [Key]
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

    public long StatusId { get; set; }

    [StringLength(2048)]
    public string? Description { get; set; }

    public long? LastInterdictOrderId { get; set; }

    public long? CorrectedInterdictOrderId { get; set; }

    public long IssueTypeId { get; set; }

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

    public short? SponsorshipCount { get; set; }

    public byte? YearCoefficient { get; set; }

    public long? EducationGradeId { get; set; }

    public long? EffectiveEducationGradeId { get; set; }

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

    public int ChildCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public DateOnly? BirthDate { get; set; }

    public long? BirthPlaceId { get; set; }

    [StringLength(64)]
    public string? DrivingLicenseNumber { get; set; }

    public long? EducatioFieldId { get; set; }

    public long? EducatioOrientationId { get; set; }

    [Column("EmpEduID")]
    public long? EmpEduId { get; set; }

    public long? IssuePlaceId { get; set; }

    [StringLength(2048)]
    public string? OrderReason { get; set; }

    public Guid? UniqueId { get; set; }

    public long? AspNetUsersId { get; set; }

    public bool PayRollAprove { get; set; }

    public long? ArearsStatusId { get; set; }

    public int ItemCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PayRollAproveDate { get; set; }

    public DateOnly? PayRollRealExecuteDate { get; set; }

    [StringLength(128)]
    public string? PayRollApproveUser { get; set; }

    /// <summary>
    ///  ��� ͘� �� ���� ����� �� ���� � ������ ����� ����� ���� ��� �
    /// </summary>
    public bool IsArrears { get; set; }

    /// <summary>
    ///  ��� ���� ��� ������ ����� ������� �� ���
    /// </summary>
    public long? ApproveTimePaymentPeriod { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<Arear> Arears { get; set; } = new List<Arear>();

    [ForeignKey("AspNetUsersId")]
    [InverseProperty("InterdictOrders")]
    public virtual AspNetUser? AspNetUsers { get; set; }

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<BatchLog> BatchLogs { get; set; } = new List<BatchLog>();

    [InverseProperty("Interdict")]
    public virtual ICollection<BatchRequestDetail> BatchRequestDetails { get; set; } = new List<BatchRequestDetail>();

    [ForeignKey("BirthPlaceId")]
    [InverseProperty("InterdictOrderBirthPlaces")]
    public virtual Place? BirthPlace { get; set; }

    [ForeignKey("CorrectedInterdictOrderId")]
    [InverseProperty("InverseCorrectedInterdictOrder")]
    public virtual InterdictOrder? CorrectedInterdictOrder { get; set; }

    [ForeignKey("EducatioFieldId")]
    [InverseProperty("InterdictOrders")]
    public virtual EducationField? EducatioField { get; set; }

    [ForeignKey("EducatioOrientationId")]
    [InverseProperty("InterdictOrders")]
    public virtual EducationOrientation? EducatioOrientation { get; set; }

    [ForeignKey("EducationGradeId")]
    [InverseProperty("InterdictOrderEducationGrades")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EffectiveEducationGradeId")]
    [InverseProperty("InterdictOrderEffectiveEducationGrades")]
    public virtual EducationGrade? EffectiveEducationGrade { get; set; }

    [ForeignKey("EmpEduId")]
    [InverseProperty("InterdictOrders")]
    public virtual Education? EmpEdu { get; set; }

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlementInterdictOrders { get; set; } = new List<EmployeeSettlement>();

    [InverseProperty("LastInterdictOrder")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlementLastInterdictOrders { get; set; } = new List<EmployeeSettlement>();

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<Fiche> Fiches { get; set; } = new List<Fiche>();

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<InsuranceDisketteItem> InsuranceDisketteItems { get; set; } = new List<InsuranceDisketteItem>();

    [InverseProperty("InterdictOrder")]
    public virtual InterdictOrderArchive? InterdictOrderArchive { get; set; }

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<InterdictOrderCoefficientItem> InterdictOrderCoefficientItems { get; set; } = new List<InterdictOrderCoefficientItem>();

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<InterdictOrderCopy> InterdictOrderCopies { get; set; } = new List<InterdictOrderCopy>();

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<InterdictOrderPromissory> InterdictOrderPromissories { get; set; } = new List<InterdictOrderPromissory>();

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<InterdictOrderWageItem> InterdictOrderWageItems { get; set; } = new List<InterdictOrderWageItem>();

    [InverseProperty("CorrectedInterdictOrder")]
    public virtual ICollection<InterdictOrder> InverseCorrectedInterdictOrder { get; set; } = new List<InterdictOrder>();

    [InverseProperty("LastInterdictOrder")]
    public virtual ICollection<InterdictOrder> InverseLastInterdictOrder { get; set; } = new List<InterdictOrder>();

    [ForeignKey("IssuePlaceId")]
    [InverseProperty("InterdictOrderIssuePlaces")]
    public virtual Place? IssuePlace { get; set; }

    [ForeignKey("LastInterdictOrderId")]
    [InverseProperty("InverseLastInterdictOrder")]
    public virtual InterdictOrder? LastInterdictOrder { get; set; }

    [ForeignKey("OrderTypeId")]
    [InverseProperty("InterdictOrders")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("RecruitOrderId")]
    [InverseProperty("InterdictOrders")]
    public virtual RecruitOrder RecruitOrder { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("InterdictOrders")]
    public virtual OrderStatus Status { get; set; } = null!;

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<TaxDisketteWh> TaxDisketteWhs { get; set; } = new List<TaxDisketteWh>();

    [InverseProperty("InterdictOrder")]
    public virtual ICollection<WorkFlowInstance> WorkFlowInstances { get; set; } = new List<WorkFlowInstance>();
}
