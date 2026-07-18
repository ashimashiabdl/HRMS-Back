using AutoMapper;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using HR.SharedKernel;

using Microsoft.Extensions.Configuration;

namespace HR.Report.Infrastructure.Services;

public class PayLocationProgressReportService(IMapper mapper, IUnitOfWork<ReportContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<PayLocationProgressReport, ReportContext, PayLocationProgressReportDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private class UserNameLookup
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// دریافت داده‌های صفحه‌بندی شده با Include های لازم
    /// </summary>
    public OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? OrganisationChartId = null)
    {
        IQueryable<PayLocationProgressReport> all = All(IgnoreExpired)
            .Include(i => i.OrganisationChart);

        // Filter by OrganisationChartId if provided
        if (OrganisationChartId.HasValue && OrganisationChartId.Value > 0)
        {
            all = all.Where(x => x.OrganisationChartId == OrganisationChartId.Value);
        }

        int rowCount = 0;
        var pagedData = PagerUtility<PayLocationProgressReport>.GetPagedData(all, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection);
        
        // Map to DTO
        var dtos = _mapper.Map<List<PayLocationProgressReportDTO>>(pagedData);
        
        // Populate user names
        if (dtos != null && dtos.Any())
        {
            PopulateUserNames(dtos);
        }
        
        return OperationResult.Succeeded(payload: dtos, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var entity = All(false)
            .Include(i => i.OrganisationChart)
            .FirstOrDefault(i => i.Id == id);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        var dto = _mapper.Map<PayLocationProgressReportDTO>(entity);
        
        // Populate navigation property titles
        if (entity.OrganisationChart != null)
        {
            dto.OrganisationChart = entity.OrganisationChart.title;
        }

        // Populate user name
        if (entity.UploadedByUserId.HasValue)
        {
            PopulateUserName(dto, entity.UploadedByUserId.Value);
        }

        return OperationResult.Succeeded(payload: dto);
    }

    private void PopulateUserNames(List<PayLocationProgressReportDTO> dtos)
    {
        var userIds = dtos
            .Where(d => d.UploadedByUserId.HasValue)
            .Select(d => d.UploadedByUserId.Value)
            .Distinct()
            .ToList();

        if (!userIds.Any())
        {
            return;
        }

        var userIdsString = string.Join(",", userIds);
        var sql = $@"SELECT Id, 
                            ISNULL(FirstName, '') + ' ' + ISNULL(LastName, '') AS FullName
                     FROM [Identity].[AspNetUsers] 
                     WHERE Id IN ({userIdsString})";

        var userNames = dapper.GetAll<UserNameLookup>(sql, new DynamicParameters(), CommandType.Text);
        var userNameLookup = userNames.ToDictionary(x => x.Id, x => x.FullName.Trim());

        foreach (var dto in dtos)
        {
            if (dto.UploadedByUserId.HasValue && userNameLookup.ContainsKey(dto.UploadedByUserId.Value))
            {
                dto.UploadedByUser = userNameLookup[dto.UploadedByUserId.Value];
            }
        }
    }

    private void PopulateUserName(PayLocationProgressReportDTO dto, long userId)
    {
        var sql = @"SELECT Id, 
                           ISNULL(FirstName, '') + ' ' + ISNULL(LastName, '') AS FullName
                    FROM [Identity].[AspNetUsers] 
                    WHERE Id = @UserId";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        var user = dapper.Get<UserNameLookup>(sql, parameters, CommandType.Text);
        if (user != null)
        {
            dto.UploadedByUser = user.FullName.Trim();
        }
    }
}

