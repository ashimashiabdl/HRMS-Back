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
    [Table("Character", Schema = "emp")]
    public class Character : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
        public Character()
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
        public long CharacterTypeId { get; set; }
        public virtual BaseTableValue? CharacterType { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long RequiredLevelId { get; set; }
        public virtual BaseTableValue? RequiredLevel { get; set; }

        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
