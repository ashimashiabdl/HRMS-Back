using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

/// <summary>
/// نگهداری اطلاعات هر بار آپلود فایل اکسل کسورات (فایل، کاربر آپلودکننده و ...)
/// </summary>
[Table("Employee_Deduction_Upload_Batch", Schema = "Payroll")]
public class EmployeeDeductionUploadBatch : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    /// <summary>
    /// شناسه فایل در جدول bas.File
    /// </summary>
    public long FileId { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(256)]
    public string? UploaderUserName { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(128)]
    public string? UploaderDisplayName { get; set; }

    public int TotalRowsRead { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }

    /// <summary>
    /// JSON آرایه ردیف‌های ناموفق برای نمایش در گزارش
    /// </summary>
    [System.ComponentModel.DataAnnotations.StringLength(4000)]
    public string? FailedRowsJson { get; set; }
}
