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
    [Table("OrganizationJob_Foreign_Language_Qualification", Schema = "Org")]
    public class OrganizationJobForeignLanguageQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long LanguageTypeId { get; set; }
        public virtual BaseTableValue? LanguageType { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long LanguageLevelTypeId { get; set; }
        public virtual BaseTableValue? LanguageLevelType { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long LanguageSkillTypeId { get; set; }
        public virtual BaseTableValue? LanguageSkillType { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
