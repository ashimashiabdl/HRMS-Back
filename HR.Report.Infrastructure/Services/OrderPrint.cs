using System.Drawing;
using System.Globalization;
using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using Hr.SystemSetting.Core.Entities;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Export;

namespace HR.Report.Infrastructure.Services;

public class OrderPrint
{
    private static readonly NumberFormatInfo FarsiNumberFormat = new()
    {
        NumberDecimalDigits = 0,
        NumberGroupSeparator = ",",
    };

    public string ToIranianDate(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return string.Empty;

        var persianCalendar = new PersianCalendar();
        return string.Format(
            "{0}/{1}/{2}",
            persianCalendar.GetYear(dateTime.Value),
            persianCalendar.GetMonth(dateTime.Value).ToString("00"),
            persianCalendar.GetDayOfMonth(dateTime.Value).ToString("00"));
    }

    public string SetFarsiNumber(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var chars = input.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] is >= '0' and <= '9')
                chars[i] = (char)(chars[i] + 1776 - '0');
        }

        return new string(chars);
    }

    public byte[] GetOrderPrint(
        InterdictOrder order,
        OrganisationMRT mrtTemplate,
        OrganisationChartImage? organImage,
        HR.Employee.Core.Entities.Image? employeeImage,
        string? fontsDirectory,
        string connectionString)
    {
        ValidateInputs(order, mrtTemplate, connectionString);

        return StimulsoftBootstrap.ExecuteExclusive(() =>
        {
            try
            {
                using var report = CreateReport(order, mrtTemplate, organImage, employeeImage, connectionString);
                return ExportReportToPdf(report);
            }
            catch (Exception ex) when (ex is InvalidOperationException or DirectoryNotFoundException or ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"خطا در تولید PDF حکم (شناسه حکم: {order.Id}): {ex.Message}",
                    ex);
            }
        }, fontsDirectory);
    }

    private static void ValidateInputs(
        InterdictOrder order,
        OrganisationMRT mrtTemplate,
        string connectionString)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "اطلاعات حکم برای چاپ یافت نشد.");

        if (mrtTemplate == null)
            throw new ArgumentNullException(nameof(mrtTemplate), "قالب چاپ حکم (MRT) یافت نشد.");

        if (mrtTemplate.Content == null || mrtTemplate.Content.Length == 0)
            throw new InvalidOperationException("محتوای قالب چاپ حکم (MRT) خالی است.");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("رشته اتصال پایگاه داده برای تولید گزارش مشخص نشده است.");
    }

    private static StiReport CreateReport(
        InterdictOrder order,
        OrganisationMRT mrtTemplate,
        OrganisationChartImage? organImage,
        HR.Employee.Core.Entities.Image? employeeImage,
        string connectionString)
    {
        // Minimal path matching the original working OrderPrint + ConvertToolBox.
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
            throw new InvalidOperationException($"خطا در بارگذاری قالب چاپ حکم: {ex.Message}", ex);
        }

        StiReportUtilities.SanitizeForServer(report);

        // Variables: same style as original (Add). Do not touch indexer before Compile.
        try
        {
            var sumWageFactors = order.SumWageFactors.HasValue && order.SumWageFactors.Value > 0
                ? SetFarsiNumberStatic(order.SumWageFactors.Value.ToString("N", FarsiNumberFormat))
                : "0";

            report.Dictionary.Variables.Add("sumwagefactors", SetFarsiNumberStatic(sumWageFactors));
        }
        catch
        {
            // Variable may already exist in template.
        }

        try
        {
            report.Dictionary.Variables.Add("InterdictId", order.Id);
        }
        catch
        {
            var existing = report.Dictionary.Variables["InterdictId"];
            if (existing != null)
                existing.ValueObject = order.Id;
        }

        ApplyOrganisationLogo(report, organImage);
        ApplyEmployeePhoto(report, employeeImage);
        ApplyConnectionString(report, connectionString);

        // Letter formatting after data bind setup, before Compile (does not change CalculationMode).
        StiReportUtilities.ApplyOrderLetterTextFormatting(report);

        try
        {
            StiReportUtilities.CompileSafe(report);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"خطا در کامپایل قالب چاپ حکم (IsCompiled={report.IsCompiled}, NeedsCompiling={report.NeedsCompiling}, Mode={report.CalculationMode}): {ex.Message}\n{ex}",
                ex);
        }

        try
        {
            if (report.IsCompiled)
            {
                try { report["InterdictId"] = order.Id; } catch { }
            }

            report.Render(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"خطا در رندر گزارش حکم (IsCompiled={report.IsCompiled}, Pages={report.Pages?.Count}, Mode={report.CalculationMode}): {ex.Message}\n{ex}",
                ex);
        }

        return report;
    }

    private static void ApplyOrganisationLogo(StiReport report, OrganisationChartImage? organImage)
    {
        if (organImage?.Content is not { Length: > 0 })
            return;

        try
        {
            // Keep stream alive with the Image (original OrderPrint behavior).
            var stream = new MemoryStream(organImage.Content);
            if (report.GetComponentByName("logoIMG") is StiImage image)
                image.Image = Image.FromStream(stream);
        }
        catch
        {
            // optional
        }
    }

    private static void ApplyEmployeePhoto(
        StiReport report,
        HR.Employee.Core.Entities.Image? employeeImage)
    {
        if (employeeImage?.ImageData is not { Length: > 0 })
            return;

        try
        {
            var stream = new MemoryStream(employeeImage.ImageData);
            if (report.GetComponentByName("employeeImage") is StiImage image)
                image.Image = Image.FromStream(stream);
        }
        catch
        {
            // optional
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

    private static string SetFarsiNumberStatic(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var chars = input.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] is >= '0' and <= '9')
                chars[i] = (char)(chars[i] + 1776 - '0');
        }

        return new string(chars);
    }
}
