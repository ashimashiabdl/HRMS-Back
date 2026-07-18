using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities.EmployeeSpecific;

[Table("Attendance_Employee_Shift_Assignment", Schema = "Attendance")]
public class EmployeeShiftAssignment : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("Employee")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeId { get; set; }
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("Shift")]
    public long ShiftId { get; set; }
    public virtual Shift? Shift { get; set; }

    [StringLength(512)]
    [Comment("توضیحات")]
    public string? Description { get; set; }
}
