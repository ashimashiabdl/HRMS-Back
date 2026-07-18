using System.Text.Json;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.BaseInfo.infrastructure.Services;

public class ImportBatchAdminService(
    IUnitOfWork<BaseInfoContext> unitOfWork,
    ImportHandlerRegistry handlerRegistry) : IScopedServices
{
    public OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string? filter = null,
        string? activeSortColumn = null,
        string? sortDirection = null,
        long? importProfileId = null)
    {
        var query = unitOfWork.Context.ImportBatches
            .AsNoTracking()
            .Include(b => b.ImportProfile)
            .Where(b => !b.IsDeleted);

        if (importProfileId is > 0)
            query = query.Where(b => b.ImportProfileId == importProfileId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(b =>
                (b.title != null && b.title.ToLower().Contains(f)) ||
                (b.UploaderDisplayName != null && b.UploaderDisplayName.ToLower().Contains(f)) ||
                (b.ImportProfile != null && b.ImportProfile.title != null && b.ImportProfile.title.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "id";

        query = orderBy switch
        {
            "title" => desc ? query.OrderByDescending(x => x.title) : query.OrderBy(x => x.title),
            "createdate" => desc ? query.OrderByDescending(x => x.CreateDate) : query.OrderBy(x => x.CreateDate),
            "validcount" => desc ? query.OrderByDescending(x => x.ValidCount) : query.OrderBy(x => x.ValidCount),
            "errorcount" => desc ? query.OrderByDescending(x => x.ErrorCount) : query.OrderBy(x => x.ErrorCount),
            "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => desc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
        };

        var page = query
            .Skip(Math.Max(0, (currentPage - 1) * pageSize))
            .Take(pageSize)
            .Select(b => new ImportBatchListDTO
            {
                Id = b.Id,
                title = b.title,
                CreateDate = b.CreateDate,
                ImportProfileId = b.ImportProfileId,
                ImportProfileTitle = b.ImportProfile != null ? b.ImportProfile.title : null,
                TargetEntityName = b.ImportProfile != null ? b.ImportProfile.TargetEntityName : null,
                FileId = b.FileId,
                Status = b.Status,
                StatusTitle = ImportDisplayHelper.GetBatchStatusTitle(b.Status),
                TotalRowsRead = b.TotalRowsRead,
                ValidCount = b.ValidCount,
                WarningCount = b.WarningCount,
                ErrorCount = b.ErrorCount,
                InsertedCount = b.InsertedCount,
                UploaderDisplayName = b.UploaderDisplayName,
                UploaderUserName = b.UploaderUserName,
                CompletedAt = b.CompletedAt,
                ErrorMessage = b.ErrorMessage
            })
            .ToList();

        EnrichFileTitles(page);
        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }

    public OperationResult GetDetail(long importBatchId)
    {
        var batch = unitOfWork.Context.ImportBatches
            .AsNoTracking()
            .Include(b => b.ImportProfile)
            .FirstOrDefault(b => b.Id == importBatchId && !b.IsDeleted);

        if (batch == null)
            return OperationResult.NotFound();

        var dto = new ImportBatchDetailDTO
        {
            Id = batch.Id,
            title = batch.title,
            CreateDate = batch.CreateDate,
            ImportProfileId = batch.ImportProfileId,
            ImportProfileTitle = batch.ImportProfile?.title,
            TargetEntityName = batch.ImportProfile?.TargetEntityName,
            FileId = batch.FileId,
            Status = batch.Status,
            StatusTitle = ImportDisplayHelper.GetBatchStatusTitle(batch.Status),
            TotalRowsRead = batch.TotalRowsRead,
            ValidCount = batch.ValidCount,
            WarningCount = batch.WarningCount,
            ErrorCount = batch.ErrorCount,
            InsertedCount = batch.InsertedCount,
            UploaderDisplayName = batch.UploaderDisplayName,
            UploaderUserName = batch.UploaderUserName,
            CompletedAt = batch.CompletedAt,
            ErrorMessage = batch.ErrorMessage,
            ContextJson = batch.ContextJson,
            FailedRowsJson = batch.FailedRowsJson
        };

        EnrichFileTitles([dto]);
        return OperationResult.Succeeded(payload: dto);
    }

    public OperationResult GetTempRowsPaged(
        long importBatchId,
        int currentPage = 1,
        int pageSize = 10,
        string? filter = null,
        string? activeSortColumn = null,
        string? sortDirection = null)
    {
        var batch = unitOfWork.Context.ImportBatches
            .AsNoTracking()
            .Include(b => b.ImportProfile!)
            .ThenInclude(p => p.Fields.Where(f => !f.IsDeleted))
            .FirstOrDefault(b => b.Id == importBatchId && !b.IsDeleted);

        var profileFields = batch?.ImportProfile?.Fields
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.ExcelColumnOrder)
            .ToList() ?? [];

        var query = unitOfWork.Context.ImportTempRows
            .AsNoTracking()
            .Where(r => r.ImportBatchId == importBatchId && !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(r =>
                (r.title != null && r.title.ToLower().Contains(f)) ||
                (r.ValidationMessagesJson != null && r.ValidationMessagesJson.ToLower().Contains(f)) ||
                (r.ResolvedDataJson != null && r.ResolvedDataJson.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "rownumber";

        query = orderBy switch
        {
            "title" => desc ? query.OrderByDescending(x => x.title) : query.OrderBy(x => x.title),
            "validationstatus" => desc ? query.OrderByDescending(x => x.ValidationStatus) : query.OrderBy(x => x.ValidationStatus),
            _ => desc ? query.OrderByDescending(x => x.RowNumber) : query.OrderBy(x => x.RowNumber)
        };

        var page = query
            .Skip(Math.Max(0, (currentPage - 1) * pageSize))
            .Take(pageSize)
            .ToList()
            .Select(row => ImportPreviewRowMapper.MapRow(row, profileFields, includeAdminFields: true))
            .ToList();

        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }

    public OperationResult DownloadUploadedFile(long importBatchId)
    {
        var batch = unitOfWork.Context.ImportBatches
            .AsNoTracking()
            .FirstOrDefault(b => b.Id == importBatchId && !b.IsDeleted);

        if (batch == null)
            return OperationResult.NotFound();

        var file = unitOfWork.Context.Files
            .AsNoTracking()
            .FirstOrDefault(f => f.Id == batch.FileId && !f.IsDeleted);

        if (file == null)
            return OperationResult.Failed("فایل آپلودشده یافت نشد.");

        var dto = new ImportFileDownloadDTO
        {
            Id = file.Id,
            FileName = file.title,
            Content = file.Content,
            MimeType = file.MimeType,
            Extension = file.Extension,
            Size = file.Size
        };

        return OperationResult.Succeeded(payload: dto);
    }

    public async Task<OperationResult> DeleteBatchAsync(long importBatchId)
    {
        var batch = await unitOfWork.Context.ImportBatches
            .Include(b => b.ImportProfile)
            .FirstOrDefaultAsync(b => b.Id == importBatchId && !b.IsDeleted);

        if (batch == null)
            return OperationResult.NotFound();

        if (batch.Status == ImportBatchStatus.Finalizing)
            return OperationResult.Failed("دسته در حال نهایی‌سازی است و قابل حذف نیست.");

        if (batch.Status == ImportBatchStatus.Completed && batch.ImportProfile != null)
        {
            var handler = handlerRegistry.GetHandler(batch.ImportProfile);
            if (handler != null)
            {
                var rollback = await handler.RollbackFinalizedBatchAsync(unitOfWork.Context, batch);
                if (rollback is { Success: false })
                    return rollback;
            }
        }

        batch.IsDeleted = true;
        batch.LastModifiedDate = DateTime.Now;

        var tempRows = await unitOfWork.Context.ImportTempRows
            .Where(r => r.ImportBatchId == importBatchId && !r.IsDeleted)
            .ToListAsync();

        foreach (var row in tempRows)
            row.IsDeleted = true;

        await unitOfWork.Context.SaveChangesAsync();
        return OperationResult.Succeeded("دسته Import و ردیف‌های موقت حذف شد.");
    }

    private void EnrichFileTitles(List<ImportBatchListDTO> batches)
    {
        if (batches.Count == 0) return;

        var fileIds = batches.Select(b => b.FileId).Distinct().ToList();
        var files = unitOfWork.Context.Files
            .AsNoTracking()
            .Where(f => fileIds.Contains(f.Id))
            .Select(f => new { f.Id, f.title, f.Extension })
            .ToList();

        foreach (var batch in batches)
        {
            var file = files.FirstOrDefault(f => f.Id == batch.FileId);
            if (file == null) continue;
            batch.FileTitle = file.title;
            batch.FileExtension = file.Extension;
        }
    }
}
