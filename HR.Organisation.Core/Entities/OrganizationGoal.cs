using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities;

/// <summary>
/// اهداف سازمان
/// </summary>
[Table("Organization_Goal", Schema = "Org")]
public class OrganizationGoal : BaseEntity, IOrganisationChartId , IignoreDateRangeValidation
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    public string? GoalDescription { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
