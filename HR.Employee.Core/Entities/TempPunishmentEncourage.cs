using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;
using HR.Organisation.Core.Entities;
using HR.BaseInfo.Core.Entities;
using Hr.SystemSetting.Core.Entities;

namespace HR.Employee.Core.Entities
{
    [Table("Temp_Punishment_Encourage", Schema = "emp")]
    public class TempPunishmentEncourage : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public TempPunishmentEncourage()
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
        public long? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        /// <summary>
        /// مقدار واحد تنبیه یا تشویق
        /// </summary>
        public int UnitValue { get; set; } = 0;
        public int? Value { get; set; } = 0;
        public string? NationalNo { get; set; } = string.Empty;
        [ForeignKey("TempFile")]
        public long? TempFileId { get; set; }
        public virtual File? TempFile { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
