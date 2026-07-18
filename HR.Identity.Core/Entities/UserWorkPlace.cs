using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("User_WorkPlace", Schema = "Identity")]
public class UserWorkPlace :HR.SharedKernel.Data.BaseEntity , IOrganisationChartId
{
    [ForeignKey("User")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }
    [ForeignKey("WorkPlace")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WorkPlaceId { get; set; }
    public virtual OrganisationChart? WorkPlace { get; set; }
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
