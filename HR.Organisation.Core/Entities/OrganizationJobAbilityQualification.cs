using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    /// <summary>
    /// جدول شماره 12 پنج سطحی باشد
    /// </summary>
    [Table("OrganizationJob_Ability_Qualification", Schema = "Org")]
    public class OrganizationJobAbilityQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long AbilityTypeId { get; set; }
        public virtual BaseTableValue? AbilityType { get; set; }


        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long LevelTypeId { get; set; }
        public virtual BaseTableValue? LevelType { get; set; }
        
        [NotMapped]
        private new string title { get; set; }
    }
}
