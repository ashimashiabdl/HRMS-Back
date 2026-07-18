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
    public class CaptivityDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? CaptivityLocationId { get; set; }
        public string? CaptivityLocationTitle { get; set; }
        public string? Description { get; set; }
        public DateTime? LetterDate { get; set; }
        public string? LetterNumber { get; set; }
        public long? ConfirmerOrganID { get; set; }
        public string? ConfirmerOrganTitle { get; set; }
        public bool? IsContinues { get; set; }
        public double? SacrificePercent { get; set; }
        public bool IsActive { get; set; }
        public string? TrackingCode { get; set; }
        public long? EducationGradeId { get; set; }
        public string? EducationGradeTitle { get; set; }
    }
}