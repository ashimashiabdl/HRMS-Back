using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.Employee.infrastructure.Services;
using HR.BaseInfo.Core.DTOs;
using HR.Employee.Core.DTOs;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace HRMS.API.Controllers.Employee;

[Route("api/Employee")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("اطلاعات کارکنان")]
//[EmployeeAccessCheck]
public class EmployeeController : AppBaseController
{
    private readonly EmployeeService _EmployeeService;
    public EmployeeController(EmployeeService Service, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _EmployeeService = Service;
        _EmployeeService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }





    [HttpGet, Route("GetAsKeyValuePairLazy/{filter}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePairLazy(string filter)
    {
        return this.AppOk(_EmployeeService.GetAsKeyValuePairLazy(filter));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        if (!_EmployeeService.CheckAccess(currentUserId, id))
        {
            return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }

        var result = _EmployeeService.GetIncluded(id);
        return this.AppOk(result);
    }

    [HttpGet, Route("GetEntityCounts/{id}")]
    [CustomAccessKey(AccessKey: "Summary")]
    public IActionResult GetEntityCounts(long id)
    {
        if (!_EmployeeService.CheckAccess(currentUserId, id))
        {
            return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }
        var result = _EmployeeService.GetEmployeeEntityCounts(id);
        return this.AppOk(result);
    }

    [HttpGet, Route("GetSummary/{id}")]
    [CustomAccessKey(AccessKey: "Summary")]
    public async Task<IActionResult> GetSummary(long id)
    {
        if (!_EmployeeService.CheckAccess(currentUserId, id))
        {
            return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }
        var result = await _EmployeeService.GetEmployeeSummaryAsync(id);
        return this.AppOk(result);
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? BaseOrganisationId = null)
    {
        if (activeSortColumn == "baseOrganisationTitle")
        {
            activeSortColumn = "BaseOrganisationId";
        }
        if (activeSortColumn == "genderTitle")
        {
            activeSortColumn = "GenderId";
        }

        // Filter to accessible employees; optional organ filter matches BaseOrganisationId
        // or current active order pay location (Interdict_Order StatusId = 9).
        var filtered = _EmployeeService.GetAccessibleEmployeesQueryable(currentUserId, BaseOrganisationId);
        return this.AppOk(_EmployeeService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: filtered));
    }

    [HttpPut("UpdateCurrentUserPassword")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> UpdateCurrentUserPassword([FromBody] UpdatePassCurrentEmployeeDTO body, [FromServices] HRMS.API.Infrastructure.Security.RsaKeyService rsaKeyService)
    {
        const string methodName = nameof(UpdateCurrentUserPassword);
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!_EmployeeService.CheckAccess(currentUserId, body.EmployeeId))
        {
            return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }

        // Decrypt password if encrypted (one-time use encryption)
        try
        {
            // Decrypt newpass
            if (!string.IsNullOrEmpty(body.newpass) && body.newpass.StartsWith("enc::", StringComparison.Ordinal))
            {
                var parts = body.newpass.Split(new[] { "::" }, StringSplitOptions.None);
                if (parts.Length == 3)
                {
                    var keyId = parts[1];
                    var cipher = parts[2];
                    if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                    {
                        body.newpass = plain;
                    }
                    else
                    {
                        _logger.LogWarning("[{MethodName}] Failed to decrypt newpass. UserId: {UserId}, IP: {IP}", 
                            methodName, currentUserId, ipAddress);
                        return this.AppBadRequest("خطا در رمزگشایی کلمه عبور جدید");
                    }
                }
            }

            // Decrypt newpassconfirm if encrypted
            if (!string.IsNullOrEmpty(body.newpassconfirm) && body.newpassconfirm.StartsWith("enc::", StringComparison.Ordinal))
            {
                var parts = body.newpassconfirm.Split(new[] { "::" }, StringSplitOptions.None);
                if (parts.Length == 3)
                {
                    var keyId = parts[1];
                    var cipher = parts[2];
                    if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                    {
                        body.newpassconfirm = plain;
                    }
                    else
                    {
                        _logger.LogWarning("[{MethodName}] Failed to decrypt newpassconfirm. UserId: {UserId}, IP: {IP}", 
                            methodName, currentUserId, ipAddress);
                        return this.AppBadRequest("خطا در رمزگشایی تکرار کلمه عبور جدید");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{MethodName}] Error decrypting passwords. UserId: {UserId}, IP: {IP}", 
                methodName, currentUserId, ipAddress);
            return this.AppBadRequest("خطا در رمزگشایی رمزهای عبور");
        }

        var result = await _EmployeeService.UpdateCurrentUserPassword(body);
        return this.AppOk(result);
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] EmployeeDTO body)
    {
        body.BaseOrganisationId = currentUserDefaultOrganId;
        try
        {
            if (HR.SharedKernel.Utilities.IsValidNationalCode(body.NationalNo))
            {

            }
            else
            {
                return this.AppBadRequest("کد ملی معتبر نمی باشد");
            }
        }
        catch (Exception ex)
        {
            return this.AppBadRequest(ex.Message);
        }
        if (_EmployeeService.All().Any(i => i.NationalNo == body.NationalNo))
        {
            return this.AppBadRequest("کد ملی وارد شده در سیستم موجود می باشد");
        }

        return Ok(await _EmployeeService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] EmployeeDTO body)
    {

        if (!_EmployeeService.CheckAccess(currentUserId, body.EmployeeId.Value))
        {
            return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }
        try
        {
            if (HR.SharedKernel.Utilities.IsValidNationalCode(body.NationalNo))
            {

            }
            else
            {
                return this.AppBadRequest("کد ملی معتبر نمی باشد");
            }
        }
        catch (Exception ex)
        {
            return this.AppBadRequest(ex.Message);
        }

        if (_EmployeeService.All().Any(i => i.NationalNo == body.NationalNo && i.Id != body.Id))
        {
            return this.AppBadRequest("کد ملی وارد شده در سیستم موجود می باشد");
        }

        var result = await _EmployeeService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpPost("AdvanceSearch")]
    [CustomAccessKey(AccessKey: "AdvanceSearch")]
    public IActionResult AdvanceSearch([FromBody] AdvanceSearchDTO body)
    {
        // Normalize incoming string fields by trimming whitespace when provided
        if (body != null)
        {
            if (!string.IsNullOrWhiteSpace(body.FirstName)) body.FirstName = body.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(body.LastName)) body.LastName = body.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(body.PersonelCode)) body.PersonelCode = body.PersonelCode.Trim();
            if (!string.IsNullOrWhiteSpace(body.IdentityNo)) body.IdentityNo = body.IdentityNo.Trim();
            if (!string.IsNullOrWhiteSpace(body.NationalNo)) body.NationalNo = body.NationalNo.Trim();
            if (!string.IsNullOrWhiteSpace(body.ActiveName)) body.ActiveName = body.ActiveName.Trim();
            if (!string.IsNullOrWhiteSpace(body.SortBy)) body.SortBy = body.SortBy.Trim();
            if (!string.IsNullOrWhiteSpace(body.SortDirection)) body.SortDirection = body.SortDirection.Trim();
        }
        if (ModelState.IsValid)
        {
            return this.AppOk(_EmployeeService.AdvanceSearch(body, currentUserId));
        }
        return this.AppBadRequest(ModelState);
    }

    [HttpGet("GetMyUnfinishedEmployeesCount")]
    [CustomAccessKey(AccessKey: "GetMyUnfinishedEmployeesCount")]
    public IActionResult GetMyUnfinishedEmployeesCount()
    {
        var result = _EmployeeService.GetAccessibleEmployeesWithoutFinalOrderCount(currentUserId);
        return this.AppOk(result);
    }
}
