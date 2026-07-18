
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using HR.BaseInfo.Core.Entities;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using Hr.SystemSetting.Infrastructure.Data;
using HR.BaseInfo.infrastructure.Data;


using Microsoft.Extensions.Configuration;

namespace HR.Report.Infrastructure.Services;

public class DynamicReportService(
    IMapper mapper,
    IUnitOfWork<ReportContext> unitOfWork,
    IUnitOfWork<BaseInfoContext> baseInfoUnitOfWork,
    IUnitOfWork<SystemSettingContext> systemSettingUnitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService) : BaseService<DynamicReport, ReportContext, DynamicReportDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private static readonly DynamicReportPrint ReportPrinter = new();

    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All().Where(i => i.IsActive == true).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.title
        }));
    }

    private const long ReportTypeFunction = 11211;
    private const int ExportTypeExcel = 21521;
    private const int ExportTypePdf = 21522;

    private const uint ExcelHeaderStyleIndex = 1U;
    private const uint ExcelDataStyleIndex = 2U;

    private static byte[] ExportDataSet(
        DataSet dataSet,
        IReadOnlyDictionary<string, string> columnTitleMap)
    {
        using var stream = new MemoryStream();
        using (var workbook = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = workbook.AddWorkbookPart();
            workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
            workbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = CreateRtlStylesheet();
            stylesPart.Stylesheet.Save();

            var sheetId = 0U;
            foreach (DataTable table in dataSet.Tables)
            {
                sheetId++;
                AppendWorksheet(workbookPart, table, sheetId, columnTitleMap);
            }

            workbookPart.Workbook.Save();
        }

        return stream.ToArray();
    }

    private static void AppendWorksheet(
        WorkbookPart workbookPart,
        DataTable table,
        uint sheetId,
        IReadOnlyDictionary<string, string> columnTitleMap)
    {
        var sheetPart = workbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
        var columnValues = columnNames.ToDictionary(name => name, _ => new List<string>(), StringComparer.Ordinal);

        var headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row { RowIndex = 1U };
        for (var columnIndex = 0; columnIndex < columnNames.Count; columnIndex++)
        {
            var columnName = columnNames[columnIndex];
            var headerTitle = ResolveHeaderTitle(columnName, columnTitleMap);
            columnValues[columnName].Add(headerTitle);
            headerRow.AppendChild(CreateTextCell(columnIndex, 1, headerTitle, ExcelHeaderStyleIndex));
        }

        sheetData.AppendChild(headerRow);

        var rowIndex = 2U;
        foreach (DataRow dataRow in table.Rows)
        {
            var excelRow = new DocumentFormat.OpenXml.Spreadsheet.Row { RowIndex = rowIndex };
            for (var columnIndex = 0; columnIndex < columnNames.Count; columnIndex++)
            {
                var columnName = columnNames[columnIndex];
                var cellValue = dataRow[columnName]?.ToString() ?? string.Empty;
                columnValues[columnName].Add(cellValue);
                excelRow.AppendChild(CreateTextCell(columnIndex, (int)rowIndex, cellValue, ExcelDataStyleIndex));
            }

            sheetData.AppendChild(excelRow);
            rowIndex++;
        }

        var worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet
        {
            SheetViews = new DocumentFormat.OpenXml.Spreadsheet.SheetViews(
                new DocumentFormat.OpenXml.Spreadsheet.SheetView
                {
                    WorkbookViewId = 0U,
                    RightToLeft = true,
                }),
        };

        worksheet.Append(BuildAutoSizedColumns(columnValues, columnNames));
        worksheet.Append(sheetData);
        sheetPart.Worksheet = worksheet;

        var sheets = workbookPart.Workbook!.Sheets!;
        var relationshipId = workbookPart.GetIdOfPart(sheetPart);
        sheets.Append(new DocumentFormat.OpenXml.Spreadsheet.Sheet
        {
            Id = relationshipId,
            SheetId = sheetId,
            Name = SanitizeSheetName(table.TableName, sheetId),
        });
    }

    private static DocumentFormat.OpenXml.Spreadsheet.Columns BuildAutoSizedColumns(
        IReadOnlyDictionary<string, List<string>> columnValues,
        IReadOnlyList<string> columnNames)
    {
        var columns = new DocumentFormat.OpenXml.Spreadsheet.Columns();

        for (var columnIndex = 0; columnIndex < columnNames.Count; columnIndex++)
        {
            var width = CalculateColumnWidth(columnValues[columnNames[columnIndex]]);
            var columnNumber = (uint)(columnIndex + 1);
            columns.Append(new DocumentFormat.OpenXml.Spreadsheet.Column
            {
                Min = columnNumber,
                Max = columnNumber,
                Width = width,
                CustomWidth = true,
            });
        }

        return columns;
    }

    private static DocumentFormat.OpenXml.Spreadsheet.Cell CreateTextCell(
        int columnIndex,
        int rowIndex,
        string value,
        uint styleIndex) =>
        new()
        {
            CellReference = GetCellReference(columnIndex, rowIndex),
            DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.InlineString,
            StyleIndex = styleIndex,
            InlineString = new DocumentFormat.OpenXml.Spreadsheet.InlineString(
                new DocumentFormat.OpenXml.Spreadsheet.Text(value)
                {
                    Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve,
                }),
        };

    private static DocumentFormat.OpenXml.Spreadsheet.Stylesheet CreateRtlStylesheet()
    {
        var fonts = new DocumentFormat.OpenXml.Spreadsheet.Fonts(
            new DocumentFormat.OpenXml.Spreadsheet.Font(),
            new DocumentFormat.OpenXml.Spreadsheet.Font(new DocumentFormat.OpenXml.Spreadsheet.Bold()));
        fonts.Count = (uint)fonts.ChildElements.Count;

        var fills = new DocumentFormat.OpenXml.Spreadsheet.Fills(
            new DocumentFormat.OpenXml.Spreadsheet.Fill(
                new DocumentFormat.OpenXml.Spreadsheet.PatternFill
                {
                    PatternType = DocumentFormat.OpenXml.Spreadsheet.PatternValues.None,
                }));
        fills.Count = 1U;

        var borders = new DocumentFormat.OpenXml.Spreadsheet.Borders(
            new DocumentFormat.OpenXml.Spreadsheet.Border());
        borders.Count = 1U;

        var cellStyleFormats = new DocumentFormat.OpenXml.Spreadsheet.CellStyleFormats(
            new DocumentFormat.OpenXml.Spreadsheet.CellFormat());
        cellStyleFormats.Count = 1U;

        var rtlAlignment = CreateRtlAlignment();
        var cellFormats = new DocumentFormat.OpenXml.Spreadsheet.CellFormats(
            new DocumentFormat.OpenXml.Spreadsheet.CellFormat(),
            new DocumentFormat.OpenXml.Spreadsheet.CellFormat
            {
                FontId = 1U,
                ApplyFont = true,
                Alignment = (DocumentFormat.OpenXml.Spreadsheet.Alignment)rtlAlignment.CloneNode(true),
                ApplyAlignment = true,
            },
            new DocumentFormat.OpenXml.Spreadsheet.CellFormat
            {
                Alignment = rtlAlignment,
                ApplyAlignment = true,
            });
        cellFormats.Count = (uint)cellFormats.ChildElements.Count;

        return new DocumentFormat.OpenXml.Spreadsheet.Stylesheet(fonts, fills, borders, cellStyleFormats, cellFormats);
    }

    private static DocumentFormat.OpenXml.Spreadsheet.Alignment CreateRtlAlignment() =>
        new()
        {
            Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right,
            Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center,
            ReadingOrder = 2U,
            WrapText = true,
        };

    private static double CalculateColumnWidth(IEnumerable<string> values)
    {
        var maxDisplayWidth = values.Max(GetDisplayWidth);
        return Math.Clamp(maxDisplayWidth * 1.1 + 2, 12, 80);
    }

    private static int GetDisplayWidth(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        var width = 0;
        foreach (var character in value)
        {
            width += character <= 127 ? 1 : 2;
        }

        return width;
    }

    private static string GetCellReference(int columnIndex, int rowIndex) =>
        $"{GetColumnName(columnIndex + 1)}{rowIndex}";

    private static string GetColumnName(int columnNumber)
    {
        var dividend = columnNumber;
        var columnName = string.Empty;

        while (dividend > 0)
        {
            var modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            dividend = (dividend - modulo) / 26;
        }

        return columnName;
    }

    private static string SanitizeSheetName(string? tableName, uint sheetId)
    {
        var invalidChars = new[] { '\\', '/', '*', '?', ':', '[', ']' };
        var name = string.IsNullOrWhiteSpace(tableName) ? $"Sheet{sheetId}" : tableName;

        foreach (var invalidChar in invalidChars)
        {
            name = name.Replace(invalidChar, '_');
        }

        return name.Length > 31 ? name[..31] : name;
    }

    private async Task<IReadOnlyDictionary<string, string>> LoadReportColumnTitleMapAsync(DataSet dataSet)
    {
        var columnNames = dataSet.Tables.Cast<DataTable>()
            .SelectMany(table => table.Columns.Cast<DataColumn>().Select(column => column.ColumnName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (columnNames.Count == 0)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var mappings = await baseInfoUnitOfWork.Context.ReportMapColumns
            .AsNoTracking()
            .Where(map => columnNames.Contains(map.title) && !map.IsDeleted)
            .Select(map => new { map.title, map.PersianName })
            .ToListAsync();

        return mappings
            .Where(map => !string.IsNullOrWhiteSpace(map.PersianName))
            .ToDictionary(map => map.title, map => map.PersianName!, StringComparer.OrdinalIgnoreCase);
    }

    private static string ResolveHeaderTitle(
        string columnName,
        IReadOnlyDictionary<string, string> columnTitleMap)
    {
        if (columnTitleMap.TryGetValue(columnName, out var persianTitle)
            && !string.IsNullOrWhiteSpace(persianTitle))
        {
            return persianTitle;
        }

        return columnName;
    }

    public async Task<ApiResultDTO> CreateDynamicReport(RequestReportDTOStandard dynamicReport, string? fontsDirectory = null)
    {
        var selectedReport = await _unitOfWork.Context.DynamicReports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == dynamicReport.Id);

        if (selectedReport == null)
        {
            return ReportResult.Failed("گزارش پویا درخواستی یافت نشد.");
        }

        if (!selectedReport.IsActive)
        {
            return ReportResult.Failed("گزارش مورد نظر غیرفعال است.");
        }

        if (selectedReport.FuctionTypeId != ReportTypeFunction)
        {
            return ReportResult.Failed("نوع تابع گزارش پشتیبانی نمی‌شود.");
        }

        if (string.IsNullOrWhiteSpace(selectedReport.Schema) || string.IsNullOrWhiteSpace(selectedReport.FunctionName))
        {
            return ReportResult.Failed("نام Schema یا Function گزارش تنظیم نشده است.");
        }

        return dynamicReport.ExportTypeId switch
        {
            ExportTypeExcel => await CreateExcelReportAsync(dynamicReport, selectedReport),
            ExportTypePdf => await CreatePdfReportAsync(dynamicReport, selectedReport, fontsDirectory),
            _ => ReportResult.Failed("نوع خروجی گزارش معتبر نیست."),
        };
    }

    private async Task<ApiResultDTO> CreateExcelReportAsync(
        RequestReportDTOStandard dynamicReport,
        DynamicReport selectedReport)
    {
        var connectionString = PrepareSqlConnectionString(_connectionString);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return ReportResult.Failed("رشته اتصال پایگاه داده برای تولید گزارش مشخص نشده است.");
        }

        var dataSetResult = await LoadReportDataSetAsync(dynamicReport, selectedReport, connectionString);
        if (dataSetResult.Error != null)
        {
            return ReportResult.Failed(dataSetResult.Error);
        }

        if (dataSetResult.Data == null || dataSetResult.Data.Tables.Count == 0)
        {
            return ReportResult.Failed("داده‌ای برای گزارش اکسل برگردانده نشد.");
        }

        var columnTitleMap = await LoadReportColumnTitleMapAsync(dataSetResult.Data);
        var excelBytes = ExportDataSet(dataSetResult.Data, columnTitleMap);
        if (excelBytes.Length == 0)
        {
            return ReportResult.Failed("فایل اکسل تولید‌شده خالی است.");
        }

        return await SaveReportFileAsync(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xlsx",
            selectedReport.title,
            "گزارش اکسل با موفقیت تهیه شد.");
    }

    private async Task<ApiResultDTO> CreatePdfReportAsync(
        RequestReportDTOStandard dynamicReport,
        DynamicReport selectedReport,
        string? fontsDirectory)
    {
        if (!selectedReport.OrganisationMRTId.HasValue || selectedReport.OrganisationMRTId.Value <= 0)
        {
            return ReportResult.Failed("قالب گزارش (MRT) برای این گزارش تنظیم نشده است.");
        }

        var organisationMrt = await systemSettingUnitOfWork.Context.OrganisationMRTs
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == selectedReport.OrganisationMRTId.Value);

        if (organisationMrt?.Content == null || organisationMrt.Content.Length == 0)
        {
            return ReportResult.Failed("قالب گزارش (MRT) یافت نشد یا محتوای آن خالی است.");
        }

        var connectionString = PrepareSqlConnectionString(_connectionString);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return ReportResult.Failed("رشته اتصال پایگاه داده برای تولید گزارش مشخص نشده است.");
        }

        var pdfResult = GenerateReportPdf(
            organisationMrt,
            connectionString,
            fontsDirectory,
            dynamicReport.CurrentUserId,
            dynamicReport.UserName,
            dynamicReport.Parameters);

        if (pdfResult.Error != null)
        {
            return ReportResult.Failed(pdfResult.Error);
        }

        if (pdfResult.Pdf == null || pdfResult.Pdf.Length == 0)
        {
            return ReportResult.Failed("فایل PDF تولید‌شده خالی است.");
        }

        return await SaveReportFileAsync(
            pdfResult.Pdf,
            "application/pdf",
            ".pdf",
            selectedReport.title,
            "گزارش PDF با موفقیت تهیه شد.");
    }

    private async Task<(DataSet? Data, string? Error)> LoadReportDataSetAsync(
        RequestReportDTOStandard dynamicReport,
        DynamicReport selectedReport,
        string connectionString)
    {
        var dataSet = new DataSet();

        await using var connection = new SqlConnection(connectionString);
        await using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = BuildStoredProcedureName(selectedReport.Schema!, selectedReport.FunctionName!);

        await connection.OpenAsync();

        try
        {
            SqlCommandBuilder.DeriveParameters(command);
        }
        catch (SqlException ex)
        {
            return (null, MapReportSqlError(ex));
        }

        var bindError = BindStoredProcedureParameters(command, dynamicReport);
        if (bindError != null)
        {
            return (null, bindError);
        }

        try
        {
            using var adapter = new SqlDataAdapter((SqlCommand)command);
            adapter.Fill(dataSet);
            return (dataSet, null);
        }
        catch (SqlException ex)
        {
            return (null, MapReportSqlError(ex));
        }
    }

    private static string? BindStoredProcedureParameters(SqlCommand command, RequestReportDTOStandard dynamicReport)
    {
        var requestParameters = BuildRequestParameterLookup(dynamicReport);

        foreach (SqlParameter sqlParameter in command.Parameters)
        {
            if (sqlParameter.Direction == ParameterDirection.ReturnValue)
            {
                continue;
            }

            var parameterName = NormalizeParameterName(sqlParameter.ParameterName);

            if (string.Equals(parameterName, "CurrentUserId", StringComparison.OrdinalIgnoreCase))
            {
                sqlParameter.Value = dynamicReport.CurrentUserId;
                continue;
            }

            if (requestParameters.TryGetValue(parameterName, out var requestValue))
            {
                sqlParameter.Value = ConvertParameterValue(sqlParameter, requestValue);
                continue;
            }

            if (sqlParameter.Value != null && sqlParameter.Value != DBNull.Value)
            {
                continue;
            }

            if (IsNullableParameter(sqlParameter))
            {
                sqlParameter.Value = DBNull.Value;
                continue;
            }

            return $"پارامتر اجباری «{parameterName}» برای تابع گزارش مقداردهی نشده است.";
        }

        return null;
    }

    private static Dictionary<string, string> BuildRequestParameterLookup(RequestReportDTOStandard dynamicReport)
    {
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var parameter in dynamicReport.Parameters ?? [])
        {
            if (string.IsNullOrWhiteSpace(parameter.title))
            {
                continue;
            }

            var csv = parameter.SelectedValues != null
                ? string.Join(",", parameter.SelectedValues.Where(v => !string.IsNullOrWhiteSpace(v)))
                : string.Empty;

            lookup[NormalizeParameterName(parameter.title)] = csv;
        }

        return lookup;
    }

    private static string NormalizeParameterName(string parameterName) =>
        parameterName.Trim().TrimStart('@');

    private static bool IsNullableParameter(SqlParameter parameter) => parameter.IsNullable;

    private static object ConvertParameterValue(SqlParameter sqlParameter, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DBNull.Value;
        }

        var scalarValue = value.Contains(',', StringComparison.Ordinal)
            ? value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? value
            : value;

        return sqlParameter.SqlDbType switch
        {
            SqlDbType.BigInt when long.TryParse(scalarValue, out var longValue) => longValue,
            SqlDbType.Int when int.TryParse(scalarValue, out var intValue) => intValue,
            SqlDbType.SmallInt when short.TryParse(scalarValue, out var shortValue) => shortValue,
            SqlDbType.TinyInt when byte.TryParse(scalarValue, out var byteValue) => byteValue,
            SqlDbType.Bit when bool.TryParse(scalarValue, out var boolValue) => boolValue,
            SqlDbType.Decimal when decimal.TryParse(scalarValue, out var decimalValue) => decimalValue,
            SqlDbType.Float when double.TryParse(scalarValue, out var doubleValue) => doubleValue,
            SqlDbType.Real when float.TryParse(scalarValue, out var floatValue) => floatValue,
            SqlDbType.Date or SqlDbType.DateTime or SqlDbType.DateTime2
                when DateTime.TryParse(scalarValue, out var dateValue) => dateValue,
            SqlDbType.UniqueIdentifier when Guid.TryParse(scalarValue, out var guidValue) => guidValue,
            _ => scalarValue,
        };
    }

    private static (byte[]? Pdf, string? Error) GenerateReportPdf(
        Hr.SystemSetting.Core.Entities.OrganisationMRT organisationMrt,
        string connectionString,
        string? fontsDirectory,
        long currentUserId,
        string? userName,
        IEnumerable<DynamicReportParameterStandard>? parameters)
    {
        try
        {
            var pdfBytes = ReportPrinter.GeneratePdf(
                organisationMrt,
                connectionString,
                fontsDirectory,
                currentUserId,
                userName,
                parameters);

            return (pdfBytes, null);
        }
        catch (InvalidOperationException ex)
        {
            return (null, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return (null, ex.Message);
        }
        catch (DirectoryNotFoundException ex)
        {
            return (null, ex.Message);
        }
    }

    private static string BuildStoredProcedureName(string schema, string functionName) =>
        $"[{schema.Trim()}].[{functionName.Trim()}]";

    private async Task<ApiResultDTO> SaveReportFileAsync(
        byte[] content,
        string mimeType,
        string extension,
        string? reportTitle,
        string successMessage)
    {
        var tempFile = new TempGlobalFile
        {
            Content = content,
            CreateDate = DateTime.Now,
            IPAddress = "Report",
            MimeType = mimeType,
            Extension = extension,
            Size = content.LongLength,
            UniqueId = Guid.NewGuid(),
            title = "Report_" + (reportTitle ?? "DynamicReport"),
        };

        baseInfoUnitOfWork.Context.TempGlobalFiles.Add(tempFile);
        await baseInfoUnitOfWork.Context.SaveChangesAsync();

        return ReportResult.Succeeded(tempFile.Id, successMessage);
    }

    private static string PrepareSqlConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return string.Empty;
        }

        return new SqlConnectionStringBuilder(connectionString)
        {
            MultipleActiveResultSets = true,
        }.ConnectionString;
    }

    private static string MapReportSqlError(SqlException ex) =>
        ex.Number switch
        {
            2812 => "تابع گزارش در پایگاه داده یافت نشد. نام Schema و Function را بررسی کنید.",
            208 => "نام Schema یا Function گزارش نامعتبر است.",
            201 or 8144 => ex.Message.Contains("too many arguments", StringComparison.OrdinalIgnoreCase)
                ? "پارامترهای ارسالی بیش از حد مجاز است. نام پارامترهای گزارش را با تعریف تابع در پایگاه داده تطبیق دهید."
                : "پارامترهای ارسالی با تعریف تابع گزارش همخوانی ندارد.",
            _ when ex.Message.Contains("Keyword not supported", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("multiple active result sets", StringComparison.OrdinalIgnoreCase)
                => "خطا در اتصال به پایگاه داده هنگام تهیه گزارش. لطفاً با پشتیبانی تماس بگیرید.",
            _ => "خطا در اجرای گزارش در پایگاه داده. پارامترهای گزارش را بررسی کنید یا با پشتیبانی تماس بگیرید.",
        };

    private static class ReportResult
    {
        public static ApiResultDTO Failed(string message) => new()
        {
            Success = false,
            Message = message,
        };

        public static ApiResultDTO Succeeded(long fileId, string message) => new()
        {
            Success = true,
            Id = fileId,
            Message = message,
        };
    }
    public new async Task<OperationResult> UpdateForAsync(DynamicReportDTO entityToUpdate)
    {
        try
        {
            var mappedTodo = _mapper.Map<DynamicReport>(entityToUpdate);
            _unitOfWork.CreateTransaction();

            try
            {
                Update(mappedTodo);
                var existingParameters = _unitOfWork.Context.DynamicReportParameter.Where(i => i.DynamicReportId == mappedTodo.Id).ToList();
                _unitOfWork.Context.RemoveRange(existingParameters);
                _unitOfWork.Context.SaveChanges();
                if (entityToUpdate.DynamicReportParameterList != null)
                {
                    if (entityToUpdate.DynamicReportParameterList.Any())
                    {
                        foreach (var item in entityToUpdate.DynamicReportParameterList)
                        {
                            _unitOfWork.Context.DynamicReportParameter.Add(
                                new DynamicReportParameter()
                                {
                                    DynamicReportId = mappedTodo.Id,
                                    CreateDate = DateTime.Now,
                                    IPAddress = "",
                                    ParameterId = item.ParameterId,
                                    Optional = item.Optional,
                                    DefaultValue = item.DefaultValue,
                                    title = item.title
                                }
                                );
                        }
                        _unitOfWork.Context.SaveChanges();
                    }
                }
                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: 1);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed();
            }
        }
        catch (Exception ex)
        {
            return OperationResult.Failed();
        }

    }
    public new async Task<OperationResult> CreateForAsync(DynamicReportDTO entityToCreate)
    {
        try
        {
            var mappedTodo = _mapper.Map<DynamicReport>(entityToCreate);
            if (typeof(DynamicReport).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                if (_currentUserDefaultOrganId > 0)
                {
                    PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                    propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                }
                else
                {
                    throw new Exception("سازمان پیش فرض مشخض نشده است");
                }
            }
            _unitOfWork.CreateTransaction();

            try
            {
                Add(mappedTodo);
                _unitOfWork.Context.SaveChanges();
                if (entityToCreate.DynamicReportParameterList != null)
                {
                    if (entityToCreate.DynamicReportParameterList.Any())
                    {
                        foreach (var item in entityToCreate.DynamicReportParameterList)
                        {
                            _unitOfWork.Context.DynamicReportParameter.Add(
                                new DynamicReportParameter()
                                {
                                    DynamicReportId = mappedTodo.Id,
                                    CreateDate = DateTime.Now,
                                    IPAddress = "",
                                    ParameterId = item.ParameterId,
                                    Optional = item.Optional,
                                    DefaultValue = item.DefaultValue,
                                    title = item.title
                                }
                                );
                        }
                        _unitOfWork.Context.SaveChanges();
                    }
                }

                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: mappedTodo.Id);

            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
            }


            return OperationResult.Failed();

        }
        catch (Exception ex)
        {
            return OperationResult.Failed();
        }

    }
   
}
