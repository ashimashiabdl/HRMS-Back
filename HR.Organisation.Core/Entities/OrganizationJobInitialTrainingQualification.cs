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
    [Table("OrganizationJob_Initial_Course_Qualification", Schema = "Org")]
    public class OrganizationJobInitialCourseQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long CourseTypeId { get; set; }
        public virtual BaseTableValue? CourseType { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long CourseLevelId { get; set; }
        public virtual BaseTableValue? CourseLevel { get; set; }

        [NotMapped]
        private new string title { get; set; }

    }
}
