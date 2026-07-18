using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities.EmployeeSpecific;
using HR.Attendance.Core.Enums;
using HR.Attendance.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public class EmployeeExceptionJustificationRequestService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeExceptionJustificationRequest, AttendanceContext, EmployeeExceptionJustificationRequestDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
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
        IQueryable<EmployeeExceptionJustificationRequest>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<EmployeeExceptionJustificationRequest> all = All(IgnoreExpired)
            .Where(i => i.EmployeeAttendanceException != null
                && i.EmployeeAttendanceException.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.EmployeeAttendanceException)
                .ThenInclude(e => e!.Employee)
            .Include(i => i.AbsenceType)
            .Include(i => i.LeaveType)
            .Include(i => i.EmployeeExceptionJustificationRequestState);

        if (EmployeeId > 0)
        {
            all = all.Where(i => i.EmployeeAttendanceException!.EmployeeId == EmployeeId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<EmployeeExceptionJustificationRequest>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<EmployeeExceptionJustificationRequestDTO>>(pagedData);
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.EmployeeAttendanceException)
                .ThenInclude(e => e!.Employee)
            .Include(i => i.AbsenceType)
            .Include(i => i.LeaveType)
            .Include(i => i.EmployeeExceptionJustificationRequestState)
            .SingleOrDefault(i =>
                i.Id == id
                && i.EmployeeAttendanceException != null
                && i.EmployeeAttendanceException.OrganisationChartId == _currentUserDefaultOrganId);

        var record = _mapper.Map<EmployeeExceptionJustificationRequestDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeExceptionJustificationRequestDTO entityToCreate)
    {
        var validation = await ValidateRequestAsync(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        PrepareRequestDto(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(EmployeeExceptionJustificationRequestDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.EmployeeExceptionJustificationRequests
            .AsNoTracking()
            .Include(i => i.EmployeeAttendanceException)
            .SingleOrDefaultAsync(i =>
                i.Id == entityToUpdate.Id
                && i.EmployeeAttendanceException != null
                && i.EmployeeAttendanceException.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted);

        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        entityToUpdate.EmployeeAttendanceExceptionId = existing.EmployeeAttendanceExceptionId;

        var validation = await ValidateRequestAsync(entityToUpdate, entityToUpdate.Id);
        if (!validation.Success)
        {
            return validation;
        }

        PrepareRequestDto(entityToUpdate, keepTitle: true);
        return await base.UpdateForAsync(entityToUpdate);
    }

    public new OperationResult DeleteRecord(long id)
    {
        var existing = _unitOfWork.Context.EmployeeExceptionJustificationRequests
            .Include(i => i.EmployeeAttendanceException)
            .SingleOrDefault(i =>
                i.Id == id
                && i.EmployeeAttendanceException != null
                && i.EmployeeAttendanceException.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted);

        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        return base.DeleteRecord(id);
    }

    private async Task<OperationResult> ValidateRequestAsync(EmployeeExceptionJustificationRequestDTO dto, long? excludeId = null)
    {
        if (dto.EmployeeAttendanceExceptionId <= 0)
        {
            return OperationResult.Failed("استثناء عدم حضور الزامی است");
        }

        if (dto.AbsenceTypeId <= 0)
        {
            return OperationResult.Failed("نوع عدم حضور الزامی است");
        }

        if (dto.EmployeeExceptionJustificationRequestStateId <= 0)
        {
            return OperationResult.Failed("وضعیت درخواست الزامی است");
        }

        if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 1024)
        {
            return OperationResult.Failed("طول توضیحات نمی‌تواند بیش از ۱۰۲۴ کاراکتر باشد");
        }

        var exception = await _unitOfWork.Context.EmployeeAttendanceExceptions
            .AsNoTracking()
            .SingleOrDefaultAsync(i =>
                i.Id == dto.EmployeeAttendanceExceptionId
                && i.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted);

        if (exception == null)
        {
            return OperationResult.Failed("استثناء عدم حضور انتخاب‌شده معتبر نیست");
        }

        if (dto.EmployeeId > 0 && exception.EmployeeId != dto.EmployeeId)
        {
            return OperationResult.Failed("استثناء عدم حضور با کارمند انتخاب‌شده همخوانی ندارد");
        }

        dto.EmployeeId = exception.EmployeeId;
        dto.EmployeeAttendanceException = exception.title;

        var absenceType = await _unitOfWork.Context.AbsenceTypes
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == dto.AbsenceTypeId && !i.IsDeleted);

        if (absenceType == null)
        {
            return OperationResult.Failed("نوع عدم حضور انتخاب‌شده معتبر نیست");
        }

        var isLeaveAbsenceType = dto.AbsenceTypeId == (long)AbsenceTypeCode.Leave;

        if (isLeaveAbsenceType)
        {
            if (!dto.LeaveTypeId.HasValue || dto.LeaveTypeId <= 0)
            {
                return OperationResult.Failed("نوع مرخصی الزامی است");
            }

            var leaveTypeExists = await _unitOfWork.Context.Set<HR.BaseInfo.Core.Entities.LeaveType>()
                .AnyAsync(i => i.Id == dto.LeaveTypeId && !i.IsDeleted);

            if (!leaveTypeExists)
            {
                return OperationResult.Failed("نوع مرخصی انتخاب‌شده معتبر نیست");
            }
        }
        else if (dto.LeaveTypeId.HasValue && dto.LeaveTypeId > 0)
        {
            var leaveTypeExists = await _unitOfWork.Context.Set<HR.BaseInfo.Core.Entities.LeaveType>()
                .AnyAsync(i => i.Id == dto.LeaveTypeId && !i.IsDeleted);

            if (!leaveTypeExists)
            {
                return OperationResult.Failed("نوع مرخصی انتخاب‌شده معتبر نیست");
            }
        }
        else
        {
            dto.LeaveTypeId = null;
        }

        var stateExists = await _unitOfWork.Context.EmployeeExceptionJustificationRequestStates
            .AnyAsync(i => i.Id == dto.EmployeeExceptionJustificationRequestStateId && !i.IsDeleted);

        if (!stateExists)
        {
            return OperationResult.Failed("وضعیت درخواست انتخاب‌شده معتبر نیست");
        }

        var duplicateExists = await _unitOfWork.Context.EmployeeExceptionJustificationRequests
            .AnyAsync(i =>
                i.EmployeeAttendanceExceptionId == dto.EmployeeAttendanceExceptionId
                && i.AbsenceTypeId == dto.AbsenceTypeId
                && i.EmployeeExceptionJustificationRequestStateId == dto.EmployeeExceptionJustificationRequestStateId
                && !i.IsDeleted
                && (!excludeId.HasValue || i.Id != excludeId.Value));

        if (duplicateExists)
        {
            return OperationResult.Failed("درخواست توجیه مشابه برای این استثناء قبلاً ثبت شده است");
        }

        return OperationResult.Succeeded();
    }

    private static void PrepareRequestDto(EmployeeExceptionJustificationRequestDTO dto, bool keepTitle = false)
    {
        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            var exceptionPart = string.IsNullOrWhiteSpace(dto.EmployeeAttendanceException)
                ? dto.EmployeeAttendanceExceptionId.ToString()
                : dto.EmployeeAttendanceException;
            var statePart = string.IsNullOrWhiteSpace(dto.EmployeeExceptionJustificationRequestState)
                ? dto.EmployeeExceptionJustificationRequestStateId.ToString()
                : dto.EmployeeExceptionJustificationRequestState;
            dto.title = $"{exceptionPart} - {statePart}";
        }
    }
}
