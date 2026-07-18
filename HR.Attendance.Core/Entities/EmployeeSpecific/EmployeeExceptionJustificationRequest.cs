using HR.Attendance.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities.EmployeeSpecific;

/// <summary>
/// درخواست توجیه استثناء عدم حضور کارمند
/// </summary>
[Table("Employee_Exception_Justification_Request", Schema = "Attendance")]
public class EmployeeExceptionJustificationRequest : BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("EmployeeAttendanceException")]
    public long EmployeeAttendanceExceptionId { get; set; }
    public virtual EmployeeAttendanceException? EmployeeAttendanceException { get; set; }

    [ForeignKey("AbsenceType")]
    public long AbsenceTypeId { get; set; }
    public virtual AbsenceType? AbsenceType { get; set; }

    [ForeignKey("LeaveType")]
    public long? LeaveTypeId { get; set; }
    public virtual LeaveType? LeaveType { get; set; }

    [ForeignKey("EmployeeExceptionJustificationRequestState")]
    public long EmployeeExceptionJustificationRequestStateId { get; set; }
    public virtual EmployeeExceptionJustificationRequestState? EmployeeExceptionJustificationRequestState { get; set; }

    [StringLength(1024)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("توضیحات")]
    public string? Description { get; set; }
}
