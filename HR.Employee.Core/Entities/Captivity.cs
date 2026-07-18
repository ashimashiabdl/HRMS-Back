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
    [Table("Captivity", Schema = "emp")]
    public class Captivity : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Captivity()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
        public long? OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
                public long? CaptivityLocationId { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public int? Year { get; set; } = 0;
        public int? Month { get; set; } = 0;
        public int? Day { get; set; } = 0;
        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ToDate { get; set; }public virtual BaseTableValue? CaptivityLocation { get; set; }
        [StringLength(500)]
        public string? Description { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? LetterDate { get; set; }
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LetterNumber { get; set; } = string.Empty;
        public long? ConfirmerOrganID { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? ConfirmerOrgan { get; set; }
        public bool? IsContinues { get; set; } = false;
        public double? SacrificePercent { get; set; } = 0d;
        public bool IsActive { get; set; } = false;
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? TrackingCode { get; set; } = string.Empty;
        [ForeignKey("EducationGrade")]
        public long? EducationGradeId { get; set; }
        public virtual EducationGrade? EducationGrade { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
