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
    /// شرایط احراز مقطع تحصیلی شغل
    /// </summary>
    [Table("OrganizationJob_Education_Grade_Qualification", Schema = "Org")]
    public class OrganizationJobEducationGradeQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; } 
        
        
        [ForeignKey("EducationGrade")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EducationGradeId { get; set; }
        public virtual EducationGrade? EducationGrade { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
