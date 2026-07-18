using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

/// <summary>
/// نوع عدم حضور
/// </summary>
[Table("Attendance_Absence_Type", Schema = "Attendance")]
public class AbsenceType : BaseEntity
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("کد نوع")]
    public int TypeCode { get; set; }
}
