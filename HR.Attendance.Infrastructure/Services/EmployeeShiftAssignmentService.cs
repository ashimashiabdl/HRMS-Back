using AutoMapper;
using HR.Attendance.Core.DTOs;
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

public class EmployeeShiftAssignmentService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeShiftAssignment, AttendanceContext, EmployeeShiftAssignmentDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
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
        IQueryable<EmployeeShiftAssignment>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<EmployeeShiftAssignment> all = All(IgnoreExpired)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.Employee)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart);

        if (EmployeeId > 0)
        {
            all = all.Where(i => i.EmployeeId == EmployeeId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<EmployeeShiftAssignment>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<EmployeeShiftAssignmentDTO>>(pagedData);
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Employee)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart)
            .SingleOrDefault(i => i.Id == id && i.OrganisationChartId == _currentUserDefaultOrganId);

        var record = _mapper.Map<EmployeeShiftAssignmentDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeShiftAssignmentDTO entityToCreate)
    {
        var validation = await ValidateAssignmentAsync(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareAssignmentDto(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(EmployeeShiftAssignmentDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.EmployeeShiftAssignments
            .AsNoTracking()
            .SingleOrDefaultAsync(i =>
                i.Id == entityToUpdate.Id
                && i.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted);

        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        entityToUpdate.EmployeeId = existing.EmployeeId;

        var validation = await ValidateAssignmentAsync(entityToUpdate);
        if (!validation.Success)
        {
            return validation;
        }

        entityToUpdate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareAssignmentDto(entityToUpdate, keepTitle: true);
        return await base.UpdateForAsync(entityToUpdate);
    }

    private async Task<OperationResult> ValidateAssignmentAsync(EmployeeShiftAssignmentDTO dto)
    {
        if (dto.EmployeeId <= 0)
        {
            return OperationResult.Failed("کارمند الزامی است");
        }

        if (dto.ShiftId <= 0)
        {
            return OperationResult.Failed("شیفت الزامی است");
        }

        if (!dto.StartDate.HasValue)
        {
            return OperationResult.Failed("تاریخ شروع اعتبار الزامی است");
        }

        if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.StartDate.Value.Date)
        {
            return OperationResult.Failed("تاریخ پایان اعتبار باید بعد از تاریخ شروع باشد");
        }

        var shiftExists = await _unitOfWork.Context.Shifts
            .AnyAsync(i => i.Id == dto.ShiftId
                && i.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted
                && i.IsActive);

        if (!shiftExists)
        {
            return OperationResult.Failed("شیفت انتخاب‌شده معتبر نیست یا غیرفعال است");
        }

        return OperationResult.Succeeded();
    }

    private static void PrepareAssignmentDto(EmployeeShiftAssignmentDTO dto, bool keepTitle = false)
    {
        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            var employeePart = string.IsNullOrWhiteSpace(dto.Employee) ? dto.EmployeeId.ToString() : dto.Employee;
            var shiftPart = string.IsNullOrWhiteSpace(dto.Shift) ? dto.ShiftId.ToString() : dto.Shift;
            dto.title = $"{employeePart} - {shiftPart}";
        }
    }
}
