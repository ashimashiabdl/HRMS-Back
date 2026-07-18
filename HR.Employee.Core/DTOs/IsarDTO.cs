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
    public class IsarDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string?  OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? IsartypeId { get; set; }
        public string?  IsartypeTitle { get; set; }
        public long? ConfirmerOrganId { get; set; }
        public string?  ConfirmerOrganTitle { get; set; }
        public DateTime? IsarStartDate { get; set; }
        public float? Isarpercent { get; set; }
        public long? IsarLocationId { get; set; }
        public string? IsarLocation { get; set; }
        public int? IsarEquipmentId { get; set; }
        [StringLength(256)]
        public string? IsarInability { get; set; }
        [StringLength(256)]
        public string? IsarInjuerdOrgan { get; set; }
        [StringLength(512)]
        public string? Description { get; set; }
        public DateTime? LetterDate { get; set; }
        [StringLength(50, ErrorMessage = "طول فیلد شماره نامه می تواند حد اکثر 50 کاراکتر باشد")]
        public string? LetterNumber { get; set; }
        public DateTime? IsarEndDate { get; set; }
        public int? IsarDurationYear { get; set; }
        public int? IsarDurationMonth { get; set; }
        public int? IsarDurationDay { get; set; }
        public bool? IsContinues { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50, ErrorMessage = "طول فیلد شماره نامه می تواند حد اکثر 50 کاراکتر باشد")]
        public string? TrackingCode { get; set; }
    }
}
