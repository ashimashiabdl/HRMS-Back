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

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class UserIssueReportService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<UserIssueReport, BaseInfoContext, UserIssueReportDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new async Task<OperationResult> CreateForAsync(UserIssueReportDTO entityToCreate)
    {
        var userId = userService.GetUserId();
        if (userId > 0)
        {
            entityToCreate.CreatedByUserId = userId;
        }
        return await base.CreateForAsync(entityToCreate);
    }

    public async Task<OperationResult> CreateFromTempFileAsync(UserIssueReportDTO dto, long tempFileId)
    {
        try
        {
            var temp = await _unitOfWork.Context.TempGlobalFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == tempFileId);
            if (temp == null)
            {
                return OperationResult.Failed("فایل موقت یافت نشد");
            }

            var finalFile = new Core.Entities.File
            {
                title = HR.SharedKernel.Share.Helper.SanitizeFileName(temp.title),
                Extension = temp.Extension,
                MimeType = temp.MimeType,
                Size = temp.Size,
                Content = temp.Content,
                CreateDate = DateTime.Now,
                IPAddress = userService.GetIP(),
                IsDeleted = false
            };
            _unitOfWork.Context.Files.Add(finalFile);
            await _unitOfWork.Context.SaveChangesAsync();

            dto.FileId = finalFile.Id;
            var userId = userService.GetUserId();
            if (userId > 0)
            {
                dto.CreatedByUserId = userId;
            }
            var entity = _mapper.Map<UserIssueReport>(dto);
            Add(entity);
            await _unitOfWork.Save();
            return OperationResult.Succeeded(payload: entity.Id);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public async Task<OperationResult> AddResponseAsync(long reportId, string response)
    {
        try
        {
            var report = await _unitOfWork.Context.UserIssueReports.FirstOrDefaultAsync(x => x.Id == reportId);
            if (report == null)
            {
                return OperationResult.Failed("گزارش یافت نشد");
            }

            var userId = userService.GetUserId();
            if (userId <= 0)
            {
                return OperationResult.Failed("کاربر مشخص نشده است");
            }

            report.Response = response;
            report.ResponseByUserId = userId;
            report.IsSubmitted = true;
            report.ResponseDate = DateTime.Now;
            report.LastModifiedDate = DateTime.Now;
            report.IPAddress = userService.GetIP();

            await _unitOfWork.Save();
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    private void PopulateUserNames(List<UserIssueReportDTO> dtos)
    {
        var userIds = dtos
            .Where(d => d.CreatedByUserId.HasValue || d.ResponseByUserId.HasValue)
            .SelectMany(d => new[] { d.CreatedByUserId, d.ResponseByUserId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (!userIds.Any()) return;

        var userIdsString = string.Join(",", userIds);
        var sql = $@"
            SELECT Id, FirstName, LastName 
            FROM [Identity].[AspNetUsers] 
            WHERE Id IN ({userIdsString})";

        var users = dapper.GetAll<(long Id, string FirstName, string LastName)>(sql, null, CommandType.Text);

        foreach (var dto in dtos)
        {
            if (dto.CreatedByUserId.HasValue)
            {
                var creator = users.FirstOrDefault(u => u.Id == dto.CreatedByUserId.Value);
                if (creator.Id > 0)
                {
                    dto.CreatedByUserFullName = $"{creator.FirstName} {creator.LastName}".Trim();
                }
            }

            if (dto.ResponseByUserId.HasValue)
            {
                var responder = users.FirstOrDefault(u => u.Id == dto.ResponseByUserId.Value);
                if (responder.Id > 0)
                {
                    dto.ResponseByUserFullName = $"{responder.FirstName} {responder.LastName}".Trim();
                }
            }
        }
    }

    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<UserIssueReport>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        var result = base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, CustomDataSource, IgnoreDefaultOrganId);
        
        if (result.Success && result.Payload is List<UserIssueReportDTO> dtos)
        {
            PopulateUserNames(dtos);
        }

        return result;
    }

    public new OperationResult Get(long id)
    {
        var result = base.Get(id);
        
        if (result.Success && result.Payload is UserIssueReportDTO dto)
        {
            PopulateUserNames(new List<UserIssueReportDTO> { dto });
        }

        return result;
    }

    public async Task<(byte[]? Content, string? MimeType, string? Title)> GetAttachmentAsync(long reportId)
    {
        var report = await _unitOfWork.Context.UserIssueReports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == reportId);
        if (report == null || report.FileId == null)
        {
            return (null, null, null);
        }
        var file = await _unitOfWork.Context.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Id == report.FileId.Value);
        if (file == null || file.Content == null)
        {
            return (null, null, null);
        }
        return (file.Content, file.MimeType, file.title);
    }
}


