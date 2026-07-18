using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("Employee_Software", Schema = "emp")]
    public class EmployeeSoftware : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public EmployeeSoftware()
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
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long SoftwareId { get; set; }
        public virtual BaseTableValue? Software { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long SoftwareTypeId { get; set; }
        public virtual BaseTableValue? SoftwareType { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long MasteryLevelTypeId { get; set; }
        public virtual BaseTableValue? MasteryLevelType { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
