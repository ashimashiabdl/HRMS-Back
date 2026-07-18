using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Organisation_Leave", Schema = "Payroll")]
public class OrganisationLeave : BaseEntity, IignoreDateRangeValidation, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("LeaveType")]
    public long LeaveTypeId { get; set; }
    public virtual LeaveType? LeaveType { get; set; }
}
