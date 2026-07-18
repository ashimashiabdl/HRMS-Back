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

public class EmployeeAttendanceExceptionService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeAttendanceException, AttendanceContext, EmployeeAttendanceExceptionDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
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
        IQueryable<EmployeeAttendanceException>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<EmployeeAttendanceException> all = All(IgnoreExpired)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.Employee)
            .Include(i => i.AttendanceCalendar)
            .Include(i => i.AbsenceType)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart);

        if (EmployeeId > 0)
        {
            all = all.Where(i => i.EmployeeId == EmployeeId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<EmployeeAttendanceException>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<EmployeeAttendanceExceptionDTO>>(pagedData);
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Employee)
            .Include(i => i.AttendanceCalendar)
            .Include(i => i.AbsenceType)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart)
            .SingleOrDefault(i => i.Id == id && i.OrganisationChartId == _currentUserDefaultOrganId);

        var record = _mapper.Map<EmployeeAttendanceExceptionDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeAttendanceExceptionDTO entityToCreate)
    {
        var validation = await ValidateExceptionAsync(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareExceptionDto(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(EmployeeAttendanceExceptionDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.EmployeeAttendanceExceptions
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

        var validation = await ValidateExceptionAsync(entityToUpdate, entityToUpdate.Id);
        if (!validation.Success)
        {
            return validation;
        }

        entityToUpdate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareExceptionDto(entityToUpdate, keepTitle: true);
        return await base.UpdateForAsync(entityToUpdate);
    }

    private async Task<OperationResult> ValidateExceptionAsync(EmployeeAttendanceExceptionDTO dto, long? excludeId = null)
    {
        if (dto.EmployeeId <= 0)
        {
            return OperationResult.Failed("کارمند الزامی است");
        }

        if (dto.AttendanceCalendarId <= 0)
        {
            return OperationResult.Failed("تقویم حضور و غیاب الزامی است");
        }

        if (dto.AbsenceTypeId <= 0)
        {
            return OperationResult.Failed("نوع عدم حضور الزامی است");
        }

        if (dto.ShiftId <= 0)
        {
            return OperationResult.Failed("شیفت الزامی است");
        }

        if (dto.StartAt == default)
        {
            return OperationResult.Failed("لحظه آغاز الزامی است");
        }

        if (dto.EndAt == default)
        {
            return OperationResult.Failed("لحظه پایان الزامی است");
        }

        if (dto.EndAt <= dto.StartAt)
        {
            return OperationResult.Failed("لحظه پایان باید بعد از لحظه آغاز باشد");
        }

        if (dto.DurationSeconds <= 0)
        {
            return OperationResult.Failed("مدت عدم حضور باید بزرگ‌تر از صفر باشد");
        }

        var calculatedDuration = (int)Math.Round((dto.EndAt - dto.StartAt).TotalSeconds);
        if (dto.DurationSeconds != calculatedDuration)
        {
            return OperationResult.Failed("مدت عدم حضور با تفاوت لحظه آغاز و پایان همخوانی ندارد");
        }

        var calendar = await _unitOfWork.Context.AttendanceCalendars
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == dto.AttendanceCalendarId && !i.IsDeleted);

        if (calendar == null)
        {
            return OperationResult.Failed("تقویم حضور و غیاب انتخاب‌شده معتبر نیست");
        }

        if (dto.StartAt.Date != calendar.Date.Date)
        {
            return OperationResult.Failed("لحظه آغاز باید در تاریخ تقویم انتخاب‌شده باشد");
        }

        dto.AttendanceCalendar = AttendanceCalendarService.FormatPersianDateLabel(calendar.Date, calendar.WeekDay);

        var absenceTypeExists = await _unitOfWork.Context.AbsenceTypes
            .AnyAsync(i => i.Id == dto.AbsenceTypeId && !i.IsDeleted);

        if (!absenceTypeExists)
        {
            return OperationResult.Failed("نوع عدم حضور انتخاب‌شده معتبر نیست");
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

        var duplicateExists = await _unitOfWork.Context.EmployeeAttendanceExceptions
            .AnyAsync(i =>
                i.OrganisationChartId == _currentUserDefaultOrganId
                && i.EmployeeId == dto.EmployeeId
                && i.AttendanceCalendarId == dto.AttendanceCalendarId
                && i.AbsenceTypeId == dto.AbsenceTypeId
                && i.StartAt == dto.StartAt
                && i.EndAt == dto.EndAt
                && !i.IsDeleted
                && (!excludeId.HasValue || i.Id != excludeId.Value));

        if (duplicateExists)
        {
            return OperationResult.Failed("بازه عدم حضور مشابه برای این کارمند در تاریخ انتخاب‌شده قبلاً ثبت شده است");
        }

        return OperationResult.Succeeded();
    }

    public OperationResult GetAsKeyValuePair(long? employeeId = null)
    {
        var query = All()
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);

        if (employeeId > 0)
        {
            query = query.Where(i => i.EmployeeId == employeeId);
        }

        return OperationResult.Succeeded(payload: query
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.title
            }));
    }

    private static void PrepareExceptionDto(EmployeeAttendanceExceptionDTO dto, bool keepTitle = false)
    {
        dto.DurationSeconds = (int)Math.Round((dto.EndAt - dto.StartAt).TotalSeconds);

        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            var employeePart = string.IsNullOrWhiteSpace(dto.Employee) ? dto.EmployeeId.ToString() : dto.Employee;
            var calendarPart = string.IsNullOrWhiteSpace(dto.AttendanceCalendar)
                ? dto.AttendanceCalendarId.ToString()
                : dto.AttendanceCalendar;
            dto.title = $"{employeePart} - {calendarPart}";
        }
    }
}
