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
    [Table("Course", Schema = "emp")]
    public class Course : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Course()
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
        public virtual Employee? Employee { get; set; }public virtual BaseTableValue? CourseStatus { get; set; }public virtual BaseTableValue? CourseLicense { get; set; }
        [StringLength(8)]
        public string? CourseMark { get; set; } = string.Empty;
        public int? CourseTime { get; set; } = 0;public virtual BaseTableValue? CourseRegType { get; set; }
        [ForeignKey("EducationGrade")]
        public long? EducationGradeId { get; set; }
        public virtual EducationGrade? EducationGrade { get; set; }
        [StringLength(128)]
        public string? CoursepPlace { get; set; } = string.Empty;
        public int? CourseSession { get; set; } = 0;
        [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; } = string.Empty;
        public long? CourseTypeId { get; set; }
        public long? CourseStatusId { get; set; }
        public long? CourseLicenseId { get; set; }
        public long? CourseRegTypeId { get; set; }
        public long? CourseId { get; set; }

        [StringLength(16)]
        public string? CourseSerial { get; set; } = string.Empty;public virtual BaseTableValue? CourseType { get; set; }public virtual BaseTableValue? CourseTitle { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
