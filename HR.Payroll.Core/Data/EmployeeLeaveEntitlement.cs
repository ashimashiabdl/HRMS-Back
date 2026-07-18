using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;


/// <summary>
/// سهمیه مرخصی سالانه
/// </summary>
[Table("Employee_Leave_Entitlement", Schema = "Payroll")]
public class EmployeeLeaveEntitlement : BaseEntity ,IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("LeaveType")]
    public long LeaveTypeId { get; set; }
    public virtual LeaveType? LeaveType { get; set; }
    public int Year { get; set; }
    public decimal LeaveAmount { get; set; }
}
