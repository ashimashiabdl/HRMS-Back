using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Attribute;

namespace HR.Organisation.Core.DTOs
{
    public class OrganizationJobDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? JobId { get; set; }
        public string? Job { get; set; }
        public long? StateId { get; set; }
        public string? State { get; set; }
        public long? InsuranceDesketid { get; set; }
        public long? StaffingRuleId { get; set; }
        public string? StaffingRule { get; set; }
        public long? OrganisationJobSeriesId { get; set; }
        public string? OrganisationJobSeries { get; set; }
        public long? OrganisationJobGroupId { get; set; }
        public string? OrganisationJobGroup { get; set; }
        public long? OrganisationJobCategoryId { get; set; }
        public string? OrganisationJobCategory { get; set; }
        public long? JobNatureId { get; set; }
        public string? JobNature { get; set; }
        public long? BasQualificationGenderId { get; set; }
        public string? BasQualificationGender { get; set; }
        public long? CoefficientOfJobTypeId { get; set; }
        public string? CoefficientOfJobType { get; set; }
        public string? Code { get; set; }
        public string? SystemCode { get; set; }
        /// <summary>
        /// کد تفضیل شغل
        /// </summary>
        public string? JobFinancialCode { get; set; }
        public string? TotalJobCode { get; set; }
        /// <summary>
        /// شرح شغل
        /// </summary>
        public string? JobDescriptions { get; set; }
        public int Capacity { get; set; }
        public int Order { get; set; }
        public int JobDegree { get; set; }
        public int FilledCapacity { get; set; }
        /// <summary>
        /// مشاغل سخت جهت استفاده در دیسکت بیمه
        /// </summary>
        public bool IsDifficultJob { get; set; }

        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public int ExperienceInYears { get; set; }
        public int ExperienceInMonths { get; set; }

        public long? TaminInsuranceJobListId { get; set; }
        public string? TaminInsuranceJobList { get; set; }
        public long? TaxOccupationId { get; set; }
        public string? TaxOccupation { get; set; }
        public long? JobActivityTypeId { get; set; }
        public string? JobActivityType { get; set; }
        public long? JobLevelId { get; set; }
        public string? JobLevel { get; set; }

        /// <summary>
        /// حداکثر مقطع تحصیلی
        /// </summary>
        public long? MaxEducationGradeId { get; set; }
        public string? MaxEducationGrade { get; set; }

        /// <summary>
        /// حوزه فرآیندی
        /// </summary>
        public long? ProcessAreaId { get; set; }
        public string? ProcessArea { get; set; }

        public int JobMatchingBaseNumber { get; set; }
    }
}
