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
    [Table("Other_Veteran", Schema = "emp")]
    public class OtherVeteran : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public OtherVeteran()
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
        public bool? IsLast { get; set; } = false;
        public long? VeteranTypeId { get; set; }
        [StringLength(500, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 500 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Descriptions { get; set; } = string.Empty;
        public int? DurationYear { get; set; } = 0;
        public int? DurationMonth { get; set; } = 0;
        public int? DurationDay { get; set; } = 0;
        public bool? IsActive { get; set; } = false;
            public bool? IsComputeable { get; set; } = false;
        public long? ConfirmerOrganId { get; set; }
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LetterNumber { get; set; } = string.Empty;
        public int? SacrificePercent { get; set; } = 0;
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? TrackingCode { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? LetterDate { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
