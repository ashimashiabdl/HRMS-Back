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
    [Table("War", Schema = "emp")]
    public class War : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public War()
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
        public long? WarTypeId { get; set; }
        public long? WarLocationId { get; set; }



    
        public long? ConfirmerOrganId { get; set; }
        [StringLength(256)]
        public string? JebheOperations { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? LetterDate { get; set; }
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LetterNumber { get; set; } = string.Empty;
        public double? PercentAnnualIncrease { get; set; } = 0d;
        public int? DurationYear { get; set; } = 0;

        public int? DurationMonth { get; set; } = 0;

        public int? DurationDay { get; set; } = 0;

        [Column(TypeName = "date")]
        public DateTime? WarDateFrom { get; set; }
        [Column(TypeName = "date")]
        public DateTime? WarDateTo { get; set; }
        public bool? IsContinues { get; set; } = false;
        [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Descriptions { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        [ForeignKey("EducationGrade")]
        public long? EducationGradeId { get; set; }
        public virtual EducationGrade? EducationGrade { get; set; }
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? TrackingCode { get; set; } = string.Empty;
        [StringLength(256)]
        public string? AcceptableDurationForTaxExemption { get; set; } = string.Empty;

        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
