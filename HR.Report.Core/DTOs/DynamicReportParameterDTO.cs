using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Report.Core.DTOs
{
    public class DynamicReportParameterDTO : BaseDTO
    {
        public long DynamicReportId { get; set; }
        public string? DynamicReport { get; set; }
        public bool Optional { get; set; }
        [StringLength(256)]
        public string? DefaultValue { get; set; }
        public long ParameterId { get; set; }
        public string? Parameter { get; set; }
    }
}
