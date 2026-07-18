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
    /// شرایط احراز رشته تحصیلی شغل
    /// </summary>
    [Table("OrganizationJob_Education_Field_Qualification", Schema = "Org")]
    public class OrganizationJobEducationFieldQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [ForeignKey("EducationField")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EducationFieldId { get; set; }
        public virtual EducationField? EducationField { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
