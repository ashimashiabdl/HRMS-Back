using HR.Report.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Core.DTOs
{
    public class DynamicReportParameterStandard
    {
        public long ParameterId { get; set; }
        public string? Parameter { get; set; }
        public string? title { get; set; }
        public bool Optional { get; set; }
        public List<string>? SelectedValues { get; set; }
    }
    public class RequestReportDTOStandard
    {
        public long Id { get; set; }
        public int ExportTypeId { get; set; }
        public long CurrentUserId { get; set; }
        public string? UserName { get; set; }
        public List<DynamicReportParameterStandard>? Parameters { get; set; }

    }
}
