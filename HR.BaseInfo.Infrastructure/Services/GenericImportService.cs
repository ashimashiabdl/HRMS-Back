using System.Text.Json;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HR.BaseInfo.infrastructure.Services;

public class GenericImportService : IScopedServices
{
    private readonly IUnitOfWork<BaseInfoContext> _unitOfWork;
    private readonly ImportProfileCrudService _profileService;
    private readonly ImportHandlerRegistry _handlerRegistry;
    private readonly ImportTemplateService _templateService;
    private readonly ImportContextService _contextService;
    private readonly ILogger<GenericImportService> _logger;

    public GenericImportService(
        IUnitOfWork<BaseInfoContext> unitOfWork,
        ImportProfileCrudService profileService,
        ImportHandlerRegistry handlerRegistry,
        ImportTemplateService templateService,
        ImportContextService contextService,
        ILogger<GenericImportService> logger)
    {
        _unitOfWork = unitOfWork;
        _profileService = profileService;
        _handlerRegistry = handlerRegistry;
        _templateService = templateService;
        _contextService = contextService;
        _logger = logger;
    }

    public async Task<OperationResult> ProcessUploadAsync(
        byte[] fileContent,
        string fileExtension,
        long fileId,
        long importProfileId,
        string batchTitle,
        string? uploaderUserName,
        string? uploaderDisplayName,
        string? ipAddress,
        IReadOnlyList<ImportColumnMappingItem>? columnMapping = null,
        string? contextJson = null,
        long organisationChartId = 0,
        ImportContextMode contextMode = ImportContextMode.BatchContext)
    {
        var profile = await _profileService.GetProfileWithFieldsAsync(importProfileId);
        if (profile == null)
            return OperationResult.Failed("پروفایل Import یافت نشد.");

        var handler = _handlerRegistry.GetHandler(profile);
        if (handler == null)
            return OperationResult.Failed(_handlerRegistry.GetMissingHandlerMessage(profile));

        var effectiveMode = ImportEffectiveFieldsHelper.NormalizeMode(contextMode);

        var contextValidation = await _contextService.ValidateAndNormalizeAsync(profile, contextJson, organisationChartId, effectiveMode);
        if (contextValidation is { Success: false })
            return contextValidation;

        var handlerContextValidation = await handler.ValidateUploadContextAsync(profile, contextJson, organisationChartId);
        if (handlerContextValidation is { Success: false })
            return handlerContextValidation;

        var normalizedContextJson = _contextService.BuildNormalizedContextJson(contextJson, organisationChartId, profile);

        var fields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, effectiveMode);
        if (fields.Count == 0)
            return OperationResult.Failed("ستون‌های پروفایل Import تعریف نشده است.");

        var columnOrderByProperty = BuildColumnOrderMap(fields, columnMapping);

