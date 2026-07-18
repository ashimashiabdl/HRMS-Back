using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

/// <summary>
/// وضعیت درخواست توجیه استثناء کارمند
/// </summary>
[Table("Employee_Exception_Justification_Request_State", Schema = "Attendance")]
public class EmployeeExceptionJustificationRequestState : BaseEntity
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("کد وضعیت")]
    public int StateCode { get; set; }
}
