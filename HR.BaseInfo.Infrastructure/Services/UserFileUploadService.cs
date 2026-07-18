using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dapper;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class UserFileUploadService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<UserFileUpload, BaseInfoContext, UserFileUploadDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public async Task<OperationResult> CreateFromFileAsync(UserFileUploadDTO dto, long fileId)
    {
        try
        {
            var file = await _unitOfWork.Context.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == fileId);
            if (file == null)
            {
                return OperationResult.Failed("فایل یافت نشد");
            }

            dto.FileId = fileId;
            dto.UploadedByUserId = userService.GetUserId();

            var entity = _mapper.Map<UserFileUpload>(dto);
            entity.IPAddress = userService.GetIP();
            entity.title = file.title;
            entity.File = null;
            Add(entity);
            await _unitOfWork.Save();
            return OperationResult.Succeeded(payload: entity.Id);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public async Task<(byte[]? Content, string? MimeType, string? Title)> GetFileContentAsync(long id)
    {
        var upload = await _unitOfWork.Context.UserFileUploads
            .AsNoTracking()
            .Include(x => x.File)
            .FirstOrDefaultAsync(x => x.FileId == id);

        if (upload == null || upload.File == null || upload.File.Content == null)
        {
            return (null, null, null);
        }

        return (upload.File.Content, upload.File.MimeType, upload.File.title);
    }

    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<UserFileUpload>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        IQueryable<UserFileUpload> dataSource = All(IgnoreExpired)
            .Include(x => x.File);

        if (CustomDataSource != null)
        {
            dataSource = CustomDataSource;
        }

        var result = base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, dataSource, IgnoreDefaultOrganId);

        if (result.Success && result.Payload is List<UserFileUploadDTO> dtos)
        {
            // Get user IDs
            var userIds = dtos.Where(d => d.UploadedByUserId.HasValue)
                .Select(d => d.UploadedByUserId!.Value)
                .Distinct()
                .ToList();

            if (userIds.Any())
            {
                // Get user names from Identity schema using Dapper with parameterized query
                var userIdsParam = string.Join(",", userIds.Select((_, i) => $"@id{i}"));
                var sql = $@"
                    SELECT Id, FirstName, LastName 
                    FROM [Identity].[AspNetUsers] 
                    WHERE Id IN ({userIdsParam})";

                var parameters = new DynamicParameters();
                for (int i = 0; i < userIds.Count; i++)
                {
                    parameters.Add($"@id{i}", userIds[i]);
                }

                using var connection = _dapper.GetDbconnection();
                var users = connection.Query<dynamic>(sql, parameters, commandType: CommandType.Text).ToList();
                var userDict = users.ToDictionary(u => (long)u.Id, u => $"{u.FirstName} {u.LastName}");

                // Map user names to DTOs
                foreach (var dto in dtos)
                {
                    if (dto.UploadedByUserId.HasValue && userDict.ContainsKey(dto.UploadedByUserId.Value))
                    {
                        dto.UploadedByUserName = userDict[dto.UploadedByUserId.Value];
                    }

                    // Set file name and size from File navigation
                    // Note: File is included but we need to get it from the original query
                    // For now, we'll get it from a separate query if needed
                }
            }

            // Get file information
            var fileIds = dtos.Select(d => d.FileId).Distinct().ToList();
            if (fileIds.Any())
            {
                var files = _unitOfWork.Context.Files
                    .AsNoTracking()
                    .Where(f => fileIds.Contains(f.Id))
                    .Select(f => new { f.Id, f.title, f.Size })
                    .ToList();

                var fileDict = files.ToDictionary(f => f.Id, f => new { f.title, f.Size });

                foreach (var dto in dtos)
                {
                    if (fileDict.ContainsKey(dto.FileId))
                    {
                        dto.FileName = fileDict[dto.FileId].title;
                        dto.FileSize = fileDict[dto.FileId].Size;
                    }
                }
            }
        }

        return result;
    }
}

