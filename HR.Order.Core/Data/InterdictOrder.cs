using HR.BaseInfo.Core.Entities;
using HR.Employee.Core.Entities;
using HR.Identity.Core.Entities;
using HR.SharedKernel.Attribute;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Order.Core.Data;

[Table("Interdict_Order", Schema = "Order")]
public class InterdictOrder : HR.SharedKernel.Data.BaseEntity
{
    public InterdictOrder()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }

    [ForeignKey("RecruitOrder")]
    public long RecruitOrderId { get; set; }
    public virtual RecruitOrder RecruitOrder { get; set; }
    public Guid? UniqueId { get; set; }
    [StringLength(50)]
    public string? Code { get; set; } = string.Empty;
    public decimal? SumWageFactors { get; set; } = 0m;
    public short? Serial { get; set; } = 0;
    [StringLength(50)]
    public string? CreatorUserName { get; set; } = string.Empty;
    [ForeignKey("OrderType")]
    public long OrderTypeId { get; set; }
    public virtual OrderType? OrderType { get; set; }
    public long IssueTypeId { get; set; }
    public long? MarriageStatusId { get; set; }
    public long InsuranceTypeId { get; set; }

    [ForeignKey("Status")]
    public long StatusId { get; set; }
    public virtual OrderStatus? Status { get; set; }
    [ForeignKey("AspNetUsers")]
    public long? AspNetUsersId { get; set; }
    public virtual AspNetUsers? AspNetUsers { get; set; }
    [StringLength(2048)]
    public string? Description { get; set; } = string.Empty;
    [ForeignKey("LastInterdictOrder")]
    public long? LastInterdictOrderId { get; set; }
    public virtual InterdictOrder? LastInterdictOrder { get; set; }
    [ForeignKey("CorrectedInterdictOrder")]
    public long? CorrectedInterdictOrderId { get; set; }
    public virtual InterdictOrder? CorrectedInterdictOrder { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? IssueType { get; set; }

    [ForeignKey("EducationField")]
    public long? EducatioFieldId { get; set; }
    public virtual EducationField? EducationField { get; set; }

    [ForeignKey("EducationOrientation")]
    public long? EducatioOrientationId { get; set; }
    public virtual EducationOrientation? EducationOrientation { get; set; }

    [Column(TypeName = "date")]
    public DateTime? BirthDate { get; set; }

    [ForeignKey("EmpEdu")]
    public long? EmpEduID { get; set; }
    public virtual Education? EmpEdu { get; set; }

    [ForeignKey("BirthPlace")]
    public long? BirthPlaceId { get; set; }
    public virtual Places? BirthPlace { get; set; }

    [StringLength(64)]
    public string? DrivingLicenseNumber { get; set; } = string.Empty;
    [ForeignKey("IssuePlace")]
    public long? IssuePlaceId { get; set; }
    public virtual Places? IssuePlace { get; set; }

    [StringLength(8)]
    public string? ExperienceRecorded { get; set; } = string.Empty;
    [StringLength(8)]
    public string? RetiredRecorded { get; set; } = string.Empty;
    [StringLength(8)]
    public string? YearRecorded { get; set; } = string.Empty;
    public int? HistoryOut { get; set; } = 0;

    public int? HistoryStop { get; set; } = 0;

    public bool? RetiredFlagOk { get; set; } = false;
    public virtual BaseTableValue? MarriageStatus { get; set; }

    public short? SponsorshipCount { get; set; } = 0;
    public byte? YearCoefficient { get; set; } = 0;

    [ForeignKey("EducationGrade")]
    public long? EducationGradeId { get; set; }
    public virtual EducationGrade? EducationGrade { get; set; }
    [ForeignKey("EffectiveEducationGrade")]
    public long? EffectiveEducationGradeId { get; set; }
    public virtual EducationGrade? EffectiveEducationGrade { get; set; }
    public bool? IsWar { get; set; } = false;
    public bool? IsCaptivity { get; set; } = false;
    public bool? IsBasij { get; set; } = false;
    public bool? IsIsar { get; set; } = false;
    public float? IsarPercent { get; set; } = 0f;
    public int? WarDuration { get; set; } = 0;
    public int? CaptivityDuration { get; set; } = 0;
    public int? BasijDuration { get; set; } = 0;
    public int? JobDegree { get; set; } = 0;
    public bool? IsMartyrs { get; set; } = false;

    public int? WifeCount { get; set; } = 0;

    public int? GradScore { get; set; } = 0;
    [Column(TypeName = "date")]
    public DateTime? EmployeeDate { get; set; }

    public string? ApproverSignatureGuid { get; set; } = string.Empty;
    public virtual BaseTableValue? InsuranceType { get; set; }
    [StringLength(50)]
    public string? AccountNumber { get; set; } = string.Empty;
    [StringLength(50)]
    public string? OtherVeterans { get; set; } = string.Empty;
    [Column(TypeName = "date")]
    public DateTime? ApproverSignatureDate { get; set; }

    public bool IsWomenHead { get; set; } = false;
    [StringLength(100)]
    public string? FirstName { get; set; } = string.Empty;
    [StringLength(100)]
    public string? LastName { get; set; } = string.Empty;
    [StringLength(40)]
    public string? FatherName { get; set; } = string.Empty;
    [StringLength(50)]
    public string? PersonelCode { get; set; } = string.Empty;
    [StringLength(15)]
    public string? IdentityNo { get; set; } = string.Empty;
    [StringLength(10)]
    public string? NationalNo { get; set; } = string.Empty;
    [StringLength(2048)]
    public string? OrderReason { get; set; } = string.Empty;
    public int? DrivingLicenseTypeId { get; set; } = 0;
    public int ChildCount { get; set; } = 0;
    public bool PayRollAprove { get; set; } = false;
    [Column(TypeName = "datetime")]
    public DateTime? PayRollAproveDate { get; set; }
    [Column(TypeName = "date")]
    public DateTime? PayRollRealExecuteDate { get; set; }
    public int ItemCount { get; set; } = 0;
    /// <summary>
    /// ����� �����
    /// </summary>
    //[ForeignKey("ArearsStatus")]
    public long? ArearsStatusId { get; set; }
    /// <summary>
    /// ��� ͘� �� ���� ����� �� ���� � ������ ����� ����� ���� ��� �
    /// </summary>
    [Comment(" ��� ͘� �� ���� ����� �� ���� � ������ ����� ����� ���� ��� �")]
    public bool IsArrears { get; set; } = false;
    ///// <summary>
    ///// ������� ����� ����� ��� ��� �� ��
    ///// </summary>
    //[Comment("������� ����� ����� ��� ��� �� ��")]
    //public bool ArrearsCalculated { get; set; }
    /// <summary>
    /// ��� ���� ��� ������ ����� ������� �� ���
    /// </summary>
    [Comment(" ��� ���� ��� ������ ����� ������� �� ���")]
    public long? ApproveTimePaymentPeriod { get; set; }
    [StringLength(128)]
    public string? PayRollApproveUser { get; set; } = string.Empty;
    [NotMapped]
    private new string title { get; set; } = string.Empty;
}
