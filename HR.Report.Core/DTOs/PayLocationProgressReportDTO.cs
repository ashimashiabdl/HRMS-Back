using HR.SharedKernel.Data;

namespace HR.Report.Core.DTOs;

public class PayLocationProgressReportDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long? UploadedByUserId { get; set; }
    public string? UploadedByUser { get; set; }
    public string? ReportDesc { get; set; }
}

