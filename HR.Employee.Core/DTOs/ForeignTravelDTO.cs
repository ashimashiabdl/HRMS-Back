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
    public class ForeignTravelDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string?  OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public string? Descriptions { get; set; }
        public DateTime? LetterDate { get; set; }
        public string? LetterNumber { get; set; }
        public long? PlaceId { get; set; }
        public string?  PlaceTitle { get; set; }
        [StringLength(256)]
        public string? CountryNames { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? StatusId { get; set; }
        public string? StatusTitle { get; set; }
        public long? TravelTypeId { get; set; }
        public string? TravelTypeTitle { get; set; }
        [StringLength(1024)]
        public string? MissionSubject { get; set; }
        public int? MissionCost { get; set; }
        public long? MissionTypeId { get; set; }
        public string?  MissionTypeTitle { get; set; }
        public long? ReasonId { get; set; }
        public string?  ReasonTitle { get; set; }
        public int? CountryCount { get; set; }
        public int? ArchiveId { get; set; }
        [StringLength(1024)]
        public string? CountryList { get; set; }
    }
}