        List<ParsedExcelRow> parsedRows;
        try
        {
            parsedRows = ExcelImportParser.Parse(fileContent, fileExtension, profile.HasHeaderRow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Import parse failed for profile {ProfileId}", importProfileId);
            return OperationResult.Failed($"خطا در خواندن فایل: {ex.Message}");
        }

        if (parsedRows.Count == 0)
            return OperationResult.Failed("فایل فاقد داده است.");

        if (parsedRows.Count > profile.MaxRowCount)
            return OperationResult.Failed($"تعداد ردیف‌ها ({parsedRows.Count}) بیش از حد مجاز ({profile.MaxRowCount}) است.");

        var normalizedTitle = string.IsNullOrWhiteSpace(batchTitle)
            ? $"Import {profile.title} - {DateTime.Now:yyyy/MM/dd HH:mm}"
            : batchTitle.Trim();

        var batch = new ImportBatch
        {
            title = normalizedTitle,
            ImportProfileId = profile.Id,
            FileId = fileId,
            ContextJson = normalizedContextJson,
            ContextMode = effectiveMode,
            Status = ImportBatchStatus.Parsed,
            CreateDate = DateTime.Now,
            IsDeleted = false,
            IPAddress = ipAddress,
            UploaderUserName = uploaderUserName,
            UploaderDisplayName = uploaderDisplayName
        };

        var ctx = _unitOfWork.Context;
        ctx.ImportBatches.Add(batch);
        await ctx.SaveChangesAsync();

        var tempRows = new List<ImportTempRow>();
        var failedRows = new List<ImportFailedRowDTO>();

        foreach (var parsedRow in parsedRows)
        {
            var rawDict = parsedRow.Cells.ToDictionary(k => k.Key.ToString(), v => v.Value);
            var parsedDict = new Dictionary<string, string?>();
            var rowMessages = new List<string>();

            foreach (var field in fields)
            {
                var columnOrder = columnOrderByProperty.GetValueOrDefault(field.TargetPropertyName, field.ExcelColumnOrder);
                parsedRow.Cells.TryGetValue(columnOrder, out var cellValue);
                var normalized = ExcelImportParser.NormalizeText(cellValue);
                parsedDict[field.TargetPropertyName] = normalized;

                if (field.IsRequired && string.IsNullOrWhiteSpace(normalized))
                    rowMessages.Add($"ستون '{field.ExcelColumnHeader ?? field.TargetPropertyName}' الزامی است.");
            }

            var temp = new ImportTempRow
            {
                title = parsedDict.GetValueOrDefault("title") ?? $"ردیف {parsedRow.RowNumber}",
                ImportBatchId = batch.Id,
                RowNumber = parsedRow.RowNumber,
                RawDataJson = JsonSerializer.Serialize(rawDict),
                ParsedDataJson = JsonSerializer.Serialize(parsedDict),
                CreateDate = DateTime.Now,
                IsDeleted = false,
                ValidationStatus = rowMessages.Count > 0 ? ImportValidationStatus.Error : ImportValidationStatus.Valid,
                ValidationMessagesJson = rowMessages.Count > 0 ? JsonSerializer.Serialize(rowMessages) : null
            };
            tempRows.Add(temp);

            if (rowMessages.Count > 0)
            {
                failedRows.Add(new ImportFailedRowDTO
                {
                    ExcelRowNumber = parsedRow.RowNumber,
                    ErrorMessage = string.Join(" ", rowMessages),
                    RawPreview = string.Join(" | ", parsedRow.Cells.OrderBy(c => c.Key).Select(c => c.Value))
                });
            }
        }

        ctx.ImportTempRows.AddRange(tempRows);
        await ctx.SaveChangesAsync();

        var rowsForHandler = tempRows.Where(r => r.ValidationStatus != ImportValidationStatus.Error).ToList();
        if (rowsForHandler.Count > 0)
            await handler.ValidateAndResolveRowsAsync(ctx, profile, rowsForHandler);

        foreach (var row in tempRows.Where(r => r.ValidationStatus == ImportValidationStatus.Error && string.IsNullOrEmpty(r.ValidationMessagesJson) == false))
        {
            if (failedRows.Any(f => f.ExcelRowNumber == row.RowNumber))
                continue;

            var msgs = DeserializeMessages(row.ValidationMessagesJson);
            failedRows.Add(new ImportFailedRowDTO
            {
                ExcelRowNumber = row.RowNumber,
                ErrorMessage = string.Join(" ", msgs),
                RawPreview = row.title
            });
        }

        foreach (var row in tempRows.Where(r => r.ValidationStatus == ImportValidationStatus.Error))
        {
            var msgs = DeserializeMessages(row.ValidationMessagesJson);
            if (msgs.Count == 0)
                continue;
            if (!failedRows.Any(f => f.ExcelRowNumber == row.RowNumber))
            {
                failedRows.Add(new ImportFailedRowDTO
                {
                    ExcelRowNumber = row.RowNumber,
                    ErrorMessage = string.Join(" ", msgs),
                    RawPreview = row.title
                });
            }
        }

        batch.TotalRowsRead = tempRows.Count;
        batch.ValidCount = tempRows.Count(r => r.ValidationStatus == ImportValidationStatus.Valid);
        batch.WarningCount = tempRows.Count(r => r.ValidationStatus == ImportValidationStatus.Warning);
        batch.ErrorCount = tempRows.Count(r => r.ValidationStatus == ImportValidationStatus.Error);
        batch.Status = ImportBatchStatus.PreviewReady;
        batch.FailedRowsJson = failedRows.Count > 0
            ? JsonSerializer.Serialize(failedRows.Take(500).ToList())
            : null;

        await ctx.SaveChangesAsync();

        var result = new ImportUploadResultDTO
        {
            ImportBatchId = batch.Id,
            FileId = fileId,
            TotalRowsRead = batch.TotalRowsRead,
            ValidCount = batch.ValidCount,
            WarningCount = batch.WarningCount,
            ErrorCount = batch.ErrorCount,
            FailedRows = failedRows.Take(100).ToList()
        };

        return OperationResult.Succeeded(payload: result);
    }

