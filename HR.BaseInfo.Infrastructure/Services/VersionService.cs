using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using VersionEntity = HR.BaseInfo.Core.Entities.Version;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class VersionService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<VersionEntity, BaseInfoContext, VersionDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new async Task<OperationResult> CreateForAsync(VersionDTO dto)
    {
        // Start transaction
        _unitOfWork.CreateTransaction();
        try
        {
            if (dto.CreatedAt == default)
            {
                dto.CreatedAt = DateTime.Now;
            }
            // Map VersionNumber to title for BaseEntity compatibility
            if (string.IsNullOrEmpty(dto.title) && !string.IsNullOrEmpty(dto.VersionNumber))
            {
                dto.title = dto.VersionNumber;
            }
            
            // Save change logs separately after version is created
            var changeLogs = dto.ChangeLogs;
            dto.ChangeLogs = null; // Don't include in version creation
            
            // Create version manually within transaction
            var mappedVersion = _mapper.Map<VersionEntity>(dto);
            if (string.IsNullOrEmpty(mappedVersion.title))
            {
                mappedVersion.title = "";
            }
            
            // Validate before adding
            if (!Validate(mappedVersion))
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("اطلاعات نسخه معتبر نیست");
            }
            
            Add(mappedVersion);
            var saveResult = await _unitOfWork.Save();
            
            if (saveResult > 0)
            {
                long versionId = mappedVersion.Id;
                
                // Save change logs within the same transaction
                if (changeLogs != null && changeLogs.Any())
                {
                    await SaveChangeLogsAsync(versionId, changeLogs, false); // false = don't call Save, we'll do it in transaction
                    // Save change logs changes
                    await _unitOfWork.Save();
                }
                
                // Commit transaction
                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: versionId);
            }
            else
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در ایجاد نسخه");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در ایجاد نسخه: {ex.Message}");
        }
    }

    public new async Task<OperationResult> UpdateForAsync(VersionDTO dto)
    {
        // Start transaction
        _unitOfWork.CreateTransaction();
        try
        {
            // Map VersionNumber to title for BaseEntity compatibility
            if (string.IsNullOrEmpty(dto.title) && !string.IsNullOrEmpty(dto.VersionNumber))
            {
                dto.title = dto.VersionNumber;
            }
            
            // Save change logs separately
            var changeLogs = dto.ChangeLogs;
            dto.ChangeLogs = null; // Don't include in version update
            
            var versionId = dto.Id ?? 0;
            if (versionId <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("شناسه نسخه معتبر نیست");
            }
            
            // Update version manually within transaction
            var existingVersion = _unitOfWork.Context.Versions.FirstOrDefault(v => v.Id == versionId);
            if (existingVersion == null)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("نسخه یافت نشد");
            }
            
            // Map DTO to entity
            _mapper.Map(dto, existingVersion);
            
            // Validate before updating
            if (!Validate(existingVersion))
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("اطلاعات نسخه معتبر نیست");
            }
            
            Update(existingVersion);
            var saveResult = await _unitOfWork.Save();
            
            if (saveResult > 0)
            {
                // Save change logs within the same transaction
                if (changeLogs != null)
                {
                    await SaveChangeLogsAsync(versionId, changeLogs, false); // false = don't call Save, we'll do it in transaction
                    // Save change logs changes
                    await _unitOfWork.Save();
                }
                
                // Commit transaction
                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: versionId);
            }
            else
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در بروزرسانی نسخه");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در بروزرسانی نسخه: {ex.Message}");
        }
    }

    private async Task SaveChangeLogsAsync(long versionId, List<VersionChangeLogDTO> changeLogs, bool saveChanges = true)
    {
        if (changeLogs == null || !changeLogs.Any())
        {
            return;
        }

        // Get existing change logs for this version
        var existingLogs = _unitOfWork.Context.VersionChangeLogs
            .Where(x => x.VersionId == versionId && !x.IsDeleted)
            .ToList();

        var existingLogIds = existingLogs.Select(x => x.Id).ToList();
        var currentLogIds = changeLogs.Where(x => x.Id.HasValue && x.Id.Value > 0).Select(x => x.Id.Value).ToList();

        // Delete change logs that were removed
        var toDelete = existingLogIds.Where(id => !currentLogIds.Contains(id)).ToList();
        foreach (var id in toDelete)
        {
            var logToDelete = existingLogs.FirstOrDefault(x => x.Id == id);
            if (logToDelete != null)
            {
                _unitOfWork.Context.VersionChangeLogs.Remove(logToDelete);
            }
        }

        // Get IP address for audit fields
        string ipAddress = "Local";
        try
        {
            ipAddress = userService.GetIP();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "Notfound")
            {
                ipAddress = "Local";
            }
        }
        catch
        {
            ipAddress = "Local";
        }

        // Save or update change logs
        foreach (var logDto in changeLogs)
        {
            if (logDto.Id.HasValue && logDto.Id.Value > 0)
            {
                // Update existing
                var existingLog = existingLogs.FirstOrDefault(x => x.Id == logDto.Id.Value);
                if (existingLog != null)
                {
                    existingLog.ChangeType = logDto.ChangeType;
                    existingLog.Description = logDto.Description;
                    existingLog.LastModifiedDate = DateTime.Now;
                    existingLog.IPAddress = ipAddress;
                    _unitOfWork.Context.VersionChangeLogs.Update(existingLog);
                }
            }
            else
            {
                // Create new
                var newLog = _mapper.Map<VersionChangeLog>(logDto);
                newLog.VersionId = versionId;
                newLog.CreateDate = DateTime.Now;
                newLog.LastModifiedDate = DateTime.Now;
                newLog.IPAddress = ipAddress;
                newLog.IsDeleted = false;
                // Set title (required by BaseEntity) - use a combination of ChangeType and Description
                if (string.IsNullOrEmpty(newLog.title))
                {
                    var titlePrefix = newLog.ChangeType ?? "Change";
                    var descriptionPreview = newLog.Description?.Length > 50 
                        ? newLog.Description.Substring(0, 50) + "..." 
                        : newLog.Description ?? "";
                    newLog.title = $"{titlePrefix}: {descriptionPreview}".Trim();
                    // Ensure title doesn't exceed 256 characters (BaseEntity limit)
                    if (newLog.title.Length > 256)
                    {
                        newLog.title = newLog.title.Substring(0, 256);
                    }
                }
                _unitOfWork.Context.VersionChangeLogs.Add(newLog);
            }
        }

        // Only save if explicitly requested (when not in transaction)
        if (saveChanges)
        {
            await _unitOfWork.Save();
        }
    }

    public OperationResult GetLatestReleasedVersion()
    {
        var latestVersion = _unitOfWork.Context.Versions
            .Include(v => v.ChangeLogs)
            .Where(v => v.Status == "Released")
            .OrderByDescending(v => v.ReleaseDate)
            .ThenByDescending(v => v.CreatedAt)
            .FirstOrDefault();

        if (latestVersion == null)
        {
            return OperationResult.Failed("نسخه منتشر شده‌ای یافت نشد");
        }

        var dto = _mapper.Map<VersionDTO>(latestVersion);
        return OperationResult.Succeeded(payload: dto);
    }

    public OperationResult GetVersionWithChangeLog(long id)
    {
        var version = _unitOfWork.Context.Versions
            .Include(v => v.ChangeLogs)
            .FirstOrDefault(v => v.Id == id);

        if (version == null)
        {
            return OperationResult.Failed("نسخه یافت نشد");
        }

        var dto = _mapper.Map<VersionDTO>(version);
        return OperationResult.Succeeded(payload: dto);
    }

    public OperationResult GetFilteredVersions(string? status = null, string? releaseType = null, string? environment = null)
    {
        var query = _unitOfWork.Context.Versions
            .Include(v => v.ChangeLogs)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(v => v.Status == status);
        }

        if (!string.IsNullOrEmpty(releaseType))
        {
            query = query.Where(v => v.ReleaseType == releaseType);
        }

        if (!string.IsNullOrEmpty(environment))
        {
            query = query.Where(v => v.Environment == environment);
        }

        var versions = query
            .OrderByDescending(v => v.ReleaseDate)
            .ThenByDescending(v => v.CreatedAt)
            .ToList();

        var dtos = _mapper.Map<List<VersionDTO>>(versions);
        return OperationResult.Succeeded(payload: dtos);
    }

    public OperationResult GetCurrentSystemVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        var buildDate = System.IO.File.GetLastWriteTime(assembly.Location);

        var result = new
        {
            BackendVersion = version,
            BuildDate = buildDate
        };

        return OperationResult.Succeeded(payload: result);
    }

    public bool Validate(VersionEntity entity, object etc = null)
    {
        // Validate semantic version format (e.g., 2.3.1)
        if (!System.Text.RegularExpressions.Regex.IsMatch(entity.VersionNumber, @"^\d+\.\d+\.\d+"))
        {
            return false;
        }

        // Validate ReleaseType
        var validReleaseTypes = new[] { "Major", "Minor", "Patch", "Hotfix" };
        if (!validReleaseTypes.Contains(entity.ReleaseType))
        {
            return false;
        }

        // Validate Status
        var validStatuses = new[] { "Draft", "Released", "Deprecated" };
        if (!validStatuses.Contains(entity.Status))
        {
            return false;
        }

        // Validate Environment
        var validEnvironments = new[] { "Production", "Staging", "Demo" };
        if (!validEnvironments.Contains(entity.Environment))
        {
            return false;
        }

        return true;
    }
}
