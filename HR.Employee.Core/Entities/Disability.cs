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
    [Table("Disability", Schema = "emp")]
    public class Disability : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Disability()
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
                public long? DisabilityLevelId { get; set; }

        public int? DisabilityPercent { get; set; } = 0;
        [Column(TypeName = "date")]
        public DateTime? DisabilityStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DisabilityEndDate { get; set; }
        public bool IsLast { get; set; } = false;
                public long? DisabilityTypeId { get; set; }

        public bool? HasCertification { get; set; } = false;
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
