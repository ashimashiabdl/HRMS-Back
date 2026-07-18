using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities
{
    [Table("Insurance_Detail", Schema = "emp")]
    public class InsuranceDetail : BaseEntity, IignoreDateRangeValidation
    {
            public InsuranceDetail()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("Insurance")]
        public long InsuranceId { get; set; }
        public virtual Insurance? Insurance { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        public long? StatusId { get; set; }
        public long? InsuranceTypeRecordId { get; set; }
        public int? Year { get; set; } = 0;
        public int? Month { get; set; } = 0;
        public int? AccDay { get; set; } = 0;
        [Column(TypeName = "date")]
        public DateTime? InsuranceStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? InsuranceEndDate { get; set; }
        public bool? IsFullInsurnce { get; set; } = false;
        public bool? IsComputable { get; set; } = false;
        public bool? IsOptionalInsurnce { get; set; } = false;
        [StringLength(256)]
        public string? Desc { get; set; } = string.Empty;
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
