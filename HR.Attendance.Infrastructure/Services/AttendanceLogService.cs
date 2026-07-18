using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Core.Entities.EmployeeSpecific;
using HR.Attendance.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public class AttendanceLogService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeAttendanceLog, AttendanceContext, AttendanceLogDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    public new OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<EmployeeAttendanceLog>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<EmployeeAttendanceLog> all = All(IgnoreExpired)
            .Include(i => i.Employee)
            .Include(i => i.AttendanceDevice);

        if (_currentUserDefaultOrganId > 0 && !IgnoreDefaultOrganId)
        {
            all = all.Where(i => i.AttendanceDevice != null && i.AttendanceDevice.OrganisationChartId == _currentUserDefaultOrganId);
        }

        if (EmployeeId > 0)
        {
            all = all.Where(i => i.EmployeeId == EmployeeId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<EmployeeAttendanceLog>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<AttendanceLogDTO>>(pagedData);
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Employee)
            .Include(i => i.AttendanceDevice)
            .SingleOrDefault(i => i.Id == id);

        var record = _mapper.Map<AttendanceLogDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(AttendanceLogDTO entityToCreate)
    {
        PrepareLogDto(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(AttendanceLogDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.Set<EmployeeAttendanceLog>()
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == entityToUpdate.Id);

        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        entityToUpdate.EmployeeId = existing.EmployeeId;
        PrepareLogDto(entityToUpdate, keepTitle: true);
        return await base.UpdateForAsync(entityToUpdate);
    }

    private static void PrepareLogDto(AttendanceLogDTO dto, bool keepTitle = false)
    {
        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            dto.title = BuildDefaultTitle(dto);
        }

        if (!dto.StartDate.HasValue)
        {
            dto.StartDate = dto.LogDateTime;
        }

        if (!dto.ReceiveDate.HasValue)
        {
            dto.ReceiveDate = DateTime.Now;
        }
    }

    private static string BuildDefaultTitle(AttendanceLogDTO dto)
    {
        var employeePart = string.IsNullOrWhiteSpace(dto.Employee) ? dto.EmployeeId.ToString() : dto.Employee;
        return $"{employeePart} - {dto.LogDateTime:yyyy-MM-dd HH:mm}";
    }
}
