using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class CourseDTO : BaseDTO
    {
        
        public long OrganisationChartId { get; set; }
        public string?  OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? CourseStatusId { get; set; }
        
        public string? CourseStatusTitle { get; set; }
        public long? CourseLicenseId { get; set; }
        public string?  CourseLicenseTitle { get; set; }
        [StringLength(8)]
        public string? CourseMark { get; set; }
        public int? CourseTime { get; set; }
        
        public long? CourseRegTypeId { get; set; }
        public string?  CourseRegTypeTitle { get; set; }
        
        public long? EducationGradeId { get; set; }
        public string?  EducationGradeTitle { get; set; }
        [StringLength(128)]
        public string? CoursepPlace { get; set; }
        public int? CourseSession { get; set; }
 
        public string? Description { get; set; }
        
        public string? CourseSerial { get; set; }
        
        public long? CourseTypeId { get; set; }
        public string?  CourseTypeTitle { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
    }
}
