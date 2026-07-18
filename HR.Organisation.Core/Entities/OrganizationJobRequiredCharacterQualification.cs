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
    /// جدول شماره 17 شخصیت مورد نیاز
    /// </summary>
    [Table("OrganizationJob_Required_Character_Qualification", Schema = "Org")]
    public class OrganizationJobRequiredCharacterQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long CharacterTypeId { get; set; }
        public virtual BaseTableValue? CharacterType { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long RequiredLevelId { get; set; }
        public virtual BaseTableValue? RequiredLevel { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
