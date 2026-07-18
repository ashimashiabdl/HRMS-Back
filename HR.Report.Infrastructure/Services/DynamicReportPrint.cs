using System.Globalization;
using HR.Report.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;

namespace HR.Report.Infrastructure.Services;

public class DynamicReportPrint
{
    public byte[] GeneratePdf(
        OrganisationMRT mrtTemplate,
        string connectionString,
        string? fontsDirectory,
        long currentUserId,
        string? userName,
        IEnumerable<DynamicReportParameterStandard>? parameters)
    {
        ValidateInputs(mrtTemplate, connectionString);

        return StimulsoftBootstrap.ExecuteExclusive(() =>
        {
            try
            {
                using var report = CreateReport(
                    mrtTemplate,
                    connectionString,
                    currentUserId,
                    userName,
                    parameters);

                return ExportReportToPdf(report);
            }
            catch (Exception ex) when (ex is InvalidOperationException or DirectoryNotFoundException or ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"خطا در تولید PDF گزارش پویا: {ex.Message}", ex);
            }
        }, fontsDirectory);
    }

    private static void ValidateInputs(OrganisationMRT mrtTemplate, string connectionString)
    {
        if (mrtTemplate == null)
            throw new ArgumentNullException(nameof(mrtTemplate), "قالب گزارش (MRT) یافت نشد.");

        if (mrtTemplate.Content == null || mrtTemplate.Content.Length == 0)
            throw new InvalidOperationException("محتوای قالب گزارش (MRT) خالی است.");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("رشته اتصال پایگاه داده برای تولید گزارش مشخص نشده است.");
    }

    private static StiReport CreateReport(
        OrganisationMRT mrtTemplate,
        string connectionString,
        long currentUserId,
        string? userName,
        IEnumerable<DynamicReportParameterStandard>? parameters)
    {
        var report = new StiReport
        {
            IsRendered = true,
        };

        try
        {
            report.Load(mrtTemplate.Content);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در بارگذاری قالب گزارش: {ex.Message}", ex);
        }

        StiReportUtilities.SanitizeForServer(report);

        ApplyReportParameters(report, parameters);
        ApplySystemVariables(report, currentUserId, userName);
        ApplyConnectionString(report, connectionString);

        try
        {
            StiReportUtilities.CompileSafe(report);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در کامپایل قالب گزارش: {ex.Message}", ex);
        }

        try
        {
            report.Render(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در رندر گزارش: {ex.Message}", ex);
        }

        return report;
    }

    private static void ApplyReportParameters(
        StiReport report,
        IEnumerable<DynamicReportParameterStandard>? parameters)
    {
        if (parameters == null)
            return;

        try
        {
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.title))
                    continue;

                var csv = parameter.SelectedValues != null
                    ? string.Join(",", parameter.SelectedValues.Select(v => v?.ToString()))
                    : string.Empty;

                SetOrAddVariable(report, parameter.title, csv);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در تنظیم پارامترهای گزارش: {ex.Message}", ex);
        }
    }

    private static void ApplySystemVariables(StiReport report, long currentUserId, string? userName)
    {
        try
        {
            SetOrAddVariable(report, "CurrentUserId", currentUserId);
            SetOrAddVariable(report, "UserName", userName ?? string.Empty);
            SetOrAddVariable(report, "CurrentTimeStamp", BuildPersianTimestamp());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در تنظیم متغیرهای سیستمی گزارش: {ex.Message}", ex);
        }
    }

    private static string BuildPersianTimestamp()
    {
        var createDate = DateTime.Now;
        var persianCalendar = new PersianCalendar();
        var datePart = string.Format(
            "{0}/{1}/{2}",
            persianCalendar.GetYear(createDate),
            persianCalendar.GetMonth(createDate).ToString("00"),
            persianCalendar.GetDayOfMonth(createDate).ToString("00"));

        var timePart = string.Format(
            "{0}:{1}:{2}",
            createDate.Hour.ToString("00"),
            createDate.Minute.ToString("00"),
            createDate.Second.ToString("00"));

        return $"{datePart} - {timePart}";
    }

    private static void SetOrAddVariable(StiReport report, string name, object value)
    {
        var existing = report.Dictionary.Variables.Items
            .OfType<StiVariable>()
            .FirstOrDefault(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            existing.ValueObject = value;
            return;
        }

        report.Dictionary.Variables.Add(name, value?.ToString() ?? string.Empty);
    }

    private static void ApplyConnectionString(StiReport report, string connectionString)
    {
        try
        {
            foreach (StiDatabase db in report.Dictionary.Databases)
            {
                if (db is StiSqlDatabase sqlDatabase)
                    sqlDatabase.ConnectionString = connectionString;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در تنظیم اتصال پایگاه داده گزارش: {ex.Message}", ex);
        }
    }

    private static byte[] ExportReportToPdf(StiReport report)
    {
        var settings = new StiPdfExportSettings
        {
            EmbeddedFonts = true,
            StandardPdfFonts = true,
        };

        try
        {
            using var stream = new MemoryStream();
            report.ExportDocument(StiExportFormat.Pdf, stream, settings);
            var pdfBytes = stream.ToArray();

            if (pdfBytes.Length == 0)
                throw new InvalidOperationException("خروجی PDF خالی است.");

            return pdfBytes;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"خطا در خروجی PDF گزارش: {ex.Message}", ex);
        }
    }
}
