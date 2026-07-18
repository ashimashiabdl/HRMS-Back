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
    public class OtherVeteranDTO : BaseDTO
    {
        
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        
        public long EmployeeId { get; set; }
        
        public bool? IsLast { get; set; }
        
        public long? VeteranTypeId { get; set; }
        public string?  VeteranTypeTitle { get; set; }
        
        public string? Descriptions { get; set; }
        public int? DurationYear { get; set; }
        public int? DurationMonth { get; set; }
        public int? DurationDay { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsComputeable { get; set; }
        
        public long? ConfirmerOrganId { get; set; }
        public string?  ConfirmerOrganTitle { get; set; }

        public string? LetterNumber { get; set; }
        public int? SacrificePercent { get; set; }
        
        public string? TrackingCode { get; set; }
        public DateTime? LetterDate { get; set; }
    }
}
