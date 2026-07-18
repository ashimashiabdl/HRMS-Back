using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("User_PayLocation", Schema = "Identity")]
public class UserPayLocation : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey("PayLocation")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long PayLocationId { get; set; }
    public virtual OrganisationChart? PayLocation { get; set; }
    [ForeignKey("User")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
