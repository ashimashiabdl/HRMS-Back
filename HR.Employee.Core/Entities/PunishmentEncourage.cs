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
    /// <summary>
    ///  تنبیه ها و تشویق ها
    /// </summary>
    [Table("Punishment_Encourage", Schema = "emp")]
    public class PunishmentEncourage : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public PunishmentEncourage()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        [ForeignKey("AgentOfPunishmentEncourage")]
        public long AgentOfPunishmentEncourageId { get; set; }
        public virtual AgentOfPunishmentEncourage? AgentOfPunishmentEncourage { get; set; }
        /// <summary>
        /// مقدار واحد تنبیه یا تشویق
        /// </summary>
        public int UnitValue { get; set; } = 0;

        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(4096)]
        public string? Description { get; set; } = string.Empty;
        /// <summary>
        /// شناسه فایل اکسل در صورتی که تنبیه یا تشویق گروهی است
        /// </summary>
        [ForeignKey("GroupPunishmentEncourage")]
        public long? GroupPunishmentEncourageId { get; set; }
        public virtual GroupPunishmentEncourage? GroupPunishmentEncourage { get; set; }
        
        
        [ForeignKey("OrganisationAgentOfPunishmentEncourageScoreInterval")]
        public long? OrganisationAgentOfPunishmentEncourageScoreIntervalId { get; set; }
        public virtual OrganisationAgentOfPunishmentEncourageScoreInterval? OrganisationAgentOfPunishmentEncourageScoreInterval { get; set; }

        /// <summary>
        /// آیا این رکورد موردی هست یا گروهی
        /// </summary>
        public bool IsGroup { get; set; } = false;
        
        public int Value { get; set; } = 0;

        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
