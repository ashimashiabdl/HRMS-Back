using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class GroupPunishmentEncourageDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long AgentOfPunishmentEncourageId { get; set; }
        public string? AgentOfPunishmentEncourage { get; set; }
        public long? OrganisationAgentOfPunishmentEncourageScoreIntervalId { get; set; }
        public string? OrganisationAgentOfPunishmentEncourageScoreInterval { get; set; }
        public string? LastModifiedUser { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(4096)]
        public string? Description { get; set; }
        public long? TempFileId { get; set; }
   
        /// <summary>
        /// تعداد افراد
        /// </summary>
        public int? EmPloyeeCount { get; set; }
    }
}
