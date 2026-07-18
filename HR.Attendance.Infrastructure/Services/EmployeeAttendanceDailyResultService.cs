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

public class EmployeeAttendanceDailyResultService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeAttendanceDailyResult, AttendanceContext, EmployeeAttendanceDailyResultDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
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
        IQueryable<EmployeeAttendanceDailyResult>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<EmployeeAttendanceDailyResult> all = All(IgnoreExpired)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.Employee)
            .Include(i => i.AttendanceCalendar)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart);

        if (EmployeeId > 0)
        {
            all = all.Where(i => i.EmployeeId == EmployeeId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<EmployeeAttendanceDailyResult>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<EmployeeAttendanceDailyResultDTO>>(pagedData);
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Employee)
            .Include(i => i.AttendanceCalendar)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart)
            .SingleOrDefault(i => i.Id == id && i.OrganisationChartId == _currentUserDefaultOrganId);

        var record = _mapper.Map<EmployeeAttendanceDailyResultDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeAttendanceDailyResultDTO entityToCreate)
    {
        var validation = await ValidateDailyResultAsync(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareDailyResultDto(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(EmployeeAttendanceDailyResultDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.EmployeeAttendanceDailyResults
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

        var validation = await ValidateDailyResultAsync(entityToUpdate, entityToUpdate.Id);
        if (!validation.Success)
        {
            return validation;
        }

        entityToUpdate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareDailyResultDto(entityToUpdate, keepTitle: true);
        return await base.UpdateForAsync(entityToUpdate);
    }

    private async Task<OperationResult> ValidateDailyResultAsync(EmployeeAttendanceDailyResultDTO dto, long? excludeId = null)
    {
        if (dto.EmployeeId <= 0)
        {
            return OperationResult.Failed("کارمند الزامی است");
        }

        if (dto.AttendanceCalendarId <= 0)
        {
            return OperationResult.Failed("تقویم حضور و غیاب الزامی است");
        }

        if (dto.ShiftId <= 0)
        {
            return OperationResult.Failed("شیفت الزامی است");
        }

        var calendar = await _unitOfWork.Context.AttendanceCalendars
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == dto.AttendanceCalendarId && !i.IsDeleted);

        if (calendar == null)
        {
            return OperationResult.Failed("تقویم حضور و غیاب انتخاب‌شده معتبر نیست");
        }

        dto.AttendanceCalendar = AttendanceCalendarService.FormatPersianDateLabel(calendar.Date, calendar.WeekDay);

        var shiftExists = await _unitOfWork.Context.Shifts
            .AnyAsync(i => i.Id == dto.ShiftId
                && i.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted
                && i.IsActive);

        if (!shiftExists)
        {
            return OperationResult.Failed("شیفت انتخاب‌شده معتبر نیست یا غیرفعال است");
        }

        var duplicateExists = await _unitOfWork.Context.EmployeeAttendanceDailyResults
            .AnyAsync(i =>
                i.OrganisationChartId == _currentUserDefaultOrganId
                && i.EmployeeId == dto.EmployeeId
                && i.AttendanceCalendarId == dto.AttendanceCalendarId
                && !i.IsDeleted
                && (!excludeId.HasValue || i.Id != excludeId.Value));

        if (duplicateExists)
        {
            return OperationResult.Failed("برای این کارمند در تاریخ تقویم انتخاب‌شده، نتیجه روزانه قبلاً ثبت شده است");
        }

        var inOutValidation = ValidateInOutPairs(dto);
        if (!inOutValidation.Success)
        {
            return inOutValidation;
        }

        return OperationResult.Succeeded();
    }

    private static OperationResult ValidateInOutPairs(EmployeeAttendanceDailyResultDTO dto)
    {
        var pairs = new (DateTime? In, DateTime? Out, string InLabel, string OutLabel)[]
        {
            (dto.SecondIn, dto.SecondOut, "ورود دوم", "خروج دوم"),
            (dto.ThirdIn, dto.ThirdOut, "ورود سوم", "خروج سوم"),
            (dto.FourthIn, dto.FourthOut, "ورود چهارم", "خروج چهارم"),
            (dto.FifthIn, dto.FifthOut, "ورود پنجم", "خروج پنجم"),
            (dto.SixthIn, dto.SixthOut, "ورود ششم", "خروج ششم"),
            (dto.SeventhIn, dto.SeventhOut, "ورود هفتم", "خروج هفتم"),
        };

        foreach (var (inTime, outTime, inLabel, outLabel) in pairs)
        {
            if (inTime.HasValue && outTime.HasValue && outTime.Value < inTime.Value)
            {
                return OperationResult.Failed($"{outLabel} نمی‌تواند قبل از {inLabel} باشد");
            }
        }

        var orderedEvents = new List<(DateTime Time, string Label)>();
        void AddEvent(DateTime? time, string label)
        {
            if (time.HasValue)
            {
                orderedEvents.Add((time.Value, label));
            }
        }

        AddEvent(dto.FirstIn, "اولین ورود");
        AddEvent(dto.SecondIn, "ورود دوم");
        AddEvent(dto.SecondOut, "خروج دوم");
        AddEvent(dto.ThirdIn, "ورود سوم");
        AddEvent(dto.ThirdOut, "خروج سوم");
        AddEvent(dto.FourthIn, "ورود چهارم");
        AddEvent(dto.FourthOut, "خروج چهارم");
        AddEvent(dto.FifthIn, "ورود پنجم");
        AddEvent(dto.FifthOut, "خروج پنجم");
        AddEvent(dto.SixthIn, "ورود ششم");
        AddEvent(dto.SixthOut, "خروج ششم");
        AddEvent(dto.SeventhIn, "ورود هفتم");
        AddEvent(dto.SeventhOut, "خروج هفتم");
        AddEvent(dto.LastOut, "آخرین خروج");

        for (var i = 1; i < orderedEvents.Count; i++)
        {
            if (orderedEvents[i].Time < orderedEvents[i - 1].Time)
            {
                return OperationResult.Failed(
                    $"ترتیب زمانی نامعتبر است: {orderedEvents[i].Label} نمی‌تواند قبل از {orderedEvents[i - 1].Label} باشد");
            }
        }

        return OperationResult.Succeeded();
    }

    private static void PrepareDailyResultDto(EmployeeAttendanceDailyResultDTO dto, bool keepTitle = false)
    {
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
