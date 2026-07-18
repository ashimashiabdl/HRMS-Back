using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;

using Microsoft.Extensions.Configuration;

namespace HR.Identity.infrastructure.Services;

public class UserReportableEntityService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) 
    : BaseService<UserReportableEntity, IdentityContext, UserReportableEntityDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{

    public new OperationResult Get(long id)
    {
        var all = All(false).Include(u => u.User);
        var row = all.SingleOrDefault(i => i.Id == id);
        var record = _mapper.Map<UserReportableEntityDTO>(row);
        
        if (record == null)
        {
            return OperationResult.NotFound();
        }
        
        // Get ReportableEntity using Dapper (Soft FK)
        if (record.ReportableEntityId > 0)
        {
            var sql = "SELECT Id, FriendlyName, TechnicalName FROM rpt.Reportable_Entity WHERE Id = @Id AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", record.ReportableEntityId);
            var reportableEntity = dapper.Get<dynamic>(sql, parameters, CommandType.Text);
            
            if (reportableEntity != null)
            {
                record.ReportableEntity = reportableEntity.FriendlyName ?? reportableEntity.TechnicalName;
            }
        }
        
        return OperationResult.Succeeded(payload: record);
    }

    public new OperationResult GetPagedData(int currentPage = 0, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? UserId = null)
    {
        IQueryable<UserReportableEntity> all = All(IgnoreExpired).Include(u => u.User);
        
        if (UserId.HasValue && UserId.Value > 0)
        {
            all = all.Where(x => x.UserId == UserId.Value);
        }
        
        if (!string.IsNullOrEmpty(filter))
        {
            all = all.Where(x => 
                (x.User != null && (x.User.UserName != null && x.User.UserName.Contains(filter) || 
                 x.User.FirstName != null && x.User.FirstName.Contains(filter) ||
                 x.User.LastName != null && x.User.LastName.Contains(filter))) ||
                x.ReportableEntityId.ToString().Contains(filter));
        }
        
        var rowCount = 0;
        var pagedData = PagerUtility<UserReportableEntity>.GetPagedData(all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);
        var dtoList = _mapper.Map<List<UserReportableEntityDTO>>(pagedData);
        
        // Fill ReportableEntity names using Dapper (Soft FK)
        var reportableEntityIds = dtoList.Where(d => d.ReportableEntityId > 0).Select(d => d.ReportableEntityId).Distinct().ToList();
        if (reportableEntityIds.Any())
        {
            var parameters = new DynamicParameters();
            var idsPlaceholders = new List<string>();
            for (int i = 0; i < reportableEntityIds.Count; i++)
            {
                var paramName = $"@Id{i}";
                parameters.Add(paramName, reportableEntityIds[i]);
                idsPlaceholders.Add(paramName);
            }
            var sql = $"SELECT Id, FriendlyName, TechnicalName FROM rpt.Reportable_Entity WHERE Id IN ({string.Join(",", idsPlaceholders)}) AND IsDeleted = 0";
            var reportableEntities = dapper.GetAll<dynamic>(sql, parameters, CommandType.Text);
            
            var entityLookup = reportableEntities.ToDictionary(e => (long)e.Id, e => e.FriendlyName ?? e.TechnicalName);
            
            foreach (var dto in dtoList)
            {
                if (dto.ReportableEntityId > 0 && entityLookup.ContainsKey(dto.ReportableEntityId))
                {
                    dto.ReportableEntity = entityLookup[dto.ReportableEntityId];
                }
            }
        }
        
        return OperationResult.Succeeded(payload: dtoList, rowCount: rowCount);
    }

    public new OperationResult GetAsKeyValuePair()
    {
        var all = All(true).Include(u => u.User);
        var result = all.Select(x => new SharedKernel.Data.KeyValuePair
        {
            key = x.Id,
            value = (x.User != null ? x.User.UserName : "") + " - " + x.ReportableEntityId.ToString()
        }).ToList();
        
        return OperationResult.Succeeded(payload: result);
    }
}

