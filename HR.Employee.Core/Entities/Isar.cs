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
    [Table("Isar", Schema = "emp")]
    public class Isar : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public Isar()
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
        public long? IsartypeId { get; set; }
        public long? ConfirmerOrganId { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IsarStartDate { get; set; }
        public float? Isarpercent { get; set; } = 0f;
        public long? IsarLocationId { get; set; }
        public long? IsarEquipmentId { get; set; }
        [StringLength(256)]
        public string? IsarInability { get; set; } = string.Empty;
        [StringLength(256)]
        public string? IsarInjuerdOrgan { get; set; } = string.Empty;
        [StringLength(512)]
        public string? Description { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? LetterDate { get; set; }
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LetterNumber { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? IsarEndDate { get; set; }
        public int? IsarDurationYear { get; set; } = 0;
        public int? IsarDurationMonth { get; set; } = 0;
        public int? IsarDurationDay { get; set; } = 0;
        public bool? IsContinues { get; set; } = false;
        public bool IsActive { get; set; } = false;
        [StringLength(50, ErrorMessage = "ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½ï؟½ ï؟½ï؟½ اکï؟½ï؟½ 50 ï؟½ï؟½ï؟½اکï؟½ï؟½ ï؟½ï؟½ï؟½ï؟½")]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? TrackingCode { get; set; } = string.Empty;
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
