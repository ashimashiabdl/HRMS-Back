using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganizationJobInitialCourseQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long CourseTypeId { get; set; }
        public string? CourseType { get; set; }
        public long CourseLevelId { get; set; }
        public string? CourseLevel { get; set; }
    }
}
