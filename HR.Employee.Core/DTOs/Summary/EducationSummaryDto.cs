using System;

namespace HR.Employee.Core.DTOs.Summary
{
    public class EducationSummaryDto
    {
        public string EducationGrade { get; set; }
        public string EffectiveEducationGrade { get; set; }
        public string EducationField { get; set; }
        public string EducationOrientation { get; set; }
        public string EducationState { get; set; }
        public string UniversityType { get; set; }
        public string University { get; set; }
        public string EducationAverage { get; set; }
        public DateTime? GraduationDate { get; set; } // Assuming EducationToDate is graduation date
        public string ThesisTitle { get; set; }
        public string Descriptions { get; set; }
    }
}
