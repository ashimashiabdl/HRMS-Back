using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Attribute;

namespace HR.Employee.Core.DTOs
{
    public class MilitaryServiceDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? MilitaryStatusCodeId { get; set; }
        public string?  MilitaryStatusCodeTitle { get; set; }
        public long? ImmunityTypeId { get; set; }
        public string?  ImmunityTypeTitle { get; set; }
        [StringLength(256)]
        public string? NameOfPeriod { get; set; }
        [StringLength(128)]
        public string? MilitaryDuration { get; set; }
        [StringLength(128)]
        public string MilitaryFullDuration { get; set; } = null!;
        [StringLength(128)]
        public string? MilitaryMinDuration { get; set; }
        public DateTime? ConfirmedLetterDate { get; set; }
        [StringLength(128)]
        public string? ConfirmedLetterNo { get; set; }
        public DateTime? MilitaryStartDate { get; set; }
        public DateTime? MilitaryEndDate { get; set; }
        public DateTime? MilitariIssueDate { get; set; }
        public DateTime? ImmunityValidDate { get; set; }
        public long? ConfirmerOrganID { get; set; }
        
        public string? ConfirmerOrgan { get; set; }
        public long? MilitariGradeTypeId { get; set; }
        public string? MilitariGradeType { get; set; }
        [StringLength(128)]
        public string? MilitariSerialNo { get; set; }
        [StringLength(512)]
        public string? Descriptions { get; set; }
        public bool? IsContinue { get; set; }
        public bool? IsLast { get; set; }
        public long? DueTypeId { get; set; }
        public string?  DueTypeTitle { get; set; }
        public long? EducationGradeId { get; set; }
        public string?  EducationGradeTitle { get; set; }
        public bool? IsComputable { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
