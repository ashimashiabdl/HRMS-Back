using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities
{
    /// <summary>
    /// عوامل تنبیه و تشویق
    /// </summary>
    [Table("Agent_Of_Punishment_Encourage", Schema = "bas")]
    public class AgentOfPunishmentEncourage : BaseEntity
    {
        /// <summary>
        /// آیا این عامل تنبیهی است ؟
        /// </summary>
        public bool IsPunishment { get; set; }
 
    }
}
