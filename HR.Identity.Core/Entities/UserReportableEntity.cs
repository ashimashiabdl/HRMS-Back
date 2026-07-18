using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("User_Reportable_Entity", Schema = "Identity")]
public class UserReportableEntity : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("User")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }

    public long ReportableEntityId { get; set; }


}
