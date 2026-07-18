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
    public class PunishmentEncourageDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        [Required(ErrorMessage = "انتخاب کارمند الزامی هست")]
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long AgentOfPunishmentEncourageId { get; set; }
        public string? AgentOfPunishmentEncourage { get; set; }
        /// <summary>
        /// مقدار واحد تنبیه یا تشویق
        /// </summary>
        public int UnitValue { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        [MaxLength(4096)]
        public string? Description { get; set; }
        /// <summary>
        /// شناسه فایل اکسل در صورتی که تنبیه یا تشویق گروهی است
        /// </summary>
      //  [ForeignKey("ImportedFile")]
        public long? ImportedFileId { get; set; }

        /// <summary>
        /// آیا این رکورد موردی هست یا گروهی
        /// </summary>
        public bool IsGroup { get; set; }


    
   
        /// <summary>
        /// شناسه فایل اکسل در صورتی که تنبیه یا تشویق گروهی است
        /// </summary>
    
        public long? GroupPunishmentEncourageId { get; set; }
        public string? GroupPunishmentEncourage { get; set; }


      
        public long? OrganisationAgentOfPunishmentEncourageScoreIntervalId { get; set; }
        public string? OrganisationAgentOfPunishmentEncourageScoreInterval { get; set; }

    

        public int Value { get; set; }
    }
}
