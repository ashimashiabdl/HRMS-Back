using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Import;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HR.BaseInfo.infrastructure.Services;

public class ImportTemplateService : IScopedServices
{
    static ImportTemplateService()
    {
        ExcelPackage.License.SetNonCommercialOrganization("HRMS");
    }

    public byte[] GenerateTemplate(ImportProfile profile, ImportContextMode contextMode = ImportContextMode.BatchContext)
    {
        var fields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, contextMode);

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Import");
        sheet.View.RightToLeft = true;

        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var col = i + 1;
            var header = field.ExcelColumnHeader ?? field.TargetPropertyName;
            if (field.IsRequired)
                header += " *";

            sheet.Cells[1, col].Value = header;
            sheet.Cells[2, col].Value = GetSampleValue(field);

            sheet.Column(col).Width = Math.Max(16, header.Length + 4);
        }

        using (var headerRange = sheet.Cells[1, 1, 1, Math.Max(1, fields.Count)])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(232, 240, 254));
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        using (var sampleRange = sheet.Cells[2, 1, 2, Math.Max(1, fields.Count)])
        {
            sampleRange.Style.Font.Italic = true;
            sampleRange.Style.Font.Color.SetColor(System.Drawing.Color.Gray);
        }

        sheet.Cells[3, 1].Value = "ردیف‌های نمونه (۲) را حذف کرده و داده‌های خود را از همین سطر وارد کنید.";
        sheet.Cells[3, 1, 3, Math.Max(1, fields.Count)].Merge = true;

        return package.GetAsByteArray();
    }

    private static string GetSampleValue(ImportProfileField field)
    {
        var dataType = (field.DataType ?? "Text").Trim();
        var header = field.ExcelColumnHeader ?? field.TargetPropertyName ?? "";
        if (string.Equals(field.TargetPropertyName, "title", StringComparison.OrdinalIgnoreCase))
            return "نمونه عنوان ۱";

        if (string.Equals(field.TargetPropertyName, "Description", StringComparison.OrdinalIgnoreCase))
            return "توضیح نمونه";

        if (string.Equals(field.TargetPropertyName, "StatusCode", StringComparison.OrdinalIgnoreCase))
            return "1";

        if (string.Equals(dataType, "Int", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataType, "Integer", StringComparison.OrdinalIgnoreCase))
            return "1";

        if (string.Equals(dataType, "Bool", StringComparison.OrdinalIgnoreCase))
            return "بله";

        if (header.Contains("(Id)", StringComparison.OrdinalIgnoreCase))
            return "1";

        return field.ExcelColumnHeader ?? field.TargetPropertyName;
    }
}
