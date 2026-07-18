using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
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
    [Table("Competency", Schema = "emp")]
    public class Competency : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Competency()
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
        public virtual Employee? Employee { get; set; }public virtual BaseTableValue? CompetencyLevel { get; set; }public virtual BaseTableValue? CompetencyType { get; set; }
        public long? CompetencyLevelId { get; set; }
        public long? CompetencyTypeId { get; set; }


        [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; } = string.Empty;

        public bool? Acceptable { get; set; } = false;
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
