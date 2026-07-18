using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("User_Report", Schema = "Identity")]
public class UserReport : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("User")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long UserId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual AspNetUsers? User { get; set; }
    [Comment("fill from DynamicReport Table in schema rpt")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long DynamicReportId { get; set; }
    [NotMapped]
    private new string title { get; set; }
}