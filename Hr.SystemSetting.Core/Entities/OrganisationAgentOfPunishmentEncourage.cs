using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_Agent_Of_Punishment_Encourage", Schema = "Setting")]
    public class OrganisationAgentOfPunishmentEncourage : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("AgentOfPunishmentEncourage")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long AgentOfPunishmentEncourageId { get; set; }
        public virtual AgentOfPunishmentEncourage? AgentOfPunishmentEncourage { get; set; } 
        
        [ForeignKey("AgentOfPunishmentEncourageGroup")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long AgentOfPunishmentEncourageGroupId { get; set; }
        public virtual AgentOfPunishmentEncourageGroup? AgentOfPunishmentEncourageGroup { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}