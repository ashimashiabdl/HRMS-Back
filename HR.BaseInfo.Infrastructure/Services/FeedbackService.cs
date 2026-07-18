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

public class FeedbackService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<Feedback, BaseInfoContext, FeedbackDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public async Task<OperationResult> CreateFeedbackAsync(FeedbackDTO dto)
    {
        try
        {
            dto.SubmittedByUserId = userService.GetUserId();
            dto.Status = "جدید";
            dto.CreateDate = DateTime.Now;

            var entity = _mapper.Map<Feedback>(dto);
            entity.IPAddress = userService.GetIP();
            entity.title = dto.FeedbackType ?? "انتقاد و پیشنهاد";
            Add(entity);
            await _unitOfWork.Save();
            return OperationResult.Succeeded(payload: entity.Id);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<Feedback>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        IQueryable<Feedback> dataSource = All(IgnoreExpired);

        if (CustomDataSource != null)
        {
            dataSource = CustomDataSource;
        }

        var result = base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, dataSource, IgnoreDefaultOrganId);

        if (result.Success && result.Payload is List<FeedbackDTO> dtos)
        {
            // Get user IDs
            var userIds = dtos
                .Where(d => d.SubmittedByUserId.HasValue || d.RespondedByUserId.HasValue)
                .SelectMany(d => new[] { d.SubmittedByUserId, d.RespondedByUserId })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
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
                    if (dto.SubmittedByUserId.HasValue && userDict.ContainsKey(dto.SubmittedByUserId.Value))
                    {
                        dto.SubmittedByUserName = userDict[dto.SubmittedByUserId.Value];
                    }
                    if (dto.RespondedByUserId.HasValue && userDict.ContainsKey(dto.RespondedByUserId.Value))
                    {
                        dto.RespondedByUserName = userDict[dto.RespondedByUserId.Value];
                    }
                }
            }
        }

        return result;
    }
}

