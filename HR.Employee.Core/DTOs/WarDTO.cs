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
    public class WarDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? WarTypeId { get; set; }
        public string? WarTypeTitle { get; set; }
        public long? WarLocationId { get; set; }
        public string? WarLocation { get; set; }

        public long? ConfirmerOrganID { get; set; }
        public string? ConfirmerOrganTitle { get; set; }
        public string? JebheOperations { get; set; }
        public DateTime? LetterDate { get; set; }
        public string? LetterNumber { get; set; }
        public double? PercentAnnualIncrease { get; set; }
        public int? DurationYear { get; set; }
        public int? DurationMonth { get; set; }
        public int? DurationDay { get; set; }
        [Column(TypeName = "date")]
        public DateTime? WarDateFrom { get; set; }
        [Column(TypeName = "date")]
        public DateTime? WarDateTo { get; set; }
        public bool? IsContinues { get; set; }
        [StringLength(500, ErrorMessage = "طول فیلد توضیحات می تواند حد اکثر 500 کاراکتر باشد")]
        public string? Descriptions { get; set; }
        public bool IsActive { get; set; }
        public long? EducationGradeId { get; set; }
        public string? EducationGradeTitle { get; set; }
        public string? TrackingCode { get; set; }
        [StringLength(256)]
        public string? AcceptableDurationForTaxExemption { get; set; }
        public long? WfStatusId { get; set; }
        public string? WfStatusTitle { get; set; }
    }
}
