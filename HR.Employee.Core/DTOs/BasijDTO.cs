using HR.BaseInfo.Core.Entities;
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
    public class BasijDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public string? EmployeeTitle { get; set; }
        public long? BasijTypeId { get; set; }
        public string? BasijTypeTitle { get; set; }
        public int? DurationYear { get; set; }
        public int? DurationMonth { get; set; }
        public int? DurationDay { get; set; }
        public string? Descriptions { get; set; }
        public DateTime? LetterDate { get; set; }
        public string? LetterNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsContinues { get; set; }
        public long? BasijPlaceId { get; set; }
        public string? BasijPlaceTitle { get; set; }
        public long? ConfirmerOrganID { get; set; }
        public string?  ConfirmerOrganTitle { get; set; }
        public bool IsActive { get; set; }
        public bool IsComputeableInHistory { get; set; }
        public int? YearCoefficient { get; set; }
        public int? Year { get; set; }
        public bool IsPercent { get; set; }
        public string? TrackingCode { get; set; }

    }
}
