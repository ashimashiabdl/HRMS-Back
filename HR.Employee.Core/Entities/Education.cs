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
    [Table("Education", Schema = "emp")]
    public class Education : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
        public Education()
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
        public virtual Employee? Employee { get; set; }
        /// <summary>
        /// گروه رشته تحصیلی
        /// </summary>
        [ForeignKey("EducationGroup")]
        public long? EducationGroupId { get; set; }
        public virtual EducationGroup? EducationGroup { get; set; }
        /// <summary>
        /// مقطع تحصیلی
        /// </summary>
        [ForeignKey("EducationGrade")]
        public long? EducationGradeId { get; set; }
        public virtual EducationGrade? EducationGrade { get; set; }
        /// <summary>
        /// مقطع تحصیلی موثر در حکم
        /// </summary>
        [ForeignKey("EffectiveEducationGrade")]
        public long? EffectiveEducationGradeId { get; set; }
        public virtual EducationGrade? EffectiveEducationGrade { get; set; }
        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        [ForeignKey("EducationField")]
        public long? EducationFieldId { get; set; }
        public virtual EducationField? EducationField { get; set; }
        /// <summary>
        /// گرایش تحصیلی
        /// </summary>
        [ForeignKey("EducationOrientation")]
        public long? EducationOrientationId { get; set; }
        public virtual EducationOrientation? EducationOrientation { get; set; }
        /// <summary>
        /// ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½
        /// </summary>
        public long? EducationStateID { get; set; }

        public virtual BaseTableValue? EducationState { get; set; }


        public long? UniversityLevelId { get; set; }
        public virtual BaseTableValue? UniversityLevel { get; set; }



        /// <summary>
        /// ظ†ظˆط¹ ط¯ط§ظ†ط´ع¯ط§ظ‡ / ظ…ظˆط³ط³ظ‡
        /// </summary>
        public long? UniversityTypeID { get; set; }
        public virtual BaseTableValue? UniversityType { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EducationFromDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EducationToDate { get; set; }
        [StringLength(8)]
        public string? EducationAverage { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? EducationLicensePresentDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EducationLicenseImplDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EducationLicenseExpireDate { get; set; }
        public bool? IsInDutyTime { get; set; } = false;
        [ForeignKey("EducationPlaces")]
        public long? EducationPlacesId { get; set; }
        public virtual Places? EducationPlaces { get; set; }
        [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Descriptions { get; set; } = string.Empty;
        public bool? IsBoursie { get; set; } = false;
        [StringLength(256)]
        public string? ThesisTitle { get; set; } = string.Empty;
        [ForeignKey("University")]
        public long? UniversityId { get; set; }
        public virtual University? University { get; set; }
        public bool? IsDefaultEducation { get; set; } = false;
        public bool? IsUsedInOrder { get; set; } = false;
        public bool? SetByEmployee { get; set; } = false;
        public long? LicenceTypeId { get; set; }

        [StringLength(128)]
        public string? LicenceNumber { get; set; } = string.Empty;
        [StringLength(128)]
        public string? OtherUniversityName { get; set; } = string.Empty;
        public long? KindOfGraduationId { get; set; }
        public long? ThesisGradeTypeId { get; set; }
        public long? ThesisGradeValueId { get; set; }

        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
