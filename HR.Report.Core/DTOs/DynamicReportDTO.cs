using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Core.DTOs
{
    public class DynamicReportDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? OrganisationMRTId { get; set; }
        public string? OrganisationMRT { get; set; }

        public long FuctionTypeId { get; set; }
        public string? FuctionType { get; set; }
        public long ExportTypeId { get; set; }

        public string? ExportType { get; set; }
        public string? SqlQuery { get; set; }
        [StringLength(256)]
        public string? EnglishName { get; set; }
        [StringLength(32)]
        public string? Schema { get; set; }
        [StringLength(255)]
        public string? FunctionName { get; set; }
        [StringLength(1024)]
        public string? Help { get; set; }
        public bool IsActive { get; set; }
        public List<DynamicReportParameterDTO>? DynamicReportParameterList { get; set; }
    }
}