    public OperationResult GetPreviewPaged(
        long importBatchId,
        int currentPage,
        int pageSize,
        string? filter,
        string? activeSortColumn,
        string? sortDirection)
    {
        var batch = _unitOfWork.Context.ImportBatches
            .AsNoTracking()
            .Include(b => b.ImportProfile!)
            .ThenInclude(p => p.Fields.Where(f => !f.IsDeleted))
            .Include(b => b.ImportProfile!)
            .ThenInclude(p => p.ContextFields.Where(f => !f.IsDeleted))
            .FirstOrDefault(b => b.Id == importBatchId && !b.IsDeleted);

        var profileFields = batch?.ImportProfile == null
            ? []
            : ImportEffectiveFieldsHelper.GetEffectiveRowFields(batch.ImportProfile, batch.ContextMode);

        var query = _unitOfWork.Context.ImportTempRows
            .AsNoTracking()
            .Where(r => r.ImportBatchId == importBatchId && !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(r =>
                (r.title != null && r.title.ToLower().Contains(f)) ||
                (r.ResolvedDataJson != null && r.ResolvedDataJson.ToLower().Contains(f)) ||
                (r.ValidationMessagesJson != null && r.ValidationMessagesJson.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "rownumber";

        IQueryable<ImportTempRow> ordered = orderBy switch
        {
            "title" => desc ? query.OrderByDescending(x => x.title) : query.OrderBy(x => x.title),
            "validationstatus" => desc ? query.OrderByDescending(x => x.ValidationStatus) : query.OrderBy(x => x.ValidationStatus),
            _ => desc ? query.OrderByDescending(x => x.RowNumber) : query.OrderBy(x => x.RowNumber)
        };

        var page = ordered
            .Skip(Math.Max(0, (currentPage - 1) * pageSize))
            .Take(pageSize)
            .ToList()
            .Select(row => ImportPreviewRowMapper.MapRow(row, profileFields))
            .ToList();

        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }

    public OperationResult DetectColumns(byte[] fileContent, string fileExtension, long importProfileId, ImportContextMode contextMode = ImportContextMode.BatchContext)
    {
        var profile = _unitOfWork.Context.ImportProfiles
            .AsNoTracking()
            .Include(p => p.Fields.Where(f => !f.IsDeleted))
            .Include(p => p.ContextFields.Where(f => !f.IsDeleted))
            .FirstOrDefault(p => p.Id == importProfileId && !p.IsDeleted);

        if (profile == null)
            return OperationResult.NotFound();

        if (!profile.HasHeaderRow)
            return OperationResult.Failed("این پروفایل فاقد ردیف Header است؛ نگاشت ستون خودکار فقط برای فایل با Header پشتیبانی می‌شود.");

        List<ExcelHeaderCell> headers;
        try
        {
            headers = ExcelImportParser.ReadHeaders(fileContent, fileExtension);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در خواندن Header فایل: {ex.Message}");
        }

        var effectiveFields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, contextMode);
        var fieldTuples = effectiveFields
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.ExcelColumnOrder)
            .Select(f => (f.Id, f.TargetPropertyName, f.ExcelColumnHeader, f.ExcelColumnOrder, f.IsRequired));

        var suggestions = ImportColumnMapper.SuggestMappings(headers, fieldTuples);
        var result = new ImportDetectColumnsResult
        {
            FileHeaders = headers,
            Suggestions = suggestions,
            IsFullyMapped = ImportColumnMapper.IsFullyMapped(suggestions)
        };

        return OperationResult.Succeeded(payload: result);
    }

    public byte[]? GenerateTemplate(long importProfileId, ImportContextMode contextMode = ImportContextMode.BatchContext)
    {
        var profile = _unitOfWork.Context.ImportProfiles
            .AsNoTracking()
            .Include(p => p.Fields.Where(f => !f.IsDeleted))
            .Include(p => p.ContextFields.Where(f => !f.IsDeleted))
            .FirstOrDefault(p => p.Id == importProfileId && !p.IsDeleted);

        return profile == null ? null : _templateService.GenerateTemplate(profile, contextMode);
    }

    public OperationResult GetUploadReport(long importBatchId)
    {
        var batch = _unitOfWork.Context.ImportBatches
            .AsNoTracking()
            .FirstOrDefault(b => b.Id == importBatchId && !b.IsDeleted);
        if (batch == null)
            return OperationResult.NotFound();

        var failedRows = new List<ImportFailedRowDTO>();
        if (!string.IsNullOrEmpty(batch.FailedRowsJson))
        {
            try
            {
                failedRows = JsonSerializer.Deserialize<List<ImportFailedRowDTO>>(batch.FailedRowsJson) ?? failedRows;
            }
            catch { /* ignore */ }
        }

        var dto = new ImportUploadResultDTO
        {
            ImportBatchId = batch.Id,
            FileId = batch.FileId,
            TotalRowsRead = batch.TotalRowsRead,
            ValidCount = batch.ValidCount,
            WarningCount = batch.WarningCount,
            ErrorCount = batch.ErrorCount,
            FailedRows = failedRows
        };

        return OperationResult.Succeeded(payload: dto);
    }

    public async Task<OperationResult> FinalizeAsync(long importBatchId, string? ipAddress)
    {
        var batch = await _unitOfWork.Context.ImportBatches
            .Include(b => b.ImportProfile!)
            .ThenInclude(p => p.Fields.Where(f => !f.IsDeleted))
            .Include(b => b.ImportProfile!)
            .ThenInclude(p => p.ContextFields.Where(f => !f.IsDeleted))
            .FirstOrDefaultAsync(b => b.Id == importBatchId && !b.IsDeleted);

        if (batch == null)
            return OperationResult.NotFound();

        if (batch.Status == ImportBatchStatus.Completed)
            return OperationResult.Failed("این دسته قبلاً نهایی‌سازی شده است.");

        if (batch.ValidCount <= 0)
            return OperationResult.Failed("ردیف معتبری برای درج وجود ندارد.");

        var handler = batch.ImportProfile != null
            ? _handlerRegistry.GetHandler(batch.ImportProfile)
            : null;
        if (handler == null && batch.ImportProfile != null)
            return OperationResult.Failed(_handlerRegistry.GetMissingHandlerMessage(batch.ImportProfile));

        if (handler == null)
            return OperationResult.Failed("Handler نهایی‌سازی یافت نشد.");

        batch.Status = ImportBatchStatus.Finalizing;
        await _unitOfWork.Context.SaveChangesAsync();

        try
        {
            var inserted = await handler.FinalizeAsync(_unitOfWork.Context, batch, ipAddress);
            batch.InsertedCount = inserted;
            batch.Status = ImportBatchStatus.Completed;
            batch.CompletedAt = DateTime.Now;
            await _unitOfWork.Context.SaveChangesAsync();

            var skipped = batch.ValidCount - inserted;
            return OperationResult.Succeeded(payload: new ImportFinalizeResultDTO
            {
                ImportBatchId = batch.Id,
                InsertedCount = inserted,
                SkippedCount = Math.Max(0, skipped)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import finalize failed for batch {BatchId}", importBatchId);
            batch.Status = ImportBatchStatus.Failed;
            batch.ErrorMessage = ex.Message;
            await _unitOfWork.Context.SaveChangesAsync();
            return OperationResult.Failed($"خطا در نهایی‌سازی: {ex.Message}");
        }
    }

    public List<string> GetAllowedExtensions(long importProfileId)
    {
        var profile = _unitOfWork.Context.ImportProfiles
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == importProfileId && !p.IsDeleted);
        if (profile == null || string.IsNullOrWhiteSpace(profile.AllowedExtensions))
            return [".xlsx", ".csv"];

        return profile.AllowedExtensions
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(e => e.StartsWith('.') ? e.ToLowerInvariant() : $".{e.ToLowerInvariant()}")
            .Distinct()
            .ToList();
    }

    private static Dictionary<string, int> BuildColumnOrderMap(
        List<ImportProfileField> fields,
        IReadOnlyList<ImportColumnMappingItem>? columnMapping)
    {
        var map = fields.ToDictionary(
            f => f.TargetPropertyName,
            f => f.ExcelColumnOrder,
            StringComparer.OrdinalIgnoreCase);

        if (columnMapping == null || columnMapping.Count == 0)
            return map;

        foreach (var item in columnMapping)
        {
            if (string.IsNullOrWhiteSpace(item.TargetPropertyName) || item.ExcelColumnOrder <= 0)
                continue;
            map[item.TargetPropertyName] = item.ExcelColumnOrder;
        }

        return map;
    }

    private static List<string> DeserializeMessages(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch
        {
            return [json];
        }
    }
}
