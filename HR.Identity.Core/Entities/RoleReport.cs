using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("Role_Report", Schema = "Identity")]
public class RoleReport : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Role")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long RoleId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual AspNetRoles? Role { get; set; }
    [Comment("fill from DynamicReport Table in schema rpt")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long DynamicReportId { get; set; }
    [NotMapped]
    private new string title { get; set; }
}

