using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("Group_Punishment_Encourage", Schema = "emp")]
    public class GroupPunishmentEncourage : BaseEntity, IignoreDateRangeValidation , INeedCurrentUser
    {
            public GroupPunishmentEncourage()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("AgentOfPunishmentEncourage")]
        public long AgentOfPunishmentEncourageId { get; set; }
        public virtual AgentOfPunishmentEncourage? AgentOfPunishmentEncourage { get; set; }
        [ForeignKey("OrganisationAgentOfPunishmentEncourageScoreInterval")]
        public long? OrganisationAgentOfPunishmentEncourageScoreIntervalId { get; set; }
        public virtual OrganisationAgentOfPunishmentEncourageScoreInterval? OrganisationAgentOfPunishmentEncourageScoreInterval { get; set; }
        public string? LastModifiedUser { get; set; } = string.Empty;
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(4096)]
        public string? Description { get; set; } = string.Empty;
        /// <summary>
        /// تعداد افراد
        /// </summary>
        public int? EmPloyeeCount { get; set; } = 0;
    }
}
