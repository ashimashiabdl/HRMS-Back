using HR.Employee.Core.DTOs.Summary;
using HR.Employee.Core.Entities;
using System.Collections.Generic;

namespace HR.Employee.Core.DTOs
{
    public class EmployeeSummaryDto
    {
        public EmployeeDetailsSummaryDto Employee { get; set; }
        public List<AbilitySummaryDto> Abilities { get; set; }
        public List<AbsenceRecord> AbsenceRecords { get; set; }
        public List<Appearance> Appearances { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<BankAccountSummaryDto> BankAccounts { get; set; }
        public List<Basij> Basijs { get; set; }
        public List<BasijGrade> BasijGrades { get; set; }
        public List<Captivity> Captivities { get; set; }
        public List<Character> Characters { get; set; }
        public List<Coefficient> Coefficients { get; set; }
        public List<Competency> Competencies { get; set; }
        public List<ContactInfo> ContactInfos { get; set; }
        public List<Course> Courses { get; set; }
        public List<Disability> Disabilities { get; set; }
        public List<DrivingLicense> DrivingLicenses { get; set; }
        public List<EducationSummaryDto> Educations { get; set; }
        public List<EmployeeFile> EmployeeFiles { get; set; }
        public List<EmployeeLoginHistory> EmployeeLoginHistories { get; set; }
        public List<EmployeeSoftware> EmployeeSoftwares { get; set; }
        public List<EvaluationResult> EvaluationResults { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Family> Families { get; set; }
        public List<HR.Employee.Core.Entities.File> Files { get; set; }
        public List<ForeignTravel> ForeignTravels { get; set; }
        public List<ForeignLanguage> ForeignLanguages { get; set; }
        public List<GroupPunishmentEncourage> GroupPunishmentEncourages { get; set; }
        public List<HistoryStop> HistoryStops { get; set; }
        public List<Image> Images { get; set; }
        public List<Insurance> Insurances { get; set; }
        public List<InsuranceDetail> InsuranceDetails { get; set; }
        public List<Isar> Isars { get; set; }
        public List<MilitaryService> MilitaryServices { get; set; }
        public List<OtherVeteran> OtherVeterans { get; set; }
        public List<PunishmentEncourage> PunishmentEncourages { get; set; }
        public List<War> Wars { get; set; }
        public List<Work> Works { get; set; }
    }
}
