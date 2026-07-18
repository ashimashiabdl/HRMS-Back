using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("Role_Reportable_Entity", Schema = "Identity")]

public class RoleReportableEntity : BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("Role")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long RoleId { get; set; }
    public virtual AspNetRoles? Role { get; set; }

    public long ReportableEntityId { get; set; }
}
