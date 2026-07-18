using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("History_Stop", Schema = "emp")]
    public class HistoryStop : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public HistoryStop()
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
        public bool? IsComputable { get; set; } = false; 
        public int? HistoryStopDays { get; set; } = 0;
        public long? HistoryStopTypeId { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
