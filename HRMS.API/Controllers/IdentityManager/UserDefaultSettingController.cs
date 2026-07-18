using HR.Identity.Core.DTOs;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserDefaultSetting")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("تنظیمات پیش فرض کاربر")]
public partial class UserDefaultSettingController : AppBaseController
{
    private readonly CustomIdentityContext _context;
    private readonly UserDefaultSettingService _userDefaultSettingService;

    public UserDefaultSettingController(
        CustomIdentityContext context,
        UserDefaultSettingService userDefaultSettingService,
        ILogger<UserDefaultSettingController> logger,
        IHttpContextAccessor accessor,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, null, dapper)
    {
        _context = context;
        _userDefaultSettingService = userDefaultSettingService;
        _userDefaultSettingService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetCurrentUserDefultSettingAndInsertIfNotExist")]
    [CustomAccessKey(AccessKey: "CurrentUser.view")]
    public async Task<IActionResult> GetCurrentUserDefultSettingAndInsertIfNotExist()
    {
        var result = await _userDefaultSettingService.GetCurrentUserDefultSettingAndInsertIfNotExist(currentUserId);
        return this.AppOk(result);
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "CurrentUser.update")]
    public async Task<IActionResult> Put([FromBody] UserDefaultSettingDTO body)
    {
        body.UserId = currentUserId;
        return this.AppOk(await _userDefaultSettingService.UpdateForAsync(body));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "Admin.view")]
    public IActionResult Get(int id) =>
        this.AppOk(_userDefaultSettingService.Get(id));

    [HttpGet, Route("GetByUserId/{userId}")]
    [CustomAccessKey(AccessKey: "Admin.view")]
    public async Task<IActionResult> GetByUserId(long userId) =>
        this.AppOk(await _userDefaultSettingService.GetByUserId(userId));

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "Admin.view")]
    public async Task<IActionResult> GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true,
        [FromQuery] long? UserId = null)
    {
        var filtered = BuildFilteredQuery(IgnoreExpired, UserId);
        var result = _userDefaultSettingService.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            EmployeeId: null,
            CustomDataSource: filtered);

        await EnrichPagedPayloadWithOrganTitlesAsync(result, filtered);
        return this.AppOk(result);
    }

    [HttpPost("PostForUser")]
    [CustomAccessKey(AccessKey: "Admin.create")]
    public Task<IActionResult> PostForUser([FromBody] UserDefaultSettingDTO body) =>
        UpsertForUserAsync(body);

    [HttpPut("PutForUser")]
    [CustomAccessKey(AccessKey: "Admin.update")]
    public Task<IActionResult> PutForUser([FromBody] UserDefaultSettingDTO body) =>
        UpsertForUserAsync(body);

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "Admin.delete")]
    public IActionResult Delete(int id) =>
        this.AppOk(_userDefaultSettingService.DeleteRecord(id));

    private async Task<IActionResult> UpsertForUserAsync(UserDefaultSettingDTO body) =>
        this.AppOk(await _userDefaultSettingService.UpsertForUserAsync(body));
}
