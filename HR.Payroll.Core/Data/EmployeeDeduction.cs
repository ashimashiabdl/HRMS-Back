using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Employee_Deduction", Schema = "Payroll")]
public class EmployeeDeduction : BaseEntity,IOrganisationChartId , IignoreDateRangeValidation
{
    /// <summary>
    /// شناسه فایل در جدول bas.File (برای ردیف‌های منتقل‌شده از آپلود اکسل)
    /// </summary>
    public long? FileId { get; set; }

    /// <summary>
    /// ردیف موقت منبع (در صورت انتقال از آپلود اکسل)
    /// </summary>
    [ForeignKey("TempEmployeeDeduction")]
    public long? TempEmployeeDeductionId { get; set; }
    public virtual TempEmployeeDeduction? TempEmployeeDeduction { get; set; }

    /// <summary>
    /// دسته آپلود منبع (برای مدیریت فایل‌های کسورات آپلود شده)
    /// </summary>
    [ForeignKey("EmployeeDeductionUploadBatch")]
    public long? EmployeeDeductionUploadBatchId { get; set; }
    public virtual EmployeeDeductionUploadBatch? EmployeeDeductionUploadBatch { get; set; }

    [ForeignKey("Employee")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("DeductionType")]
    public long DeductionTypeId { get; set; }
    public virtual DeductionType? DeductionType { get; set; }
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("StartDeductPaymentPeriod")]
    public long StartDeductPaymentPeriodId { get; set; }
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [StringLength(128)]
    public string? LoanPaymentDocDesc { get; set; }

    public long? AllAmount { get; set; }
    public long? InstallmentAmount { get; set; }

    /// <summary>
    /// خرده باقیمانده در اولین قسط کم شود
    /// </summary>
    public bool RemainingCrumbsAtFirst { get; set; }

    public bool IsActive { get; set; }
}


