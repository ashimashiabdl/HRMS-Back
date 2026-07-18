using AutoMapper;
using Hr.Employee.infrastructure.Services;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Excel;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HR.Payroll.Infrastructure.Services;

public sealed class ExcelFunctionImportSummary
{
    public long DataStartRow { get; set; }
    public int CandidateRows { get; set; }
    public int HeaderRowsSkipped { get; set; }
    public int RowsProcessed { get; set; }
    public int RowsImported { get; set; }
    public int RowsSkippedNoEmployee { get; set; }
    public int LeaveRowsImported { get; set; }
}

public class FunctionExcelDefinitionService(IMapper mapper, EmployeeService EmployeeService, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<FunctionExcelDefinitionService> logger) : BaseService<FunctionExcelDefinition, PayrollContext, FunctionExcelDefinitionDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private EmployeeService _employeeService = EmployeeService;
    private readonly ILogger<FunctionExcelDefinitionService> _logger = logger;

    public IQueryable<FunctionExcelDefinition> All(bool IgnoreExpired = false)
    {
        return base.All(IgnoreExpired)
            .Include(i => i.ExcelDefinitionType)
            .Include(i => i.PersonnelFunctionColumn)
            .Include(i => i.MappedExcelColumn)
            .Include(i => i.EmployeeType)
            .Include(i => i.LeaveType)
            ;
    }

    public OperationResult PhysicalDelete(long id)
    {
        var entity = _unitOfWork.Context.FunctionExcelDefinitions.Find(id);
        if (entity == null)
        {
            return OperationResult.Failed("رکورد یافت نشد.");
        }

        try
        {
            _unitOfWork.Context.FunctionExcelDefinitions.Remove(entity);
            if (_unitOfWork.Save().GetAwaiter().GetResult() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }

            return OperationResult.Failed();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.GetBaseException().Message);
        }
    }
    /// <summary>
    /// آپلود موقت فایل اکسل کارکرد برای پیش‌نمایش و ثبت.
    /// </summary>
    public async Task<OperationResult> UploadTempExcelFileAsync(
        Stream fileStream,
        string originalFileName,
        long fileSizeBytes,
        long paymentPeriodId,
        long employeeTypeId,
        long aspNetUserId,
        string ipAddress)
    {
        var safeFileName = originalFileName ?? string.Empty;
        _logger.LogInformation(
            "Excel function upload started. FileName={FileName}, SizeBytes={SizeBytes}, PaymentPeriodId={PaymentPeriodId}, EmployeeTypeId={EmployeeTypeId}, OrganId={OrganId}, UserId={UserId}, IP={IP}",
            safeFileName, fileSizeBytes, paymentPeriodId, employeeTypeId, _currentUserDefaultOrganId, aspNetUserId, ipAddress);

        if (fileStream == null || !fileStream.CanRead)
        {
            _logger.LogWarning("Excel function upload rejected: unreadable stream. FileName={FileName}, UserId={UserId}", safeFileName, aspNetUserId);
            return OperationResult.Failed("محتوای فایل قابل خواندن نیست");
        }

        if (fileSizeBytes <= 0)
        {
            _logger.LogWarning("Excel function upload rejected: empty file. FileName={FileName}, UserId={UserId}", safeFileName, aspNetUserId);
            return OperationResult.Failed("فایل خالی است");
        }

        var extension = Path.GetExtension(safeFileName);
        if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Excel function upload rejected: invalid extension. FileName={FileName}, Extension={Extension}, UserId={UserId}",
                safeFileName, extension, aspNetUserId);
            return OperationResult.Failed("فقط فایل اکسل با پسوند .xlsx مجاز است");
        }

        if (paymentPeriodId <= 0)
        {
            return OperationResult.Failed("دوره پرداخت انتخاب نشده است");
        }

        if (employeeTypeId <= 0)
        {
            return OperationResult.Failed("نوع استخدام انتخاب نشده است");
        }

        var maxSizeSetting = GetSettingById(10008);
        var maxSizeKb = string.IsNullOrEmpty(maxSizeSetting)
            ? HR.SharedKernel.Share.Constants.defaultMaxUploadSize
            : Convert.ToInt64(maxSizeSetting);
        var fileSizeKb = fileSizeBytes / 1024;
        if (fileSizeKb > maxSizeKb)
        {
            _logger.LogWarning(
                "Excel function upload rejected: size limit exceeded. FileName={FileName}, SizeKb={SizeKb}, LimitKb={LimitKb}, UserId={UserId}",
                safeFileName, fileSizeKb, maxSizeKb, aspNetUserId);
            return OperationResult.Failed($"حجم فایل ارسالی بیش از مقدار مجاز تعیین شده می باشد ({maxSizeKb} KB)");
        }

        try
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await fileStream.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            if (fileBytes.Length == 0)
            {
                _logger.LogWarning("Excel function upload rejected: zero bytes after read. FileName={FileName}, UserId={UserId}", safeFileName, aspNetUserId);
                return OperationResult.Failed("فایل خالی است");
            }

            if (!HasXlsxSignature(fileBytes))
            {
                _logger.LogWarning(
                    "Excel function upload rejected: content signature mismatch. FileName={FileName}, UserId={UserId}",
                    safeFileName, aspNetUserId);
                return OperationResult.Failed("محتوای فایل با فرمت اکسل (.xlsx) سازگار نیست");
            }

            var sanitizedName = HR.SharedKernel.Share.Helper.SanitizeFileName(safeFileName);
            var toAdd = new PersonnelFunctionExcelFile
            {
                CreateDate = DateTime.Now,
                IPAddress = ipAddress,
                AspNetUsersId = aspNetUserId,
                title = sanitizedName,
                IsDeleted = false,
                UniqueId = Guid.NewGuid(),
                Extension = extension,
                OrganisationChartId = _currentUserDefaultOrganId,
                Size = fileBytes.Length,
                MimeType = HR.SharedKernel.Share.Helper.GetMimeType(extension),
                PaymentPeriodId = paymentPeriodId,
                EmployeeTypeId = employeeTypeId,
                Content = fileBytes
            };

            _unitOfWork.Context.PersonnelFunctionExcelFiles.Add(toAdd);
            await _unitOfWork.Context.SaveChangesAsync();

            _logger.LogInformation(
                "Excel function upload succeeded. FileId={FileId}, FileName={FileName}, SizeBytes={SizeBytes}, PaymentPeriodId={PaymentPeriodId}, EmployeeTypeId={EmployeeTypeId}, UserId={UserId}",
                toAdd.Id, sanitizedName, fileBytes.Length, paymentPeriodId, employeeTypeId, aspNetUserId);

            return OperationResult.Succeeded(payload: toAdd.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Excel function upload failed. FileName={FileName}, PaymentPeriodId={PaymentPeriodId}, EmployeeTypeId={EmployeeTypeId}, UserId={UserId}",
                safeFileName, paymentPeriodId, employeeTypeId, aspNetUserId);
            return OperationResult.Failed("خطا در ذخیره فایل اکسل. لطفاً دوباره تلاش کنید.");
        }
    }

    private static bool HasXlsxSignature(byte[] content)
    {
        // XLSX is an OOXML zip archive: PK\x03\x04
        return content.Length >= 4
            && content[0] == 0x50
            && content[1] == 0x4B
            && content[2] == 0x03
            && content[3] == 0x04;
    }

    /// <summary>
    /// این سرویس فایل آپلود شده اکسل توسط کاربر را بر اساس تنظیمات به اطلاعات جدول کارکرد تبدیل می کند
    /// </summary>
    /// <param name="FileId"></param>
    /// <returns></returns>
    public OperationResult GetExcelPreview(long FileId)
    {
        _logger.LogInformation("Excel preview started. FileId={FileId}, OrganId={OrganId}", FileId, _currentUserDefaultOrganId);
        try
        {
            var File = _unitOfWork.Context.PersonnelFunctionExcelFiles.Find(FileId);
            if (File == null || File.Content == null || File.Content.Length == 0)
            {
                _logger.LogWarning("Excel preview failed: file not found or empty. FileId={FileId}", FileId);
                return OperationResult.Failed("فایل یافت نشد یا خالی است");
            }

            int errorCount;
            List<string> errors;
            var successCount = ExcelDataToDataTableFunction(new MemoryStream(File.Content), FileId, File.EmployeeTypeId, File.PaymentPeriodId, out errorCount, out errors, out var importSummary);

            string message = successCount > 0
                ? $"{successCount} ردیف با موفقیت خوانده شد" + (errorCount > 0 ? $"، {errorCount} خطا در خواندن ثبت شد" : string.Empty)
                : BuildZeroImportUserMessage(importSummary, errorCount);

            if (successCount == 0)
            {
                _logger.LogWarning(
                    "Excel preview imported zero rows. FileId={FileId}, EmployeeTypeId={EmployeeTypeId}, PaymentPeriodId={PaymentPeriodId}, DataStartRow={DataStartRow}, CandidateRows={CandidateRows}, HeaderSkipped={HeaderSkipped}, Processed={Processed}, SkippedNoEmployee={SkippedNoEmployee}, Errors={ErrorCount}",
                    FileId, File.EmployeeTypeId, File.PaymentPeriodId,
                    importSummary.DataStartRow, importSummary.CandidateRows, importSummary.HeaderRowsSkipped,
                    importSummary.RowsProcessed, importSummary.RowsSkippedNoEmployee, errorCount);
            }

            _logger.LogInformation(
                "Excel preview completed. FileId={FileId}, SuccessRows={SuccessRows}, ErrorCount={ErrorCount}",
                FileId, successCount, errorCount);

            return OperationResult.Succeeded(msg: message, payload: new
            {
                successCount,
                errorCount,
                errors,
                diagnostics = importSummary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel preview failed with exception. FileId={FileId}", FileId);
            return OperationResult.Failed($"خطا در خواندن فایل اکسل: {ex.Message}");
        }
    }

    public int ExcelDataToDataTableFunction(Stream strm, long FileId, long EmployeeTypeId, long PaymentPeriodId, out int errorCount, out List<string> errors, out ExcelFunctionImportSummary importSummary)
    {
        errorCount = 0;
        errors = new List<string>();
        importSummary = new ExcelFunctionImportSummary();
        try
        {
            List<TempPersonnelFunction> retFunctionList = new List<TempPersonnelFunction>();
            List<TempPersonnelLeave> retLeaveList = new List<TempPersonnelLeave>();

            if (EmployeeTypeId > 0)
            {

            }
            else
            {
                errors.Add("نوع استخدام یافت نشد");
                errorCount++;
                return 0;
            }
            if (PaymentPeriodId > 0)
            {

            }
            else
            {
                errors.Add("دوره مورد نظر ثبت نشده است");
                errorCount++;
                return 0;
            }
            using (var doc = SpreadsheetDocument.Open(strm, false))
            {
                var workbookPart = doc.WorkbookPart;
                var sheet = workbookPart.Workbook.Sheets.OfType<Sheet>().FirstOrDefault();
                if (sheet == null)
                {
                    errors.Add("Sheet اصلی یافت نشد");
                    errorCount++;
                    return 0;
                }
                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                if (sheetData == null)
                {
                    errors.Add("داده ای در برگه یافت نشد");
                    errorCount++;
                    return 0;
                }

                long pureDataStartIndex = 0;
                var maxSizeSetting = GetSettingById(10018);
                if (string.IsNullOrEmpty(maxSizeSetting))
                {
                    pureDataStartIndex = HR.SharedKernel.Share.Constants.FunctionExcelStartingRow;
                }
                else
                {
                    pureDataStartIndex = Convert.ToInt64(maxSizeSetting);
                }



                var settingList = _unitOfWork.Context.FunctionExcelDefinitions
                    .Include(i => i.MappedExcelColumn)
                    .Include(i => i.PersonnelFunctionColumn)
                    .Include(i => i.LeaveType)
                    .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId
                    && i.EmployeeTypeId == EmployeeTypeId
                    ).ToList();

                if (settingList == null || settingList.Count == 0)
                {
                    errors.Add("هیچ تنظیماتی برای نگاشت ستون‌های اکسل یافت نشد. لطفاً ابتدا در بخش 'تعریف اکسل کارکرد' ستون‌ها را تعریف کنید.");
                    errorCount++;
                    return 0;
                }

                // Separate settings for function and leave
                var functionSettings = settingList.Where(s => !s.IsLeave).ToList();
                var leaveSettings = settingList.Where(s => s.IsLeave).ToList();

                var propertyListFunction = typeof(TempPersonnelFunction).GetProperties();
                var propertyListLeave = typeof(TempPersonnelLeave).GetProperties();

                var sharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;

                var mandatorySettings = settingList
                    .Where(s => s.IsMandatory && s.MappedExcelColumn != null && s.PersonnelFunctionColumn != null)
                    .ToList();

                var rowsInSheet = sheetData.Elements<Row>().Where(r => r.RowIndex >= pureDataStartIndex).ToList();
                importSummary.DataStartRow = pureDataStartIndex;
                importSummary.CandidateRows = rowsInSheet.Count;
                if (rowsInSheet.Count == 0)
                {
                    errors.Add($"هیچ ردیفی از ردیف {pureDataStartIndex} به بعد در اکسل یافت نشد. لطفاً ردیف شروع داده را بررسی کنید.");
                    errorCount++;
                    return 0;
                }

                // First row raw data debug suppressed from user-visible errors

                var selectedPeriod = _unitOfWork.Context.PaymentPeriods.Find(PaymentPeriodId);


                foreach (var rowElement in rowsInSheet)
                {
                    if (ShouldSkipRowAsHeader(rowElement, functionSettings, sharedStringTable))
                    {
                        importSummary.HeaderRowsSkipped++;
                        _logger.LogInformation(
                            "Skipping excel header row. FileId={FileId}, RowIndex={RowIndex}",
                            FileId, rowElement.RowIndex);
                        continue;
                    }

                    importSummary.RowsProcessed++;

                    bool checkIdentity = true;
                    long resolvedEmployeeId = 0;

                    // Create function row if there are function settings
                    TempPersonnelFunction? functionRow = null;
                    if (functionSettings.Count > 0)
                    {
                        functionRow = new()
                        {
                            OrganisationChartId = _currentUserDefaultOrganId,
                            CreateDate = DateTime.Now,
                            IPAddress = "",
                            PersonnelFunctionExcelFileId = FileId,
                            Year = selectedPeriod.ShamsiYear,
                            Month = selectedPeriod.ShamsiMonth,
                            FunctionDay = selectedPeriod.PeriodDays
                        };
                        ((HR.SharedKernel.Data.BaseEntity)functionRow).title = Guid.NewGuid().ToString("N");
                    }

                    List<string> rowErrors = new List<string>();

                    // Process function columns
                    if (functionRow != null)
                    {
                        foreach (var property in propertyListFunction)
                        {
                            var singleSetting = functionSettings.FirstOrDefault(s => s.PersonnelFunctionColumn != null && s.PersonnelFunctionColumn.Value != null && s.PersonnelFunctionColumn.Value.Trim().ToLower() == property.Name.Trim().ToLower());
                            if (singleSetting == null || singleSetting.MappedExcelColumn == null || singleSetting.PersonnelFunctionColumn == null)
                            {
                                continue;
                            }

                            var cell = GetCellByColumnIndex(rowElement, singleSetting.MappedExcelColumn.Order, singleSetting.MappedExcelColumn.Value);
                            var cellText = GetCellText(cell, sharedStringTable);
                            if ((cell == null || cell.CellValue == null || string.IsNullOrWhiteSpace(cellText)) && cell?.CellFormula != null)
                            {
                                // Formula without cached value – cannot evaluate on server; treat as empty but warn
                                rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار ستون دارای فرمول محاسبه نشده است (ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)})");
                            }

                            var propertyInfo = functionRow.GetType().GetProperty(singleSetting.PersonnelFunctionColumn.Value);
                            if (propertyInfo == null)
                            {
                                continue;
                            }
                            var typeName = propertyInfo.PropertyType?.FullName?.ToLower();
                            if (string.IsNullOrEmpty(typeName))
                            {
                                continue;
                            }
                            if (typeName.Contains("int32"))
                            {
                                if (TryParseInt32CellValue(cellText, singleSetting, out var intVal, out var intError))
                                {
                                    propertyInfo.SetValue(functionRow, intVal, null);
                                    if (!string.IsNullOrEmpty(intError))
                                    {
                                        rowErrors.Add($"ردیف {rowElement.RowIndex}: {intError} (ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)})");
                                    }
                                }
                                else
                                {
                                    rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار نامعتبر برای عدد صحیح در ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)} (مقدار: '{FormatCellValueForError(cellText)}')");
                                }
                                continue;
                            }
                            if (typeName.Contains("int64"))
                            {
                                if (TryParseInt64CellValue(cellText, singleSetting, out var longVal, out var longError))
                                {
                                    propertyInfo.SetValue(functionRow, longVal, null);
                                    if (!string.IsNullOrEmpty(longError))
                                    {
                                        rowErrors.Add($"ردیف {rowElement.RowIndex}: {longError} (ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)})");
                                    }
                                }
                                else
                                {
                                    rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار نامعتبر برای عدد صحیح بزرگ در ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)}");
                                }
                                continue;
                            }
                            if (typeName.Contains("decimal") || typeName.Contains("single") || typeName.Contains("double"))
                            {
                                if (TryParseDecimalCellValue(cellText, singleSetting, out var decVal, out var decError))
                                {
                                    var numericTargetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                                    propertyInfo.SetValue(functionRow, Convert.ChangeType(decVal, numericTargetType), null);
                                    if (!string.IsNullOrEmpty(decError))
                                    {
                                        rowErrors.Add($"ردیف {rowElement.RowIndex}: {decError} (ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)})");
                                    }
                                }
                                else
                                {
                                    rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار نامعتبر برای عدد اعشاری در ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)} (مقدار: '{FormatCellValueForError(cellText)}')");
                                }
                                continue;
                            }
                            if (typeName.Contains("string"))
                            {
                                if (singleSetting.IsHourMinute)
                                {
                                    var allowMinFraction = singleSetting.IsFirstOrSecondSection && singleSetting.NeedMinuteNormalization;
                                    var normResult = NormalizeTime(cellText, allowMinFraction, true);
                                    if (normResult.HasError)
                                    {
                                        rowErrors.Add($"ردیف {rowElement.RowIndex}: {normResult.ErrorMessage} (ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)})");
                                    }
                                    propertyInfo.SetValue(functionRow, normResult.Normalized, null);
                                }
                                else
                                {
                                    propertyInfo.SetValue(functionRow, string.IsNullOrEmpty(cellText) ? string.Empty : Convert.ToString(cellText), null);
                                }
                                continue;
                            }
                            if (typeName.Contains("boolean"))
                            {
                                if (string.IsNullOrWhiteSpace(cellText))
                                {
                                    propertyInfo.SetValue(functionRow, false, null);
                                }
                                else if (bool.TryParse(cellText, out var boolVal))
                                {
                                    propertyInfo.SetValue(functionRow, boolVal, null);
                                }
                                else if (cellText.Trim() == "1" || cellText.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                                {
                                    propertyInfo.SetValue(functionRow, true, null);
                                }
                                else if (cellText.Trim() == "0" || cellText.Trim().Equals("false", StringComparison.OrdinalIgnoreCase))
                                {
                                    propertyInfo.SetValue(functionRow, false, null);
                                }
                                else
                                {
                                    rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار نامعتبر برای مقدار بولین در ستون {GetExcelColumnName(singleSetting.MappedExcelColumn.Order)}");
                                }
                                continue;
                            }
                        }
                    }

                    // Mandatory checks per row
                    foreach (var mand in mandatorySettings)
                    {
                        var mandCell = GetCellByColumnIndex(rowElement, mand.MappedExcelColumn!.Order, mand.MappedExcelColumn.Value);
                        var mandText = GetCellText(mandCell, sharedStringTable);
                        if (string.IsNullOrWhiteSpace(mandText))
                        {
                            var fieldName = mand.PersonnelFunctionColumn!.Value ?? "فیلد الزامی";
                            rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار '{fieldName}' الزامی است (ستون {GetExcelColumnName(mand.MappedExcelColumn.Order)})");
                        }
                    }

                    // Resolve Employee ID (needed for both function and leave)
                    if (checkIdentity)
                    {
                        var nationalNoSetting = settingList
                            .Where(s => s.PersonnelFunctionColumn != null
                                && string.Equals(s.PersonnelFunctionColumn.Value?.Trim(), "NationalNo", StringComparison.OrdinalIgnoreCase)
                                && s.MappedExcelColumn != null)
                            .OrderByDescending(s => s.MappedExcelColumn!.Order)
                            .FirstOrDefault();
                        if (nationalNoSetting != null && nationalNoSetting.MappedExcelColumn != null)
                        {
                            var cell = GetCellByColumnIndex(rowElement, nationalNoSetting.MappedExcelColumn.Order, nationalNoSetting.MappedExcelColumn.Value);
                            var cellText = GetCellText(cell, sharedStringTable);
                            if (!string.IsNullOrEmpty(cellText))
                            {
                                var resolvedFromNationalNo = TryResolveEmployeeIdByNationalNo(cellText, rowElement, sharedStringTable, out var attemptedNationalNo);
                                if (resolvedFromNationalNo > 0)
                                {
                                    resolvedEmployeeId = resolvedFromNationalNo;
                                    checkIdentity = false; // identity resolved
                                }
                                else if (!string.IsNullOrEmpty(attemptedNationalNo))
                                {
                                    rowErrors.Add($"ردیف {rowElement.RowIndex}: کارمند با کد ملی '{attemptedNationalNo}' یافت نشد");
                                }
                            }
                        }

                        if (checkIdentity)
                        {
                            var employeeIdSetting = settingList.FirstOrDefault(s => s.PersonnelFunctionColumn != null && s.PersonnelFunctionColumn.Value != null && s.PersonnelFunctionColumn.Value.Trim().ToLower() == "employeeid");
                            if (employeeIdSetting != null && employeeIdSetting.MappedExcelColumn != null)
                            {
                                var cell = GetCellByColumnIndex(rowElement, employeeIdSetting.MappedExcelColumn.Order, employeeIdSetting.MappedExcelColumn.Value);
                                var cellText = GetCellText(cell, sharedStringTable);
                                if (!string.IsNullOrEmpty(cellText))
                                {
                                    if (long.TryParse(cellText, out var empId))
                                    {
                                        var employee = _employeeService.All().Where(i => i.Id == empId);
                                        if (employee != null && employee.Any())
                                        {
                                            resolvedEmployeeId = employee.Single().Id;
                                            checkIdentity = false; // identity resolved
                                        }
                                        else
                                        {
                                            rowErrors.Add($"ردیف {rowElement.RowIndex}: کارمند با شناسه '{cellText}' یافت نشد");
                                        }
                                    }
                                    else
                                    {
                                        rowErrors.Add($"ردیف {rowElement.RowIndex}: مقدار شناسه کارمند '{cellText}' نامعتبر است");
                                    }
                                }
                            }
                            else
                            {
                                // No EmployeeId mapping configured either
                                if (nationalNoSetting == null || nationalNoSetting.MappedExcelColumn == null)
                                {
                                    rowErrors.Add($"ردیف {rowElement.RowIndex}: هیچ نگاشتی برای 'NationalNo' یا 'EmployeeId' تعریف نشده است");
                                }
                            }
                        }
                    }

                    // Set EmployeeId for function row
                    if (functionRow != null && resolvedEmployeeId > 0)
                    {
                        functionRow.EmployeeId = resolvedEmployeeId;
                    }
                    else if (functionRow != null)
                    {
                        importSummary.RowsSkippedNoEmployee++;
                        if (!rowErrors.Any(e => e.Contains("کارمند", StringComparison.Ordinal) || e.Contains("کد ملی", StringComparison.Ordinal)))
                        {
                            var nationalNoColumn = GetNationalNoColumnLabel(settingList);
                            var nationalNoText = GetNationalNoCellText(rowElement, settingList, sharedStringTable);
                            if (string.IsNullOrWhiteSpace(nationalNoText))
                            {
                                rowErrors.Add($"ردیف {rowElement.RowIndex}: کد ملی در ستون {nationalNoColumn} خالی است؛ ردیف وارد سیستم نشد");
                            }
                            else
                            {
                                rowErrors.Add($"ردیف {rowElement.RowIndex}: کاربر/کارمند با کد ملی '{NormalizeNationalNo(nationalNoText)}' شناسایی نشد؛ ردیف وارد سیستم نشد");
                            }
                        }

                        _logger.LogWarning(
                            "Excel row skipped because employee was not resolved. FileId={FileId}, RowIndex={RowIndex}, NationalNoCell={NationalNoCell}",
                            FileId, rowElement.RowIndex, FormatCellValueForError(GetNationalNoCellText(rowElement, settingList, sharedStringTable)));
                    }

                    // Attach row errors to comment for user visibility
                    if (rowErrors.Count > 0)
                    {
                        errorCount += rowErrors.Count;
                        string errorComment = string.Join(" | ", rowErrors);

                        if (functionRow != null)
                        {
                            if (!string.IsNullOrWhiteSpace(functionRow.Comment))
                            {
                                functionRow.Comment = ($"{functionRow.Comment} | {errorComment}");
                            }
                            else
                            {
                                functionRow.Comment = errorComment;
                            }
                            // enforce max length to fit column constraint (2048)
                            if (!string.IsNullOrEmpty(functionRow.Comment) && functionRow.Comment.Length > 2048)
                            {
                                functionRow.Comment = functionRow.Comment.Substring(0, 2048);
                            }
                        }

                        // cap total errors to avoid huge payload
                        if (errors.Count < 200)
                        {
                            errors.AddRange(rowErrors.Take(200 - errors.Count));
                        }
                    }

                    // Add function row if exists and has valid EmployeeId
                    if (functionRow != null && resolvedEmployeeId > 0)
                    {
                        retFunctionList.Add(functionRow);
                        importSummary.RowsImported++;
                    }

                    // Process leave settings and create TempPersonnelLeave records
                    // Only process if we have a valid EmployeeId
                    if (resolvedEmployeeId > 0)
                    {
                        foreach (var leaveSetting in leaveSettings)
                        {
                            if (leaveSetting.MappedExcelColumn == null || leaveSetting.LeaveTypeId == null || leaveSetting.LeaveTypeId <= 0)
                            {
                                continue;
                            }

                            var cell = GetCellByColumnIndex(rowElement, leaveSetting.MappedExcelColumn.Order, leaveSetting.MappedExcelColumn.Value);
                            var cellText = GetCellText(cell, sharedStringTable);

                            if (string.IsNullOrWhiteSpace(cellText))
                            {
                                continue; // Skip empty leave values
                            }

                            var leaveRow = new TempPersonnelLeave
                            {
                                OrganisationChartId = _currentUserDefaultOrganId,
                                CreateDate = DateTime.Now,
                                IPAddress = "",
                                EmployeeId = resolvedEmployeeId,
                                PaymentPeriodId = PaymentPeriodId,
                                LeaveTypeId = leaveSetting.LeaveTypeId.Value,
                                Day = 0,
                                Hour = 0,
                                Minute = 0,
                                PersonnelFunctionExcelFileId = FileId
                            };
                            ((HR.SharedKernel.Data.BaseEntity)leaveRow).title = Guid.NewGuid().ToString("N");

                        // Parse value based on IsDaily flag
                        if (leaveSetting.IsDaily)
                        {
                            // Value goes to Day field
                            if (decimal.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out var dayVal) ||
                                decimal.TryParse(cellText, NumberStyles.Any, CultureInfo.CurrentCulture, out dayVal))
                            {
                                leaveRow.Day = dayVal;
                            }
                            else
                            {
                                errors.Add($"ردیف {rowElement.RowIndex}: مقدار نامعتبر برای روز مرخصی در ستون {GetExcelColumnName(leaveSetting.MappedExcelColumn.Order)}");
                                errorCount++;
                            }
                        }
                        else
                        {
                            // Value is Hour:Minute format
                            var allowMinFraction = leaveSetting.IsFirstOrSecondSection && leaveSetting.NeedMinuteNormalization;
                            var normResult = NormalizeTime(cellText, allowMinFraction, true);
                            if (normResult.HasError)
                            {
                                errors.Add($"ردیف {rowElement.RowIndex}: {normResult.ErrorMessage} در ستون مرخصی {GetExcelColumnName(leaveSetting.MappedExcelColumn.Order)}");
                                errorCount++;
                            }
                            leaveRow.Hour = normResult.Hours;
                            leaveRow.Minute = normResult.Minutes;
                        }

                            retLeaveList.Add(leaveRow);
                        }
                    }
                }
            }
            _unitOfWork.Context.TempPersonnelFunctions.AddRange(retFunctionList);
            _unitOfWork.Context.TempPersonnelLeaves.AddRange(retLeaveList);
            _unitOfWork.Context.SaveChanges();
            importSummary.LeaveRowsImported = retLeaveList.Count;

            AppendImportDiagnosticsIfNeeded(importSummary, retFunctionList.Count + retLeaveList.Count, errors, ref errorCount);

            _logger.LogInformation(
                "Excel import finished. FileId={FileId}, DataStartRow={DataStartRow}, CandidateRows={CandidateRows}, HeaderSkipped={HeaderSkipped}, Processed={Processed}, Imported={Imported}, SkippedNoEmployee={SkippedNoEmployee}, LeaveImported={LeaveImported}, Errors={ErrorCount}",
                FileId, importSummary.DataStartRow, importSummary.CandidateRows, importSummary.HeaderRowsSkipped,
                importSummary.RowsProcessed, importSummary.RowsImported, importSummary.RowsSkippedNoEmployee,
                importSummary.LeaveRowsImported, errorCount);

            return retFunctionList.Count + retLeaveList.Count;
        }
        catch (Exception ex)
        {
            errorCount++;
            _logger.LogError(ex,
                "Unexpected error while parsing function excel. FileId={FileId}, EmployeeTypeId={EmployeeTypeId}, PaymentPeriodId={PaymentPeriodId}, OrganId={OrganId}",
                FileId, EmployeeTypeId, PaymentPeriodId, _currentUserDefaultOrganId);

            var errorMessage = $"خطای غیرمنتظره در پردازش فایل: {ex.Message}";
            
            // Check for inner exceptions and add them to the error message
            var innerEx = ex.InnerException;
            var innerLevel = 1;
            while (innerEx != null && innerLevel <= 3) // Limit to 3 levels to avoid too long messages
            {
                errorMessage += $" | Inner Exception {innerLevel}: {innerEx.Message}";
                innerEx = innerEx.InnerException;
                innerLevel++;
            }
            
            // Add stack trace for debugging (first 500 chars to avoid too long messages)
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                var stackTrace = ex.StackTrace.Length > 500 ? ex.StackTrace.Substring(0, 500) + "..." : ex.StackTrace;
                errorMessage += $" | Stack: {stackTrace}";
            }
            
            errors.Add(errorMessage);
            return 0;
        }

    }
    private sealed class NormalizedTimeResult
    {
        public string Normalized { get; set; } = "000:00";
        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    private static NormalizedTimeResult NormalizeTime(string? input, bool allowMinuteFractionNormalization = false, bool carryOverflowToHours = true)
    {
        var result = new NormalizedTimeResult();

        if (string.IsNullOrWhiteSpace(input))
        {
            result.Normalized = "000:00";
            result.Hours = 0;
            result.Minutes = 0;
            return result;
        }

        // Normalize digits and spaces
        var trimmed = ConvertToWesternDigits(input).Trim();
        // Remove all whitespace characters
        trimmed = new string(trimmed.Where(c => !char.IsWhiteSpace(c)).ToArray());

        // Keep relevant characters and first colon
        string hoursPart = string.Empty;
        string minutesPart = string.Empty;
        string rawMinutesToken = string.Empty;
        int colonIndex = trimmed.IndexOf(':');
        if (colonIndex >= 0)
        {
            hoursPart = new string(trimmed.Substring(0, colonIndex).Where(char.IsDigit).ToArray());
            var rest = trimmed.Substring(colonIndex + 1);
            // take up to next colon if exists
            int nextColon = rest.IndexOf(':');
            if (nextColon >= 0)
            {
                rest = rest.Substring(0, nextColon);
            }
            rawMinutesToken = rest;
            minutesPart = new string(rest.Where(char.IsDigit).ToArray());
        }
        else
        {
            if (trimmed.Contains('.') || trimmed.Contains(','))
            {
                var decimalToken = trimmed.Replace(',', '.');
                if (decimal.TryParse(decimalToken, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var decimalHours)
                    || decimal.TryParse(decimalToken, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out decimalHours))
                {
                    if (decimalHours < 0)
                    {
                        result.HasError = true;
                        result.ErrorMessage = "مقدار ساعت نمی‌تواند منفی باشد";
                    }
                    else
                    {
                        var parsedHours = (int)Math.Floor(decimalHours);
                        var parsedMinutes = (int)Math.Round((decimalHours - parsedHours) * 60m, MidpointRounding.AwayFromZero);
                        if (carryOverflowToHours && parsedMinutes >= 60)
                        {
                            parsedHours += parsedMinutes / 60;
                            parsedMinutes %= 60;
                        }

                        if (parsedHours > 999)
                        {
                            result.HasError = true;
                            result.ErrorMessage = "ساعت بیش از 3 رقم است";
                            parsedHours = Math.Min(parsedHours, 999);
                        }

                        result.Hours = parsedHours;
                        result.Minutes = parsedMinutes;
                        result.Normalized = $"{parsedHours:000}:{parsedMinutes:00}";
                        return result;
                    }
                }
            }

            hoursPart = new string(trimmed.Where(char.IsDigit).ToArray());
        }

        if (string.IsNullOrEmpty(hoursPart)) hoursPart = "0";
        if (string.IsNullOrEmpty(minutesPart)) minutesPart = "0";

        // Parse
        if (!int.TryParse(hoursPart, NumberStyles.None, CultureInfo.InvariantCulture, out var hours))
        {
            hours = 0;
            result.HasError = true;
            result.ErrorMessage = "مقدار ساعت نامعتبر است";
        }
        int minutes;
        bool hasDecimalInMinutes = !string.IsNullOrEmpty(rawMinutesToken) && (rawMinutesToken.Contains('.') || rawMinutesToken.Contains(','));
        if (hasDecimalInMinutes)
        {
            if (allowMinuteFractionNormalization)
            {
                // Normalize decimals to minutes, interpret token as fraction of hour
                var token = rawMinutesToken.Replace(',', '.');
                if (decimal.TryParse(token, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var frac))
                {
                    if (frac < 0)
                    {
                        minutes = 0;
                        result.HasError = true;
                        result.ErrorMessage = "بخش دقیقه نمی‌تواند منفی باشد";
                    }
                    else if (frac < 1)
                    {
                        minutes = (int)Math.Round(frac * 60, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        // Decimal but >= 1 is ambiguous
                        minutes = 0;
                        result.HasError = true;
                        result.ErrorMessage = "بخش دقیقه اعشاری معتبر نیست؛ باید کمتر از 1 ساعت باشد";
                    }
                }
                else
                {
                    minutes = 0;
                    result.HasError = true;
                    result.ErrorMessage = "قالب اعشاری دقیقه نامعتبر است";
                }
            }
            else
            {
                // Not allowed to normalize decimal minute; flag error
                minutes = 0;
                result.HasError = true;
                result.ErrorMessage = "بخش دقیقه نباید به صورت اعشاری وارد شود";
            }
        }
        else if (!int.TryParse(minutesPart, NumberStyles.None, CultureInfo.InvariantCulture, out minutes))
        {
            minutes = 0;
            result.HasError = true;
            result.ErrorMessage = string.IsNullOrEmpty(result.ErrorMessage) ? "مقدار دقیقه نامعتبر است" : (result.ErrorMessage + " و مقدار دقیقه نامعتبر است");
        }

        if (hours > 999)
        {
            result.HasError = true;
            result.ErrorMessage = string.IsNullOrEmpty(result.ErrorMessage) ? "ساعت بیش از 3 رقم است" : (result.ErrorMessage + " | ساعت بیش از 3 رقم است");
            hours = Math.Min(hours, 999);
        }
        if (minutes >= 60)
        {
            if (carryOverflowToHours)
            {
                hours += minutes / 60;
                minutes = minutes % 60;
                if (hours > 999)
                {
                    result.HasError = true;
                    result.ErrorMessage = string.IsNullOrEmpty(result.ErrorMessage) ? "مجموع ساعت از 999 بیشتر شد" : (result.ErrorMessage + " | مجموع ساعت از 999 بیشتر شد");
                    hours = 999;
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = string.IsNullOrEmpty(result.ErrorMessage) ? "دقیقه باید کمتر از 60 باشد" : (result.ErrorMessage + " | دقیقه باید کمتر از 60 باشد");
                minutes = minutes % 60; // normalize to 0-59
            }
        }

        result.Hours = hours;
        result.Minutes = minutes;
        result.Normalized = $"{hours.ToString("D3", CultureInfo.InvariantCulture)}:{minutes.ToString("D2", CultureInfo.InvariantCulture)}";
        return result;
    }

    private static string ConvertToWesternDigits(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var map = new Dictionary<char, char>
        {
            ['۰'] = '0',
            ['۱'] = '1',
            ['۲'] = '2',
            ['۳'] = '3',
            ['۴'] = '4',
            ['۵'] = '5',
            ['۶'] = '6',
            ['۷'] = '7',
            ['۸'] = '8',
            ['۹'] = '9',
            ['٠'] = '0',
            ['١'] = '1',
            ['٢'] = '2',
            ['٣'] = '3',
            ['٤'] = '4',
            ['٥'] = '5',
            ['٦'] = '6',
            ['٧'] = '7',
            ['٨'] = '8',
            ['٩'] = '9'
        };
        var chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (map.TryGetValue(chars[i], out var latin))
            {
                chars[i] = latin;
            }
        }
        return new string(chars);
    }
    private static string BuildZeroImportUserMessage(ExcelFunctionImportSummary summary, int errorCount)
    {
        if (errorCount > 0)
        {
            return $"ردیفی وارد سیستم نشد؛ {errorCount} خطا در خواندن ثبت شد";
        }

        if (summary.CandidateRows == 0)
        {
            return "ردیفی در محدوده شروع داده اکسل یافت نشد";
        }

        if (summary.HeaderRowsSkipped >= summary.CandidateRows)
        {
            return "ردیف داده‌ای در فایل شناسایی نشد؛ احتمالاً ردیف شروع داده باید بعد از سطر عنوان‌ها تنظیم شود";
        }

        if (summary.RowsSkippedNoEmployee > 0)
        {
            return $"هیچ ردیفی وارد نشد؛ {summary.RowsSkippedNoEmployee} ردیف به‌دلیل شناسایی‌نشدن کارمند (کد ملی) نادیده گرفته شد";
        }

        return "ردیفی برای خواندن یافت نشد";
    }

    private static void AppendImportDiagnosticsIfNeeded(ExcelFunctionImportSummary summary, int importedCount, List<string> errors, ref int errorCount)
    {
        if (importedCount > 0)
        {
            return;
        }

        if (summary.CandidateRows == 0)
        {
            AddDiagnosticError(errors, ref errorCount, $"هیچ ردیفی از ردیف {summary.DataStartRow} به بعد در فایل وجود ندارد");
            return;
        }

        if (summary.HeaderRowsSkipped >= summary.CandidateRows)
        {
            AddDiagnosticError(errors, ref errorCount,
                $"تمام {summary.CandidateRows} ردیف خوانده‌شده به‌عنوان سطر عنوان شناسایی شدند. ردیف شروع داده (تنظیم 10018) را بررسی کنید");
            return;
        }

        if (summary.RowsProcessed == 0)
        {
            AddDiagnosticError(errors, ref errorCount, "هیچ ردیف داده‌ای برای پردازش باقی نماند");
            return;
        }

        if (summary.RowsSkippedNoEmployee > 0 && errors.Count == 0)
        {
            AddDiagnosticError(errors, ref errorCount,
                $"هیچ ردیفی وارد نشد؛ {summary.RowsSkippedNoEmployee} ردیف داده بدون شناسایی کارمند رد شد");
        }
    }

    private static void AddDiagnosticError(List<string> errors, ref int errorCount, string message)
    {
        if (errors.Contains(message))
        {
            return;
        }

        errorCount++;
        if (errors.Count < 200)
        {
            errors.Add(message);
        }
    }

    private static string GetNationalNoColumnLabel(IReadOnlyList<FunctionExcelDefinition> settingList)
    {
        var nationalNoSetting = settingList
            .Where(s => string.Equals(s.PersonnelFunctionColumn?.Value, "NationalNo", StringComparison.OrdinalIgnoreCase)
                && s.MappedExcelColumn != null)
            .OrderByDescending(s => s.MappedExcelColumn!.Order)
            .FirstOrDefault();

        return nationalNoSetting?.MappedExcelColumn?.Value
            ?? GetExcelColumnName(nationalNoSetting?.MappedExcelColumn?.Order ?? 4);
    }

    private static string GetNationalNoCellText(Row rowElement, IReadOnlyList<FunctionExcelDefinition> settingList, SharedStringTable? sharedStringTable)
    {
        var nationalNoSetting = settingList
            .Where(s => string.Equals(s.PersonnelFunctionColumn?.Value, "NationalNo", StringComparison.OrdinalIgnoreCase)
                && s.MappedExcelColumn != null)
            .OrderByDescending(s => s.MappedExcelColumn!.Order)
            .FirstOrDefault();

        if (nationalNoSetting?.MappedExcelColumn == null)
        {
            return string.Empty;
        }

        var cell = GetCellByColumnIndex(rowElement, nationalNoSetting.MappedExcelColumn.Order, nationalNoSetting.MappedExcelColumn.Value);
        return GetCellText(cell, sharedStringTable);
    }

    private static bool ShouldSkipRowAsHeader(Row rowElement, IReadOnlyList<FunctionExcelDefinition> functionSettings, SharedStringTable? sharedStringTable)
    {
        if (RowHasPlausibleEmployeeIdentity(rowElement, functionSettings, sharedStringTable))
        {
            return false;
        }

        var labelCells = 0;
        var checkedCells = 0;
        foreach (var setting in functionSettings)
        {
            if (setting.MappedExcelColumn == null || string.IsNullOrWhiteSpace(setting.PersonnelFunctionColumn?.Value))
            {
                continue;
            }

            var property = typeof(TempPersonnelFunction).GetProperty(setting.PersonnelFunctionColumn.Value);
            if (property == null || !IsNumericMappedPropertyType(property.PropertyType.FullName))
            {
                continue;
            }

            checkedCells++;
            var cellText = GetCellText(
                GetCellByColumnIndex(rowElement, setting.MappedExcelColumn.Order, setting.MappedExcelColumn.Value),
                sharedStringTable);

            if (string.IsNullOrWhiteSpace(cellText))
            {
                continue;
            }

            if (LooksLikeHeaderLabel(cellText))
            {
                labelCells++;
            }
            else
            {
                return false;
            }
        }

        return checkedCells > 0 && labelCells > 0;
    }

    private static bool RowHasPlausibleEmployeeIdentity(Row rowElement, IReadOnlyList<FunctionExcelDefinition> settings, SharedStringTable? sharedStringTable)
    {
        foreach (var nationalSetting in settings
            .Where(s => string.Equals(s.PersonnelFunctionColumn?.Value, "NationalNo", StringComparison.OrdinalIgnoreCase)
                && s.MappedExcelColumn != null)
            .OrderByDescending(s => s.MappedExcelColumn!.Order))
        {
            var cellText = GetCellText(
                GetCellByColumnIndex(rowElement, nationalSetting.MappedExcelColumn!.Order, nationalSetting.MappedExcelColumn.Value),
                sharedStringTable);
            if (ExtractDigits(cellText).Length >= 8)
            {
                return true;
            }
        }

        foreach (var candidate in FindNationalNoCandidatesInRow(rowElement, sharedStringTable))
        {
            if (ExtractDigits(candidate).Length >= 8)
            {
                return true;
            }
        }

        var employeeIdSetting = settings.FirstOrDefault(s =>
            string.Equals(s.PersonnelFunctionColumn?.Value, "EmployeeId", StringComparison.OrdinalIgnoreCase)
            && s.MappedExcelColumn != null);
        if (employeeIdSetting != null)
        {
            var cellText = GetCellText(
                GetCellByColumnIndex(rowElement, employeeIdSetting.MappedExcelColumn!.Order, employeeIdSetting.MappedExcelColumn.Value),
                sharedStringTable);
            var normalized = NormalizeNumericCellText(cellText);
            if (long.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId) && employeeId > 0)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsNumericMappedPropertyType(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return false;
        }

        var normalized = typeName.ToLowerInvariant();
        return normalized.Contains("int32")
            || normalized.Contains("int64")
            || normalized.Contains("decimal")
            || normalized.Contains("single")
            || normalized.Contains("double");
    }

    private static bool LooksLikeHeaderLabel(string? cellText)
    {
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return false;
        }

        var trimmed = cellText.Trim();
        if (!trimmed.Any(char.IsLetter))
        {
            return false;
        }

        if (LooksLikeTimeValue(trimmed) || LooksLikeDecimalHoursValue(trimmed))
        {
            return false;
        }

        var normalized = NormalizeNumericCellText(trimmed);
        if (!string.IsNullOrEmpty(normalized)
            && (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                || decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out _)))
        {
            return false;
        }

        return true;
    }

    private static string NormalizeNumericCellText(string? cellText)
    {
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return string.Empty;
        }

        var normalized = ConvertToWesternDigits(cellText).Trim();
        normalized = normalized.Replace("٬", string.Empty, StringComparison.Ordinal)
            .Replace(",", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        if (normalized.Length == 0
            || normalized.Equals("-", StringComparison.Ordinal)
            || normalized.Equals("—", StringComparison.Ordinal)
            || normalized.Equals("–", StringComparison.Ordinal)
            || normalized.Equals("_", StringComparison.Ordinal)
            || normalized.Equals("null", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("n/a", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return normalized;
    }

    private static string FormatCellValueForError(string? cellText)
    {
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return "(خالی)";
        }

        var trimmed = cellText.Trim();
        return trimmed.Length <= 40 ? trimmed : trimmed.Substring(0, 40) + "...";
    }

    private static int GetColumnIndexFromName(string? columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
        {
            return 0;
        }

        var index = 0;
        foreach (var ch in columnName.Trim().ToUpperInvariant())
        {
            if (ch is < 'A' or > 'Z')
            {
                continue;
            }

            index = (index * 26) + (ch - 'A' + 1);
        }

        return index;
    }

    private static string ResolveTargetColumnName(long columnOrder, string? mappedColumnValue)
    {
        if (!string.IsNullOrWhiteSpace(mappedColumnValue))
        {
            var candidate = mappedColumnValue.Trim();
            if (candidate.All(ch => char.IsLetter(ch)))
            {
                return candidate.ToUpperInvariant();
            }
        }

        return GetExcelColumnName(columnOrder);
    }

    private static Cell? GetCellByColumnIndex(Row rowElement, long columnIndex, string? mappedColumnValue = null)
    {
        var targetColumnName = ResolveTargetColumnName(columnIndex, mappedColumnValue);
        var targetColumnNumber = GetColumnIndexFromName(targetColumnName);
        if (targetColumnNumber <= 0)
        {
            return null;
        }

        var cellWithReference = rowElement.Elements<Cell>()
            .FirstOrDefault(c => string.Compare(GetColumnName(c.CellReference?.Value), targetColumnName, StringComparison.OrdinalIgnoreCase) == 0);
        if (cellWithReference != null)
        {
            return cellWithReference;
        }

        var currentColumnNumber = 0;
        foreach (var cell in rowElement.Elements<Cell>())
        {
            if (!string.IsNullOrWhiteSpace(cell.CellReference?.Value))
            {
                currentColumnNumber = GetColumnIndexFromName(GetColumnName(cell.CellReference));
            }
            else if (currentColumnNumber > 0)
            {
                currentColumnNumber++;
            }
            else
            {
                currentColumnNumber = 1;
            }

            if (currentColumnNumber == targetColumnNumber)
            {
                return cell;
            }
        }

        return null;
    }

    private long TryResolveEmployeeIdByNationalNo(string cellText, Row rowElement, SharedStringTable? sharedStringTable, out string attemptedNationalNo)
    {
        attemptedNationalNo = string.Empty;
        var candidates = BuildNationalNoCandidates(cellText, rowElement, sharedStringTable);
        foreach (var candidate in candidates)
        {
            attemptedNationalNo = candidate;
            var employee = _employeeService.All().FirstOrDefault(i => i.NationalNo == candidate);
            if (employee != null)
            {
                return employee.Id;
            }
        }

        // Retry without leading zeros for databases that store unpadded national IDs.
        foreach (var candidate in candidates)
        {
            var trimmed = candidate.TrimStart('0');
            if (string.IsNullOrEmpty(trimmed))
            {
                continue;
            }

            attemptedNationalNo = trimmed;
            var employee = _employeeService.All().FirstOrDefault(i => i.NationalNo == trimmed || i.NationalNo == candidate);
            if (employee != null)
            {
                return employee.Id;
            }
        }

        return 0;
    }

    private static List<string> BuildNationalNoCandidates(string cellText, Row rowElement, SharedStringTable? sharedStringTable)
    {
        var candidates = new List<string>();
        var primary = NormalizeNationalNo(cellText);
        var primaryDigits = ExtractDigits(cellText);

        // Short values (e.g. row index in column A) are usually not national IDs.
        if (primaryDigits.Length >= 8 && !string.IsNullOrEmpty(primary))
        {
            candidates.Add(primary);
        }

        foreach (var candidate in FindNationalNoCandidatesInRow(rowElement, sharedStringTable))
        {
            if (!candidates.Contains(candidate, StringComparer.Ordinal))
            {
                candidates.Add(candidate);
            }
        }

        if (candidates.Count == 0 && !string.IsNullOrEmpty(primary))
        {
            candidates.Add(primary);
        }

        return candidates;
    }

    private static IEnumerable<string> FindNationalNoCandidatesInRow(Row rowElement, SharedStringTable? sharedStringTable)
    {
        foreach (var cell in rowElement.Elements<Cell>())
        {
            var text = GetCellText(cell, sharedStringTable);
            var normalized = NormalizeNationalNo(text);
            if (string.IsNullOrEmpty(normalized))
            {
                continue;
            }

            var digits = ExtractDigits(text);
            if (digits.Length < 8 || IsLikelyRowIndexNationalNo(normalized))
            {
                continue;
            }

            yield return normalized;
        }
    }

    private static string? NormalizeNationalNo(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var digitsOnly = ExtractDigits(raw);
        if (string.IsNullOrEmpty(digitsOnly))
        {
            return null;
        }

        if (digitsOnly.Length > 10)
        {
            digitsOnly = digitsOnly.Substring(digitsOnly.Length - 10, 10);
        }

        return digitsOnly.PadLeft(10, '0');
    }

    private static string ExtractDigits(string? raw)
    {
        return Regex.Replace(ConvertToWesternDigits(raw ?? string.Empty), @"\D", string.Empty);
    }

    private static bool IsLikelyRowIndexNationalNo(string normalized)
    {
        if (!normalized.StartsWith("0000000", StringComparison.Ordinal))
        {
            return false;
        }

        return long.TryParse(normalized, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
            && value > 0
            && value < 10000;
    }

    private static bool LooksLikeTimeValue(string? cellText)
    {
        return !string.IsNullOrWhiteSpace(cellText) && ConvertToWesternDigits(cellText).Contains(':');
    }

    private static bool LooksLikeDecimalHoursValue(string? cellText)
    {
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return false;
        }

        var normalized = ConvertToWesternDigits(cellText).Trim();
        if (normalized.Contains(':'))
        {
            return false;
        }

        return (normalized.Contains('.') || normalized.Contains(','))
            && (decimal.TryParse(normalized.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _)
                || decimal.TryParse(normalized.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out _));
    }

    private static bool TryParseInt32CellValue(string? cellText, FunctionExcelDefinition setting, out int value, out string? warning)
    {
        value = 0;
        warning = null;
        cellText = NormalizeNumericCellText(cellText);
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return true;
        }

        if (setting.IsHourMinute || LooksLikeTimeValue(cellText) || LooksLikeDecimalHoursValue(cellText))
        {
            return TryParseHourMinutePart(cellText, setting, out value, out warning);
        }

        if (decimal.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue)
            || decimal.TryParse(cellText, NumberStyles.Any, CultureInfo.CurrentCulture, out decimalValue))
        {
            value = (int)Math.Round(decimalValue, MidpointRounding.AwayFromZero);
            return true;
        }

        return int.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
            || int.TryParse(cellText, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }

    private static bool TryParseInt64CellValue(string? cellText, FunctionExcelDefinition setting, out long value, out string? warning)
    {
        value = 0;
        warning = null;
        cellText = NormalizeNumericCellText(cellText);
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return true;
        }

        if (setting.IsHourMinute || LooksLikeTimeValue(cellText) || LooksLikeDecimalHoursValue(cellText))
        {
            if (!TryParseHourMinutePart(cellText, setting, out var intPart, out warning))
            {
                return false;
            }

            value = intPart;
            return true;
        }

        return long.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
            || long.TryParse(cellText, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }

    private static bool TryParseDecimalCellValue(string? cellText, FunctionExcelDefinition setting, out decimal value, out string? warning)
    {
        value = 0;
        warning = null;
        var rawCellText = cellText;
        cellText = NormalizeNumericCellText(cellText);
        if (string.IsNullOrWhiteSpace(cellText))
        {
            return true;
        }

        if (setting.IsHourMinute || LooksLikeTimeValue(rawCellText) || LooksLikeDecimalHoursValue(rawCellText))
        {
            var allowMinFraction = setting.IsFirstOrSecondSection && setting.NeedMinuteNormalization;
            var normResult = NormalizeTime(rawCellText, allowMinFraction, true);
            if (normResult.HasError)
            {
                warning = normResult.ErrorMessage;
            }

            if (setting.IsHourMinute)
            {
                value = setting.IsFirstOrSecondSection ? normResult.Hours : normResult.Minutes;
                return true;
            }

            value = normResult.Hours + (normResult.Minutes / 60.0m);
            return true;
        }

        return decimal.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
            || decimal.TryParse(cellText, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }

    private static bool TryParseHourMinutePart(string? cellText, FunctionExcelDefinition setting, out int value, out string? warning)
    {
        value = 0;
        warning = null;
        var allowMinFraction = setting.IsFirstOrSecondSection && setting.NeedMinuteNormalization;
        var normResult = NormalizeTime(cellText, allowMinFraction, true);
        if (normResult.HasError)
        {
            warning = normResult.ErrorMessage;
        }

        if (setting.IsHourMinute)
        {
            value = setting.IsFirstOrSecondSection ? normResult.Hours : normResult.Minutes;
        }
        else
        {
            // Auto-detected HH:mm defaults to hours.
            value = normResult.Hours;
        }

        return true;
    }

    private static string GetCellText(Cell? cell, SharedStringTable? sharedStringTable)
    {
        if (cell == null || cell.CellValue == null)
        {
            return string.Empty;
        }
        var value = cell.CellValue.InnerText;
        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            if (int.TryParse(value, out var sstIndex) && sharedStringTable != null)
            {
                return sharedStringTable.ElementAt(sstIndex).InnerText ?? string.Empty;
            }
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var numericValue))
        {
            if (Math.Abs(numericValue - Math.Round(numericValue)) < 0.0000001d)
            {
                return ((long)Math.Round(numericValue)).ToString(CultureInfo.InvariantCulture);
            }

            return numericValue.ToString(CultureInfo.InvariantCulture);
        }

        return ConvertToWesternDigits(value);
    }

    private static string GetColumnName(string? cellReference)
    {
        if (string.IsNullOrEmpty(cellReference)) return string.Empty;
        var columnName = new string(cellReference.Where(c => char.IsLetter(c)).ToArray());
        return columnName;
    }

    private static string GetExcelColumnName(long columnNumber)
    {
        var dividend = columnNumber;
        var columnName = string.Empty;
        long modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo) + columnName;
            dividend = (dividend - modulo) / 26;
        }
        return columnName;
    }
 
}
