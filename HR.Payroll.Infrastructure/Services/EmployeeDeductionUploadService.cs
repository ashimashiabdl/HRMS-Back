using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HR.Payroll.Infrastructure.Services;

public class EmployeeDeductionUploadService : IScopedServices
{
    private readonly IUnitOfWork<PayrollContext> _unitOfWork;
    private readonly IDapper _dapper;
    private readonly string _connectionString;
    private readonly ILogger<EmployeeDeductionUploadService> _logger;

    public long _currentUserDefaultOrganId { get; set; }

    public EmployeeDeductionUploadService(
        IUnitOfWork<PayrollContext> unitOfWork,
        IDapper dapper,
        UserResolverService userService,
        ILogger<EmployeeDeductionUploadService> logger)
    {
        _unitOfWork = unitOfWork;
        _dapper = dapper;
        _logger = logger;
        _connectionString = GetConnectionString();
    }

    private static string GetConnectionString()
    {
#if DEBUG
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!)
            .AddJsonFile("appsettings.Development.json")
            .Build();
#else
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!)
            .AddJsonFile("appsettings.json")
            .Build();
#endif
        var raw = configuration.GetConnectionString("HRMSConnection");
        var dec = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw);
        return dec ?? raw ?? "";
    }

    /// <summary>
    /// تبدیل اعداد فارسی/عربی به انگلیسی
    /// </summary>
    private static string NormalizeDigitsToEnglish(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (c >= '\u06F0' && c <= '\u06F9') sb.Append((char)('0' + (c - '\u06F0')));
            else if (c >= '\u0660' && c <= '\u0669') sb.Append((char)('0' + (c - '\u0660')));
            else if (c >= '\u06C0' && c <= '\u06C9') sb.Append((char)('0' + (c - '\u06C0')));
            else if (char.IsDigit(c)) sb.Append(c);
            else sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>
    /// استخراج ردیف‌های CSV: اگر خط اول شبیه هدر باشد (ستون اول ۱۰ رقم نباشد) نادیده گرفته می‌شود؛ وگرنه همه خطوط داده‌اند.
    /// ستون اول کد ملی، ستون دوم مبلغ. جداکننده: کاما. پشتیبانی از UTF-8 و BOM.
    /// </summary>
    private static List<CsvRow> ParseCsvRows(byte[] fileContent)
    {
        var list = new List<CsvRow>();
        using var reader = new StreamReader(new MemoryStream(fileContent), Encoding.UTF8);
        string? line;
        var isFirst = true;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            var parts = line.Split(',');
            var nationalRaw = parts.Length > 0 ? parts[0].Trim() : "";
            var amountRaw = parts.Length > 1 ? parts[1].Trim() : "";
            if (isFirst)
            {
                isFirst = false;
                var normalized = NormalizeNationalNo(nationalRaw);
                if (normalized == null || normalized.Length != 10)
                    continue; // skip header line (e.g. "کد ملی,مبلغ")
            }
            list.Add(new CsvRow(nationalRaw, amountRaw));
        }
        return list;
    }

    private sealed record CsvRow(string NationalRaw, string AmountRaw);

    /// <summary>
    /// نرمال‌سازی کد ملی: فقط رقم، پد چپ ۱۰ با صفر
    /// </summary>
    private static string? NormalizeNationalNo(string? raw)
    {
        if (raw == null) return null;
        var digitsOnly = Regex.Replace(NormalizeDigitsToEnglish(raw), @"\D", "");
        if (string.IsNullOrEmpty(digitsOnly)) return null;
        if (digitsOnly.Length > 10) digitsOnly = digitsOnly.Substring(0, 10);
        return digitsOnly.PadLeft(10, '0');
    }

    /// <summary>
    /// استخراج عدد خالص از ستون مبلغ (حذف جداکننده هزارگان و کاراکترهای اضافی)
    /// </summary>
    private static bool TryParseAmount(string? raw, out long value, out string? errorMessage)
    {
        value = 0;
        errorMessage = null;
        if (raw == null || string.IsNullOrWhiteSpace(raw))
        {
            errorMessage = "مقدار خالی است.";
            return false;
        }
        var normalized = NormalizeDigitsToEnglish(raw);
        var digitsOnly = Regex.Replace(normalized, @"[^\d]", "");
        if (string.IsNullOrEmpty(digitsOnly))
        {
            errorMessage = "هیچ رقم معتبری در مبلغ یافت نشد.";
            return false;
        }
        if (!long.TryParse(digitsOnly, NumberStyles.None, CultureInfo.InvariantCulture, out value))
        {
            errorMessage = "عدد خارج از محدوده مجاز است.";
            return false;
        }
        if (value <= 0)
        {
            errorMessage = "مبلغ باید بزرگ‌تر از صفر باشد.";
            return false;
        }
        return true;
    }

    private long GetEmployeeIdFromNationalNo(string nationalNo)
    {
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[emp].[GetEmployeeIdFromNationalNo]";
            command.Parameters.AddWithValue("@NationalNO", nationalNo);
            var returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.BigInt);
            returnValue.Direction = ParameterDirection.ReturnValue;
            connection.Open();
            command.ExecuteNonQuery();
            return Convert.ToInt64(returnValue.Value ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GetEmployeeIdFromNationalNo failed for {NationalNo}", nationalNo);
            return 0;
        }
    }

    public async Task<OperationResult> ProcessUploadAsync(
        byte[] fileContent,
        long fileId,
        string batchTitle,
        long deductionTypeId,
        long organisationChartId,
        string? uploaderUserName,
        string? uploaderDisplayName,
        long startDeductPaymentPeriodId,
        DateTime paymentDate)
    {
        var failedRows = new List<EmployeeDeductionUploadFailedRowDTO>();
        int totalRowsRead = 0;
        int successCount = 0;

        var ctx = _unitOfWork.Context;
        var deductionType = await ctx.DeductionTypes
            .FirstOrDefaultAsync(d => d.Id == deductionTypeId && d.OrganisationChartId == organisationChartId);
        if (deductionType == null)
            return OperationResult.Failed("نوع کسور معتبر نیست.");

        var period = await ctx.PaymentPeriods
            .FirstOrDefaultAsync(p => p.Id == startDeductPaymentPeriodId && p.OrganisationChartId == organisationChartId);
        if (period == null)
            return OperationResult.Failed("دوره پرداخت معتبر نیست.");

        var normalizedTitle = string.IsNullOrWhiteSpace(batchTitle)
            ? $"آپلود کسورات - {DateTime.Now:yyyy/MM/dd HH:mm}"
            : batchTitle.Trim();

        var batch = new EmployeeDeductionUploadBatch
        {
            title = normalizedTitle,
            OrganisationChartId = organisationChartId,
            FileId = fileId,
            UploaderUserName = uploaderUserName ?? "",
            UploaderDisplayName = uploaderDisplayName ?? "",
            CreateDate = DateTime.Now,
            IPAddress = null,
            IsDeleted = false,
            TotalRowsRead = 0,
            SuccessCount = 0,
            FailedCount = 0
        };
        ctx.EmployeeDeductionUploadBatches.Add(batch);
        await ctx.SaveChangesAsync();
        var batchId = batch.Id;

        try
        {
            var rows = ParseCsvRows(fileContent);
            if (rows.Count < 1)
            {
                await RemoveBatchOnFailure(ctx, batchId);
                return OperationResult.Failed("فایل CSV فاقد داده است (حداقل یک ردیف داده لازم است).");
            }

            var tempList = new List<TempEmployeeDeduction>();
            for (var i = 0; i < rows.Count; i++)
            {
                var rowIndex = i + 1; // شماره خط در فایل (۱-based)
                var row = rows[i];
                var cellNational = row.NationalRaw;
                var cellAmount = row.AmountRaw;

                totalRowsRead++;

                var nationalNo = NormalizeNationalNo(cellNational);
                if (string.IsNullOrEmpty(nationalNo) || nationalNo.Length != 10)
                {
                    failedRows.Add(new EmployeeDeductionUploadFailedRowDTO
                    {
                        ExcelRowNumber = rowIndex,
                        NationalNoRaw = cellNational,
                        AmountRaw = cellAmount,
                        ErrorMessage = "کد ملی معتبر نیست (باید ۱۰ رقم باشد)."
                    });
                    continue;
                }

                if (!TryParseAmount(cellAmount, out var amount, out var amountError))
                {
                    failedRows.Add(new EmployeeDeductionUploadFailedRowDTO
                    {
                        ExcelRowNumber = rowIndex,
                        NationalNoRaw = cellNational,
                        AmountRaw = cellAmount,
                        ErrorMessage = amountError ?? "مبلغ نامعتبر."
                    });
                    continue;
                }

                var employeeId = GetEmployeeIdFromNationalNo(nationalNo);
                if (employeeId <= 0)
                {
                    failedRows.Add(new EmployeeDeductionUploadFailedRowDTO
                    {
                        ExcelRowNumber = rowIndex,
                        NationalNoRaw = nationalNo,
                        AmountRaw = cellAmount,
                        ErrorMessage = "کاربری با این کد ملی یافت نشد."
                    });
                    continue;
                }

                var temp = new TempEmployeeDeduction
                {
                    title = "",
                    CreateDate = DateTime.Now,
                    IsDeleted = false,
                    EmployeeDeductionUploadBatchId = batchId,
                    FileId = fileId,
                    EmployeeId = employeeId,
                    NationalNo = nationalNo,
                    DeductionTypeId = deductionTypeId,
                    OrganisationChartId = organisationChartId,
                    StartDeductPaymentPeriodId = startDeductPaymentPeriodId,
                    PaymentDate = paymentDate,
                    InstallmentAmount = amount,
                    AllAmount = amount,
                    RemainingCrumbsAtFirst = false,
                    IsActive = true,
                    LoanPaymentDocDesc = "آپلود گروهی از CSV"
                };
                tempList.Add(temp);
                successCount++;
            }

            if (tempList.Count > 0)
            {
                ctx.TempEmployeeDeductions.AddRange(tempList);
                await ctx.SaveChangesAsync();
            }

            batch.TotalRowsRead = totalRowsRead;
            batch.SuccessCount = successCount;
            batch.FailedCount = failedRows.Count;
            batch.FailedRowsJson = failedRows.Count > 0
                ? JsonSerializer.Serialize(failedRows.Take(500).ToList())
                : null;
            await ctx.SaveChangesAsync();

            var result = new EmployeeDeductionUploadResultDTO
            {
                EmployeeDeductionUploadBatchId = batchId,
                FileId = fileId,
                TotalRowsRead = totalRowsRead,
                SuccessCount = successCount,
                FailedCount = failedRows.Count,
                FailedRows = failedRows
            };
            return OperationResult.Succeeded(payload: result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProcessUploadAsync failed for FileId={FileId}, BatchId={BatchId}", fileId, batchId);
            await RemoveBatchOnFailure(ctx, batchId);
            return OperationResult.Failed("خطا در پردازش فایل CSV: " + ex.Message);
        }
    }

    private static async Task RemoveBatchOnFailure(PayrollContext ctx, long batchId)
    {
        var batch = await ctx.EmployeeDeductionUploadBatches.FindAsync(batchId);
        if (batch != null)
        {
            ctx.EmployeeDeductionUploadBatches.Remove(batch);
            await ctx.SaveChangesAsync();
        }
    }

    public OperationResult GetPreviewPaged(
        long employeeDeductionUploadBatchId,
        int currentPage,
        int pageSize,
        string? filter,
        string? activeSortColumn,
        string? sortDirection)
    {
        var query = _unitOfWork.Context.TempEmployeeDeductions
            .Include(t => t.Employee)
            .Include(t => t.DeductionType)
            .Where(t => t.EmployeeDeductionUploadBatchId == employeeDeductionUploadBatchId
                        && t.OrganisationChartId == _currentUserDefaultOrganId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(t =>
                (t.NationalNo != null && t.NationalNo.Contains(f)) ||
                (t.Employee != null && t.Employee.FirstName != null && t.Employee.FirstName.ToLower().Contains(f)) ||
                (t.Employee != null && t.Employee.LastName != null && t.Employee.LastName.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "id";
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        IQueryable<TempEmployeeDeduction> ordered = orderBy switch
        {
            "nationalno" => desc ? query.OrderByDescending(x => x.NationalNo) : query.OrderBy(x => x.NationalNo),
            "employeefullname" or "fullname" => desc
                ? query.OrderByDescending(x => x.Employee != null ? x.Employee.FirstName + " " + x.Employee.LastName : "")
                : query.OrderBy(x => x.Employee != null ? x.Employee.FirstName + " " + x.Employee.LastName : ""),
            "installmentamount" => desc ? query.OrderByDescending(x => x.InstallmentAmount) : query.OrderBy(x => x.InstallmentAmount),
            "allamount" => desc ? query.OrderByDescending(x => x.AllAmount) : query.OrderBy(x => x.AllAmount),
            _ => desc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
        };

        var page = ordered
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TempEmployeeDeductionPreviewDTO
            {
                Id = t.Id,
                NationalNo = t.NationalNo,
                EmployeeId = t.EmployeeId,
                EmployeeFullName = t.Employee != null ? (t.Employee.FirstName + " " + t.Employee.LastName) : null,
                DeductionTypeTitle = t.DeductionType != null ? t.DeductionType.title : null,
                InstallmentAmount = t.InstallmentAmount,
                AllAmount = t.AllAmount,
                ParseErrorMessage = t.ParseErrorMessage
            })
            .ToList();

        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }

    public OperationResult GetUploadReport(long employeeDeductionUploadBatchId)
    {
        var batch = _unitOfWork.Context.EmployeeDeductionUploadBatches
            .FirstOrDefault(b => b.Id == employeeDeductionUploadBatchId && b.OrganisationChartId == _currentUserDefaultOrganId);
        if (batch == null)
            return OperationResult.NotFound();

        var failedRows = new List<EmployeeDeductionUploadFailedRowDTO>();
        if (!string.IsNullOrEmpty(batch.FailedRowsJson))
        {
            try
            {
                failedRows = JsonSerializer.Deserialize<List<EmployeeDeductionUploadFailedRowDTO>>(batch.FailedRowsJson) ?? failedRows;
            }
            catch { /* ignore */ }
        }

        var dto = new EmployeeDeductionUploadResultDTO
        {
            EmployeeDeductionUploadBatchId = batch.Id,
            FileId = batch.FileId,
            TotalRowsRead = batch.TotalRowsRead,
            SuccessCount = batch.SuccessCount,
            FailedCount = batch.FailedCount,
            FailedRows = failedRows
        };
        return OperationResult.Succeeded(payload: dto);
    }

    public async Task<OperationResult> FinalizeAsync(long employeeDeductionUploadBatchId)
    {
        var batch = await _unitOfWork.Context.EmployeeDeductionUploadBatches
            .Include(b => b.OrganisationChart)
            .FirstOrDefaultAsync(b => b.Id == employeeDeductionUploadBatchId && b.OrganisationChartId == _currentUserDefaultOrganId);
        if (batch == null)
            return OperationResult.NotFound();

        var temps = await _unitOfWork.Context.TempEmployeeDeductions
            .Include(t => t.DeductionType)
            .Include(t => t.StartDeductPaymentPeriod)
            .Where(t => t.EmployeeDeductionUploadBatchId == employeeDeductionUploadBatchId && t.EmployeeId != null && t.EmployeeId > 0)
            .ToListAsync();

        var list = new List<EmployeeDeduction>();
        foreach (var t in temps)
        {
            var ed = new EmployeeDeduction
            {
                title = "",
                CreateDate = DateTime.Now,
                IsDeleted = false,
                EmployeeId = t.EmployeeId!.Value,
                DeductionTypeId = t.DeductionTypeId,
                OrganisationChartId = t.OrganisationChartId,
                StartDeductPaymentPeriodId = t.StartDeductPaymentPeriodId,
                PaymentDate = t.PaymentDate,
                LoanPaymentDocDesc = t.LoanPaymentDocDesc ?? "آپلود گروهی از اکسل",
                AllAmount = t.AllAmount,
                InstallmentAmount = t.InstallmentAmount,
                RemainingCrumbsAtFirst = t.RemainingCrumbsAtFirst,
                IsActive = t.IsActive,
                FileId = batch.FileId,
                TempEmployeeDeductionId = t.Id,
                EmployeeDeductionUploadBatchId = batch.Id
            };
            list.Add(ed);
        }

        _unitOfWork.Context.EmployeeDeductions.AddRange(list);
        await _unitOfWork.Context.SaveChangesAsync();
        return OperationResult.Succeeded($"{list.Count} ردیف به جدول کسور کارکنان منتقل شد.",payload: list.Count);
    }

    /// <summary>
    /// لیست صفحه‌بندی‌شده دسته‌های آپلود کسورات برای سازمان جاری
    /// </summary>
    public OperationResult GetBatchesPaged(int currentPage, int pageSize, string? filter, string? activeSortColumn, string? sortDirection)
    {
        var ctx = _unitOfWork.Context;
        var query = ctx.EmployeeDeductionUploadBatches
            .Where(b => b.OrganisationChartId == _currentUserDefaultOrganId && !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(b =>
                (b.title != null && b.title.ToLower().Contains(f)) ||
                (b.UploaderDisplayName != null && b.UploaderDisplayName.ToLower().Contains(f)) ||
                (b.UploaderUserName != null && b.UploaderUserName.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "id";
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        IQueryable<EmployeeDeductionUploadBatch> ordered = orderBy switch
        {
            "title" => desc ? query.OrderByDescending(x => x.title) : query.OrderBy(x => x.title),
            "createdate" => desc ? query.OrderByDescending(x => x.CreateDate) : query.OrderBy(x => x.CreateDate),
            "uploaderdisplayname" => desc ? query.OrderByDescending(x => x.UploaderDisplayName) : query.OrderBy(x => x.UploaderDisplayName),
            "totalrowsread" => desc ? query.OrderByDescending(x => x.TotalRowsRead) : query.OrderBy(x => x.TotalRowsRead),
            "successcount" => desc ? query.OrderByDescending(x => x.SuccessCount) : query.OrderBy(x => x.SuccessCount),
            "failedcount" => desc ? query.OrderByDescending(x => x.FailedCount) : query.OrderBy(x => x.FailedCount),
            _ => desc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
        };

        var page = ordered
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new EmployeeDeductionUploadBatchDTO
            {
                Id = b.Id,
                title = b.title,
                CreateDate = b.CreateDate,
                OrganisationChartId = b.OrganisationChartId,
                FileId = b.FileId,
                UploaderUserName = b.UploaderUserName,
                UploaderDisplayName = b.UploaderDisplayName,
                TotalRowsRead = b.TotalRowsRead,
                SuccessCount = b.SuccessCount,
                FailedCount = b.FailedCount
            })
            .ToList();

        // خواندن نام فایل آپلود شده بر اساس FileId
        if (page.Count > 0)
        {
            var fileIds = page.Select(p => p.FileId).Distinct().ToList();
            try
            {
                using var con = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                con.Open();
                var idsParam = string.Join(",", fileIds);
                using var cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT Id, title FROM [bas].[File] WHERE Id IN ({idsParam})";
                using var reader = cmd.ExecuteReader();
                var map = new Dictionary<long, string?>();
                while (reader.Read())
                {
                    var id = reader.GetInt64(0);
                    var t = reader.IsDBNull(1) ? null : reader.GetString(1);
                    map[id] = t;
                }
                foreach (var b in page)
                {
                    if (map.TryGetValue(b.FileId, out var ft))
                    {
                        b.FileTitle = ft;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load file titles for EmployeeDeductionUploadBatch list");
            }
        }

        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }

    /// <summary>
    /// در صورت استفاده نشدن کسورات این دسته در هیچ FicheItem، تمام وابستگی‌ها و خود دسته در یک تراکنش واحد حذف می‌شوند؛ وگرنه پیام خطا برمی‌گردد.
    /// </summary>
    public async Task<OperationResult> TryDeleteBatchAsync(long batchId)
    {
        var ctx = _unitOfWork.Context;

        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            var batch = await ctx.EmployeeDeductionUploadBatches
                .FirstOrDefaultAsync(b => b.Id == batchId && b.OrganisationChartId == _currentUserDefaultOrganId);
            if (batch == null)
                return OperationResult.NotFound();

            var deductionIdsInBatch = await ctx.EmployeeDeductions
                .Where(ed => ed.EmployeeDeductionUploadBatchId == batchId)
                .Select(ed => ed.Id)
                .ToListAsync();

            if (deductionIdsInBatch.Count > 0)
            {
                var usedInFiche = await ctx.FicheItems
                    .AnyAsync(f => f.EmployeeDeductionId != null && deductionIdsInBatch.Contains(f.EmployeeDeductionId.Value));
                if (usedInFiche)
                {
                    return OperationResult.Failed(
                        "این رکورد کسور در محاسبات فیش لحاظ شده است. در صورتی که حقوق پرداخت نشده است فیش را حذف نموده سپس اقدام به حذف فایل آپلود شده بفرمایید.");
                }
            }

            // حذف به ترتیب وابستگی: EmployeeDeductionPayment -> EmployeeDeduction -> TempEmployeeDeduction -> Batch
            var paymentsToRemove = await ctx.EmployeeDeductionPayments
                .Where(p => deductionIdsInBatch.Contains(p.EmployeeDeductionId))
                .ToListAsync();
            if (paymentsToRemove.Count > 0)
            {
                ctx.EmployeeDeductionPayments.RemoveRange(paymentsToRemove);
            }

            var deductionsToRemove = await ctx.EmployeeDeductions
                .Where(ed => ed.EmployeeDeductionUploadBatchId == batchId)
                .ToListAsync();
            if (deductionsToRemove.Count > 0)
            {
                ctx.EmployeeDeductions.RemoveRange(deductionsToRemove);
            }

            var tempsToRemove = await ctx.TempEmployeeDeductions
                .Where(t => t.EmployeeDeductionUploadBatchId == batchId)
                .ToListAsync();
            if (tempsToRemove.Count > 0)
            {
                ctx.TempEmployeeDeductions.RemoveRange(tempsToRemove);
            }

            ctx.EmployeeDeductionUploadBatches.Remove(batch);

            await ctx.SaveChangesAsync();
            await transaction.CommitAsync();
            return OperationResult.Succeeded("دسته آپلود و وابستگی‌های آن حذف شد.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "TryDeleteBatchAsync failed for BatchId={BatchId}", batchId);
            return OperationResult.Failed("خطا در حذف دسته آپلود کسورات: " + ex.Message);
        }
    }
}
