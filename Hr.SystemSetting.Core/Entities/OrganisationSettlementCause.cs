using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hr.SystemSetting.Core.Entities;

[Table("Organisation_Settlement_Cause", Schema = "Setting")]
public class OrganisationSettlementCause : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("SettlementCause")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long SettlementCauseId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual SettlementCause? SettlementCause { get; set; }
}
