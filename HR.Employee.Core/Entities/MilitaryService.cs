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
    [Table("Military_Service", Schema = "emp")]
    public class MilitaryService : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
    {
            public MilitaryService()
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
        public long? MilitaryStatusCodeId { get; set; }
        public long? ImmunityTypeId { get; set; }
        public long? ConfirmerOrganID { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [StringLength(256)]
        public string? NameOfPeriod { get; set; } = string.Empty;
        [StringLength(128)]
        public string? MilitaryDuration { get; set; } = string.Empty;
        [StringLength(128)]
        public string MilitaryFullDuration { get; set; } = string.Empty;
        [StringLength(128)]
        public string? MilitaryMinDuration { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? ConfirmedLetterDate { get; set; }
        [StringLength(128)]
        public string? ConfirmedLetterNo { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        public DateTime? MilitaryStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? MilitaryEndDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? MilitariIssueDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ImmunityValidDate { get; set; }
        public long? MilitariGradeTypeId { get; set; }
        [StringLength(128)]
        public string? MilitariSerialNo { get; set; } = string.Empty;
        [StringLength(512)]
        public string? Descriptions { get; set; } = string.Empty;
        public bool? IsContinue { get; set; } = false;
        public bool? IsLast { get; set; } = false;
        public long? DueTypeId { get; set; }
        [ForeignKey("EducationGrade")]
        public long? EducationGradeId { get; set; }
        public virtual EducationGrade? EducationGrade { get; set; }
        public bool? IsComputable { get; set; } = false;

        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ToDate { get; set; }
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
