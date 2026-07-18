using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class EducationDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long? EducationGroupId { get; set; }
        public string? EducationGroupTitle { get; set; }
        public long EmployeeId { get; set; }

        /// <summary>
        /// مقطع تحصیلی
        /// </summary>

        public long? EducationGradeId { get; set; }
        public string? EducationGradeTitle { get; set; }
        /// <summary>
        /// مقطع تحصیلی موثر در حکم
        /// </summary>

        public long? EffectiveEducationGradeId { get; set; }
        public string? EffectiveEducationGradeTitle { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>

        public long? EducationFieldId { get; set; }
        public string? EducationFieldTitle { get; set; }
        /// <summary>
        /// گرایش تحصیلی
        /// </summary>

        public long? EducationOrientationId { get; set; }
        public string? EducationOrientationTitle { get; set; }
        /// <summary>
        /// وضعیت تحصیلی
        /// </summary>

        public long? EducationStateID { get; set; }
        public string? EducationStateTitle { get; set; }

        public DateTime? EducationFromDate { get; set; }

        public DateTime? EducationToDate { get; set; }
        [StringLength(8)]
        public string? EducationAverage { get; set; }

        public DateTime? EducationLicensePresentDate { get; set; }

        public DateTime? EducationLicenseImplDate { get; set; }

        public DateTime? EducationLicenseExpireDate { get; set; }
        public bool? IsInDutyTime { get; set; }

        public long? EducationPlacesId { get; set; }
        public string? EducationPlacesTitle { get; set; }

        public string? Descriptions { get; set; }
        public bool? IsBoursie { get; set; }

        public string? ThesisTitle { get; set; }

        public long? UniversityId { get; set; }
        public string? UniversityTitle { get; set; }
        public long? UniversityTypeID { get; set; }
        public string? UniversityTypeTitle { get; set; }
        /// <summary>
        /// سطح دانشگاه
        /// </summary>
        public long? UniversityLevelId { get; set; }
        public string? UniversityLevelTitle { get; set; }
        public bool? IsDefaultEducation { get; set; }
        public bool? IsUsedInOrder { get; set; }
        public bool? SetByEmployee { get; set; }

        public long? LicenceTypeId { get; set; }
        public string? LicenceTypeTitle { get; set; }
        [StringLength(128)]
        public string? LicenceNumber { get; set; }
        [StringLength(128)]
        public string? OtherUniversityName { get; set; }

        public long? KindOfGraduationId { get; set; }
        public string? KindOfGraduationTitle { get; set; }

        public long? ThesisGradeTypeId { get; set; }
        public string? ThesisGradeTypeTitle { get; set; }

        public long? ThesisGradeValueId { get; set; }
        public string? ThesisGradeValueTitle { get; set; }
    }
}
