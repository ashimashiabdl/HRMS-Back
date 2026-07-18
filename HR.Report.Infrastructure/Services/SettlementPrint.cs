using System.Drawing;
using Hr.SystemSetting.Core.Entities;
using HR.Organisation.Core.Entities;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;

namespace HR.Report.Infrastructure.Services;

public class SettlementPrint
{
    public byte[] GetSettlementPrint(
        long settlementId,
        long interdictOrderId,
        OrganisationMRT mrtTemplate,
        OrganisationChartImage? organImage,
        HR.Employee.Core.Entities.Image? employeeImage,
        string? fontsDirectory,
        string connectionString)
    {
        ValidateInputs(settlementId, interdictOrderId, mrtTemplate, connectionString);

        return StimulsoftBootstrap.ExecuteExclusive(() =>
        {
            try
            {
                using var report = CreateReport(
                    settlementId,
                    interdictOrderId,
                    mrtTemplate,
                    organImage,
                    employeeImage,
                    connectionString);
                return ExportReportToPdf(report);
            }
            catch (Exception ex) when (ex is InvalidOperationException or DirectoryNotFoundException or ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"خطا در تولید PDF تسویه حساب (شناسه تسویه: {settlementId}): {ex.Message}",
                    ex);
            }
        }, fontsDirectory);
    }

    private static void ValidateInputs(
        long settlementId,
        long interdictOrderId,
        OrganisationMRT mrtTemplate,
        string connectionString)
    {
        if (settlementId <= 0)
            throw new ArgumentOutOfRangeException(nameof(settlementId), "شناسه تسویه حساب معتبر نیست.");

        if (interdictOrderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(interdictOrderId), "شناسه حکم مرتبط با تسویه یافت نشد.");

        if (mrtTemplate == null)
            throw new ArgumentNullException(nameof(mrtTemplate), "قالب چاپ تسویه (MRT) یافت نشد.");

        if (mrtTemplate.Content == null || mrtTemplate.Content.Length == 0)
            throw new InvalidOperationException("محتوای قالب چاپ تسویه (MRT) خالی است.");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("رشته اتصال پایگاه داده برای تولید گزارش مشخص نشده است.");
    }

    private static StiReport CreateReport(
        long settlementId,
        long interdictOrderId,
        OrganisationMRT mrtTemplate,
        OrganisationChartImage? organImage,
        HR.Employee.Core.Entities.Image? employeeImage,
        string connectionString)
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
            throw new InvalidOperationException($"خطا در بارگذاری قالب چاپ تسویه: {ex.Message}", ex);
        }

        StiReportUtilities.SanitizeForServer(report);

        SetReportVariables(report, settlementId, interdictOrderId);
        ApplyOrganisationLogo(report, organImage);
        ApplyEmployeePhoto(report, employeeImage);
        ApplyConnectionString(report, connectionString);

        try
        {
            StiReportUtilities.CompileSafe(report);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در کامپایل قالب چاپ تسویه: {ex.Message}", ex);
        }

        try
        {
            report.Render(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در رندر گزارش تسویه: {ex.Message}", ex);
        }

        return report;
    }

    private static void SetReportVariables(
        StiReport report,
        long settlementId,
        long interdictOrderId)
    {
        try
        {
            SetOrAddVariable(report, "SettlementId", settlementId);
            SetOrAddVariable(report, "EmployeeSettlementId", settlementId);
            SetOrAddVariable(report, "InterdictId", interdictOrderId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در تنظیم متغیرهای گزارش تسویه: {ex.Message}", ex);
        }
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

    private static void ApplyOrganisationLogo(StiReport report, OrganisationChartImage? organImage)
    {
        if (organImage?.Content is not { Length: > 0 })
            return;

        ApplyImageComponent(report, "logoIMG", organImage.Content);
    }

    private static void ApplyEmployeePhoto(
        StiReport report,
        HR.Employee.Core.Entities.Image? employeeImage)
    {
        if (employeeImage?.ImageData is not { Length: > 0 })
            return;

        ApplyImageComponent(report, "employeeImage", employeeImage.ImageData);
    }

    private static void ApplyImageComponent(
        StiReport report,
        string componentName,
        byte[] imageBytes)
    {
        if (report.GetComponentByName(componentName) is not StiImage imageComponent)
            return;

        try
        {
            using var stream = new MemoryStream(imageBytes);
            using var sourceImage = Image.FromStream(stream);
            imageComponent.Image = new Bitmap(sourceImage);
        }
        catch
        {
            // تصویر اختیاری است.
        }
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
