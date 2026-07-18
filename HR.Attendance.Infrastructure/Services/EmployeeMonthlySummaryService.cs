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

public class EmployeeMonthlySummaryService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeMonthlySummary, AttendanceContext, EmployeeMonthlySummaryDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    private static readonly Dictionary<int, string> PersianMonths = new()
    {
        [1] = "فروردین",
        [2] = "اردیبهشت",
        [3] = "خرداد",
        [4] = "تیر",
        [5] = "مرداد",
        [6] = "شهریور",
        [7] = "مهر",
        [8] = "آبان",
        [9] = "آذر",
        [10] = "دی",
        [11] = "بهمن",
        [12] = "اسفند",
    };

    public new OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<EmployeeMonthlySummary>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<EmployeeMonthlySummary> all = All(IgnoreExpired)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.Employee)
            .Include(i => i.OrganisationChart)
            .Include(i => i.CostCenter)
            .Include(i => i.OrganizationUnit)
            .Include(i => i.WorkPlace);

        if (EmployeeId > 0)
        {
            all = all.Where(i => i.EmployeeId == EmployeeId);
        }

        var rowCount = 0;
        var pagedData = PagerUtility<EmployeeMonthlySummary>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<EmployeeMonthlySummaryDTO>>(pagedData);
        foreach (var item in result)
        {
            item.MonthTitle = ToMonthTitle(item.Month);
        }

        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Employee)
            .Include(i => i.OrganisationChart)
            .Include(i => i.CostCenter)
            .Include(i => i.OrganizationUnit)
            .Include(i => i.WorkPlace)
            .SingleOrDefault(i => i.Id == id && i.OrganisationChartId == _currentUserDefaultOrganId);

        var record = _mapper.Map<EmployeeMonthlySummaryDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        record.MonthTitle = ToMonthTitle(record.Month);
        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeMonthlySummaryDTO entityToCreate)
    {
        var validation = await ValidateSummaryAsync(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareSummaryDto(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(EmployeeMonthlySummaryDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.EmployeeMonthlySummaries
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

        var validation = await ValidateSummaryAsync(entityToUpdate, entityToUpdate.Id);
        if (!validation.Success)
        {
            return validation;
        }

        entityToUpdate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareSummaryDto(entityToUpdate, keepTitle: true);
        return await base.UpdateForAsync(entityToUpdate);
    }

    public new OperationResult DeleteRecord(long id)
    {
        var existing = _unitOfWork.Context.EmployeeMonthlySummaries
            .SingleOrDefault(i =>
                i.Id == id
                && i.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted);

        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        return base.DeleteRecord(id);
    }

    public OperationResult GetYearList()
    {
        List<SharedKernel.Data.KeyValuePair> years = [];
        var persianCalendar = new System.Globalization.PersianCalendar();
        var shamsiYear = persianCalendar.GetYear(DateTime.Now);
        const int startYear = 1368;

        for (var i = startYear; i <= shamsiYear + 1; i++)
        {
            years.Add(new SharedKernel.Data.KeyValuePair
            {
                key = i,
                id = i,
                value = i.ToString(),
            });
        }

        return OperationResult.Succeeded(payload: years.OrderByDescending(i => i.key));
    }

    public OperationResult GetMonths()
    {
        List<SharedKernel.Data.KeyValuePair> months = [];

        for (var i = 1; i < 13; i++)
        {
            months.Add(new SharedKernel.Data.KeyValuePair
            {
                key = i,
                id = i,
                value = ToMonthTitle(i) ?? i.ToString(),
            });
        }

        return OperationResult.Succeeded(payload: months);
    }

    private async Task<OperationResult> ValidateSummaryAsync(EmployeeMonthlySummaryDTO dto, long? excludeId = null)
    {
        if (dto.EmployeeId <= 0)
        {
            return OperationResult.Failed("کارمند الزامی است");
        }

        if (!dto.Year.HasValue || dto.Year <= 0)
        {
            return OperationResult.Failed("سال الزامی است");
        }

        if (!dto.Month.HasValue || dto.Month < 1 || dto.Month > 12)
        {
            return OperationResult.Failed("ماه الزامی است");
        }

        if (!dto.FunctionDay.HasValue || dto.FunctionDay <= 0)
        {
            return OperationResult.Failed("روزهای کاری ماه الزامی است");
        }

        if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 1012)
        {
            return OperationResult.Failed("طول توضیحات نمی‌تواند بیش از ۱۰۱۲ کاراکتر باشد");
        }

        if (!string.IsNullOrWhiteSpace(dto.Comment) && dto.Comment.Length > 2048)
        {
            return OperationResult.Failed("طول اظهار نظر نمی‌تواند بیش از ۲۰۴۸ کاراکتر باشد");
        }

        var duplicateExists = await _unitOfWork.Context.EmployeeMonthlySummaries
            .AnyAsync(i =>
                i.OrganisationChartId == _currentUserDefaultOrganId
                && i.EmployeeId == dto.EmployeeId
                && i.Year == dto.Year
                && i.Month == dto.Month
                && !i.IsDeleted
                && (!excludeId.HasValue || i.Id != excludeId.Value));

        if (duplicateExists)
        {
            return OperationResult.Failed("خلاصه ماهانه برای این کارمند در سال و ماه انتخاب‌شده قبلاً ثبت شده است");
        }

        return OperationResult.Succeeded();
    }

    private static void PrepareSummaryDto(EmployeeMonthlySummaryDTO dto, bool keepTitle = false)
    {
        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            var employeePart = string.IsNullOrWhiteSpace(dto.Employee) ? dto.EmployeeId.ToString() : dto.Employee;
            var monthPart = ToMonthTitle(dto.Month) ?? dto.Month?.ToString() ?? "-";
            var yearPart = dto.Year?.ToString() ?? "-";
            dto.title = $"{employeePart} - {monthPart} {yearPart}";
        }
    }

    private static string? ToMonthTitle(int? month)
    {
        if (!month.HasValue)
        {
            return null;
        }

        return PersianMonths.TryGetValue(month.Value, out var title) ? title : month.Value.ToString();
    }
}
