using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.infrastructure.Services;
using HR.Report.Core.DTOs;
using HR.Report.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using HR.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.Report;

[Route("api/DynamicReport")]
[ControllerGroup("Report", "گزارش گیری")]
[DisplayName("گزارش های پویا")]
public class DynamicReportController : AppBaseController
{
    private const long ReportTypeSqlQuery = 11210;
    private const long ReportTypeFunction = 11211;
    private const long ExportTypePdf = 21522;
    private const long ExportTypeMrt = 21523;

    private readonly DynamicReportService _DynamicReportService;
    private readonly UserReportService _userReportService;
    private readonly TempGlobalFileService _tempGlobalFileService;

    public DynamicReportController(
        DynamicReportService Service,
        TempGlobalFileService TempGlobalFileService,
        UserReportService UserReportService,
        ILogger<DynamicReportController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService UserResolverService)
        : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _tempGlobalFileService = TempGlobalFileService;
        _userReportService = UserReportService;
        _DynamicReportService = Service;
        _DynamicReportService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _mapper = mapper;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_DynamicReportService.GetAsKeyValuePair());
    }

    [HttpGet, Route("GetForReport/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetForReport(int id)
    {
        if (id <= 0)
        {
            return Ok(OperationResult.NotFound("شناسه گزارش نامعتبر است."));
        }

        if (!HasReportAccess(id))
        {
            return Ok(OperationResult.Failed("شما به گزارش مورد نظر دسترسی ندارید."));
        }

        var report = await _DynamicReportService.GetIdAsync(id);
        if (report == null)
        {
            return Ok(OperationResult.NotFound("گزارش مورد نظر یافت نشد."));
        }

        if (!report.IsActive)
        {
            return Ok(OperationResult.Failed("گزارش مورد نظر غیرفعال است."));
        }

        var ret = _mapper.Map<DynamicReportDTO>(report);
        ret.SqlQuery = string.Empty;

        var parameters = await _DynamicReportService._unitOfWork.Context.DynamicReportParameter
            .AsNoTracking()
            .Include(i => i.Parameter)
            .Where(i => i.DynamicReportId == id)
            .ToListAsync();

        ret.DynamicReportParameterList = _mapper.Map<List<DynamicReportParameterDTO>>(parameters);
        return Ok(OperationResult.Succeeded(payload: ret));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> Get(int id)
    {
        if (id <= 0)
        {
            return Ok(OperationResult.NotFound("شناسه گزارش نامعتبر است."));
        }

        var report = await _DynamicReportService.GetIdAsync(id);
        if (report == null)
        {
            return Ok(OperationResult.NotFound("گزارش مورد نظر یافت نشد."));
        }

        var ret = _mapper.Map<DynamicReportDTO>(report);
        var parameters = await _DynamicReportService._unitOfWork.Context.DynamicReportParameter
            .AsNoTracking()
            .Where(i => i.DynamicReportId == id)
            .ToListAsync();

        ret.DynamicReportParameterList = _mapper.Map<List<DynamicReportParameterDTO>>(parameters);
        return Ok(OperationResult.Succeeded(payload: ret));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true)
    {
        if (isAdmin)
        {
            return this.AppOk(_DynamicReportService.GetPagedData(
                currentPage: currentPage,
                pageSize: pageSize,
                filter,
                activeSortColumn,
                Sortdirection,
                IgnoreExpired,
                IgnoreDefaultOrganId: true));
        }

        var allowedReportIds = GetAllowedReportIds();
        var customDataSource = _DynamicReportService.All(IgnoreExpired)
            .Where(i => allowedReportIds.Contains(i.Id));

        return this.AppOk(_DynamicReportService.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            CustomDataSource: customDataSource));
    }

    [HttpPost("DownloadDynamicReport")]
    [CustomAccessKey(AccessKey: "DownloadDynamicReport")]
    public async Task<IActionResult> DownloadDynamicReport([FromBody] RequestReportDTOStandard body)
    {
        if (body.Id == 0)
        {
            return Ok(OperationResult.NotFound("شناسه گزارش نامعتبر است."));
        }

        if (!HasReportAccess(body.Id))
        {
            return Ok(OperationResult.Failed("شما به گزارش مورد نظر دسترسی ندارید."));
        }

        var parameterError = ValidateReportParameters(body);
        if (parameterError != null)
        {
            return Ok(parameterError);
        }

        body.UserName = CurrentUserFullName;
        body.CurrentUserId = currentUserId;

        var reportResult = await _DynamicReportService.CreateDynamicReport(body, fontsDirectory: null);
        if (reportResult == null)
        {
            return Ok(OperationResult.Failed("خطا در تهیه گزارش: پاسخی از سرویس دریافت نشد."));
        }

        if (!reportResult.Success)
        {
            return Ok(OperationResult.Failed(
                string.IsNullOrWhiteSpace(reportResult.Message)
                    ? "خطا در تهیه گزارش."
                    : reportResult.Message));
        }

        var file = await _tempGlobalFileService.GetIdAsync(reportResult.Id);
        if (file == null)
        {
            return Ok(OperationResult.Failed("فایل گزارش یافت نشد. لطفاً دوباره تلاش کنید."));
        }

        if (file.Content == null || file.Content.Length == 0)
        {
            return Ok(OperationResult.Failed("فایل گزارش خالی است. پارامترهای گزارش را بررسی کنید."));
        }

        file.temp_inBase64 = Convert.ToBase64String(file.Content);
        file.Content = null;
        file.title = (file.title ?? "Report").Replace(" ", "_");

        return Ok(OperationResult.Succeeded(payload: file));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] DynamicReportDTO body)
    {
        var validationError = ValidateDynamicReportBody(body);
        if (validationError != null)
        {
            return Ok(validationError);
        }

        NormalizeDynamicReportBody(body);
        _DynamicReportService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        return Ok(await _DynamicReportService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] DynamicReportDTO body)
    {
        if (body.Id <= 0)
        {
            return Ok(OperationResult.NotFound("شناسه گزارش برای بروزرسانی نامعتبر است."));
        }

        var validationError = ValidateDynamicReportBody(body);
        if (validationError != null)
        {
            return Ok(validationError);
        }

        NormalizeDynamicReportBody(body);
        return Ok(await _DynamicReportService.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return Ok(OperationResult.NotFound("شناسه گزارش نامعتبر است."));
        }

        return Ok(_DynamicReportService.DeleteRecord(id));
    }

    private List<long> GetAllowedReportIds()
    {
        var userReportIds = _userReportService.All()
            .Where(i => i.UserId == currentUserId)
            .Select(i => i.DynamicReportId);

        var userRoleIds = _userReportService._unitOfWork.Context.Set<UserRole>()
            .AsNoTracking()
            .Where(ur => ur.UserId == currentUserId)
            .Select(ur => ur.RoleId);

        var roleReportIds = _userReportService._unitOfWork.Context.Set<RoleReport>()
            .AsNoTracking()
            .Where(rr => userRoleIds.Contains(rr.RoleId))
            .Select(rr => rr.DynamicReportId);

        return userReportIds.Union(roleReportIds).Distinct().ToList();
    }

    private bool HasReportAccess(long reportId)
    {
        if (isAdmin)
        {
            return true;
        }

        return GetAllowedReportIds().Contains(reportId);
    }

    private static OperationResult? ValidateDynamicReportBody(DynamicReportDTO body)
    {
        if (body.FuctionTypeId == ReportTypeSqlQuery)
        {
            return OperationResult.Failed("گزارش پویا با کوئری SQL آزاد فعلاً غیرفعال است.");
        }

        if (body.FuctionTypeId == ReportTypeFunction
            && (string.IsNullOrWhiteSpace(body.Schema) || string.IsNullOrWhiteSpace(body.FunctionName)))
        {
            return OperationResult.Failed("نام Schema و Function برای گزارش تابعی اجباری است.");
        }

        return null;
    }

    private static void NormalizeDynamicReportBody(DynamicReportDTO body)
    {
        if (body.FuctionTypeId == ReportTypeFunction)
        {
            body.SqlQuery = null;
        }

        if (body.ExportTypeId != ExportTypePdf && body.ExportTypeId != ExportTypeMrt)
        {
            body.OrganisationMRTId = null;
        }
    }

    private OperationResult? ValidateReportParameters(RequestReportDTOStandard body)
    {
        var requiredParameters = _DynamicReportService._unitOfWork.Context.DynamicReportParameter
            .AsNoTracking()
            .Include(i => i.Parameter)
            .Where(i => i.DynamicReportId == body.Id && !i.Optional)
            .ToList();

        if (requiredParameters.Count == 0)
        {
            return null;
        }

        if (body.Parameters == null)
        {
            return OperationResult.Failed("پارامترهای گزارش ارسال نشده‌اند.");
        }

        if (!body.Parameters.Any())
        {
            return OperationResult.Failed("هیچ پارامتری برای گزارش ارسال نشده است.");
        }

        foreach (var required in requiredParameters)
        {
            var isProvided = body.Parameters.Any(
                p => p.ParameterId == required.ParameterId
                     && p.SelectedValues != null
                     && p.SelectedValues.Any(v => !string.IsNullOrWhiteSpace(v)));

            if (!isProvided)
            {
                var parameterName = required.Parameter?.Value ?? $"شناسه {required.ParameterId}";
                return OperationResult.Failed($"پارامتر اجباری «{parameterName}» مقداردهی نشده است.");
            }
        }

        return null;
    }
}
