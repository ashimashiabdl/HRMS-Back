using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_Agent_Of_Punishment_Encourage_Score_Interval", Schema = "Setting")]
    public class OrganisationAgentOfPunishmentEncourageScoreInterval : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        public int FromValue { get; set; }
        public int ToValue { get; set; }
    }
}
