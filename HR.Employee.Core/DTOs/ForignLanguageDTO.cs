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
    public class ForignLanguageDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string?  OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public DateTime Initdate { get; set; }
        public DateTime? Expiredate { get; set; }
        public long? LanguageId { get; set; }
        public string?  LanguageTitle { get; set; }
        public long? LanguageskillId { get; set; }
        public string? LanguageskillTitle { get; set; }
        public long? LevelId { get; set; }
        public string? LevelTitle { get; set; }
        public string? Languagescore { get; set; }
        public string? OtherLanguageName { get; set; }
        public long? ApprovedbyId { get; set; }
        public string?  ApprovedbyTitle { get; set; }
        public bool? Acceptable { get; set; }
        public string? Description { get; set; }
    }
}
