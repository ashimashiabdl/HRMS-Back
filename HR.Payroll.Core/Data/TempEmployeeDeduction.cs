using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

/// <summary>
/// ردیف‌های موقت استخراج‌شده از اکسل کسورات قبل از نهایی‌سازی
/// </summary>
[Table("Temp_Employee_Deduction", Schema = "Payroll")]
public class TempEmployeeDeduction : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("EmployeeDeductionUploadBatch")]
    public long EmployeeDeductionUploadBatchId { get; set; }
    public virtual EmployeeDeductionUploadBatch? EmployeeDeductionUploadBatch { get; set; }

    /// <summary>
    /// شناسه فایل در جدول bas.File
    /// </summary>
    public long FileId { get; set; }

    [ForeignKey("Employee")]
    public long? EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

    /// <summary>
    /// کد ملی خام خوانده‌شده از اکسل (پس از نرمال‌سازی)
    /// </summary>
    [StringLength(10)]
    public string? NationalNo { get; set; }

    [ForeignKey("DeductionType")]
    public long DeductionTypeId { get; set; }
    public virtual DeductionType? DeductionType { get; set; }

    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("StartDeductPaymentPeriod")]
    public long StartDeductPaymentPeriodId { get; set; }
    public virtual PaymentPeriod? StartDeductPaymentPeriod { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [StringLength(128)]
    public string? LoanPaymentDocDesc { get; set; }

    public long? AllAmount { get; set; }
    public long? InstallmentAmount { get; set; }

    public bool RemainingCrumbsAtFirst { get; set; }

    public bool IsActive { get; set; }

    /// <summary>
    /// در صورت خطا در استخراج یا عدم یافتن کارمند، متن خطا
    /// </summary>
    [StringLength(512)]
    public string? ParseErrorMessage { get; set; }
}
