using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Black_List", Schema = "Payroll")]
public class BlackList : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    public long BlackListEnumerationId { get; set; }
    [ForeignKey("Employee")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
    [StringLength(128)]
    public string? Comment { get; set; }
    public bool WillBeCalculated { get; set; }
    [StringLength(512)]
    public string? Description { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
