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
    [Table("Basij", Schema = "emp")]
    public class Basij : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId , IignoreDateRangeValidation
    {
            public Basij()
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
        public virtual Employee? Employee { get; set; }public virtual BaseTableValue? BasijType { get; set; }
        public int? DurationYear { get; set; } = 0;
        public int? DurationMonth { get; set; } = 0;
        public int? DurationDay { get; set; } = 0;
        [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Descriptions { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? LetterDate { get; set; }
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LetterNumber { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ToDate { get; set; }
        public bool? IsContinues { get; set; } = false;public virtual BaseTableValue? BasijPlace { get; set; }
        public long? ConfirmerOrganID { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? ConfirmerOrgan { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsComputeableInHistory { get; set; } = false;
        public int? YearCoefficient { get; set; } = 0;
        public int? Year { get; set; } = 0;
        public bool IsPercent { get; set; } = false;
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? TrackingCode { get; set; } = string.Empty; public long? BasijTypeId { get; set; }        public long? BasijPlaceId { get; set; }

        [NotMapped]
        private new string title { get; set; } = string.Empty;

    }
}
