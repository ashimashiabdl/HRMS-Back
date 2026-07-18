using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities.EmployeeSpecific;

[Table("Employee_Attendance_Log", Schema = "Attendance")]
public class EmployeeAttendanceLog : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("AttendanceDevice")]
    public long AttendanceDeviceId { get; set; }
    public virtual AttendanceDevice? AttendanceDevice { get; set; }

    [StringLength(128)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("شناسه کاربر در دستگاه")]
    public string? DeviceUserId { get; set; }

    [Comment("زمان ثبت در دستگاه")]
    public DateTime LogDateTime { get; set; }

    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("جهت تردد")]
    public string? Direction { get; set; }

    [StringLength(64)]
    [Comment("نوع احراز هویت")]
    public string? VerifyMode { get; set; }

    [StringLength(64)]
    [Comment("کد کار")]
    public string? WorkCode { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Comment("دمای اندازه‌گیری شده")]
    public decimal? Temperature { get; set; }

    [Comment("ماسک")]
    public bool? Mask { get; set; }

    [Comment("داده خام دستگاه")]
    public string? RawData { get; set; }

    [Comment("زمان دریافت در سامانه")]
    public DateTime? ReceiveDate { get; set; }

    [StringLength(64)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("وضعیت پردازش")]
    public string? Status { get; set; }
}
