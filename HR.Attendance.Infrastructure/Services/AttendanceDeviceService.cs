using AutoMapper;
using Dapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

internal class BaseTableValueTitle
{
    public long Id { get; set; }
    public string title { get; set; } = string.Empty;
}

public class AttendanceDeviceService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<AttendanceDevice, AttendanceContext, AttendanceDeviceDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    public OperationResult GetAsKeyValuePair(long currentUserDefaultOrganId)
    {
        return OperationResult.Succeeded(payload: All()
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId)
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = (string.IsNullOrWhiteSpace(i.Code) ? i.title : i.Code + " - " + i.title)
            }));
    }

    public new OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<AttendanceDevice>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<AttendanceDevice> all = All(IgnoreExpired)
            .Include(i => i.AttendanceLocation);

        if (_currentUserDefaultOrganId > 0 && !IgnoreDefaultOrganId)
        {
            all = all.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<AttendanceDevice>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<AttendanceDeviceDTO>>(pagedData);
        EnrichBaseTableTitles(result);

        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.AttendanceLocation)
            .SingleOrDefault(i => i.Id == id);

        var record = _mapper.Map<AttendanceDeviceDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        EnrichBaseTableTitles([record]);
        return OperationResult.Succeeded(payload: record);
    }

    private void EnrichBaseTableTitles(IEnumerable<AttendanceDeviceDTO> records)
    {
        var list = records.ToList();
        var allIds = list
            .SelectMany(r => new long?[] { r.BrandId, r.StatusId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (allIds.Count == 0)
        {
            return;
        }

        var sql = $"SELECT Id, title FROM bas.Base_Table_Value WHERE Id IN ({string.Join(",", allIds)}) AND IsDeleted = 0";
        var titles = dapper.GetAll<BaseTableValueTitle>(sql, new DynamicParameters(), CommandType.Text);
        var titleLookup = titles.ToDictionary(x => x.Id, x => x.title);

        foreach (var dto in list)
        {
            if (dto.BrandId.HasValue && titleLookup.TryGetValue(dto.BrandId.Value, out var brandTitle))
            {
                dto.Brand = brandTitle;
            }

            if (dto.StatusId.HasValue && titleLookup.TryGetValue(dto.StatusId.Value, out var statusTitle))
            {
                dto.Status = statusTitle;
            }
        }
    }
}
