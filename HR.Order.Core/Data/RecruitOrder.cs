using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.Data;

[Table("Recruit_Order", Schema = "Order")]
public class RecruitOrder : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("PayLocation")]
    public long PayLocationId { get; set; }
    public virtual OrganisationChart? PayLocation { get; set; }
    [ForeignKey("CostCenter")]
    public long CostCenterId { get; set; }
    public virtual OrganisationChart? CostCenter { get; set; }
    public int? CostCenterPercent { get; set; }

    [ForeignKey("OrganizationUnit")]
    public long? OrganizationUnitId { get; set; }
    public virtual OrganisationChart? OrganizationUnit { get; set; }
    [ForeignKey("WorkPlace")]
    public long? WorkPlaceId { get; set; }
    public virtual OrganisationChart? WorkPlace { get; set; }

    [ForeignKey("Project")]
    public long? ProjectId { get; set; }
    public virtual Project? Project { get; set; }

    [ForeignKey("EmployeeStatus")]
    public long EmployeeStatusId { get; set; }
    public virtual EmployeeStatus? EmployeeStatus { get; set; }
    [ForeignKey("EmployeeType")]
    public long EmployeeTypeId { get; set; }
    public virtual EmployeeType? EmployeeType { get; set; }
    [ForeignKey("OrganizationJob")]
    public long? OrganizationJobId { get; set; }
    public virtual OrganizationJob? OrganizationJob { get; set; }
    [ForeignKey("OrganisationPosition")]
    public long? OrganisationPositionId { get; set; }
    public virtual OrganisationPosition? OrganisationPosition { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
