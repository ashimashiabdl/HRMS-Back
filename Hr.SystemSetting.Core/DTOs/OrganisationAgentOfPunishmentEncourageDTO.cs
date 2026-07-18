using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationAgentOfPunishmentEncourageDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long AgentOfPunishmentEncourageId { get; set; }
        public string? AgentOfPunishmentEncourage { get; set; }
        public long AgentOfPunishmentEncourageGroupId { get; set; }
        public string? AgentOfPunishmentEncourageGroup { get; set; }
    }
}
