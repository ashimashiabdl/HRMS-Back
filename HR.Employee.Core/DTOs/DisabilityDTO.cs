using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class DisabilityDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? DisabilityLevelId { get; set; }
        public string? DisabilityLevelTitle { get; set; }
        public int? DisabilityPercent { get; set; }
        public DateTime? DisabilityStartDate { get; set; }
        public DateTime? DisabilityEndDate { get; set; }
        public bool IsLast { get; set; }
        public long? DisabilityTypeId { get; set; }
        public string? DisabilityTypeTitle { get; set; }
        public bool? HasCertification { get; set; }
    }
}
