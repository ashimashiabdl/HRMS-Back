using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationAgentOfPunishmentEncourage")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("عوامل تنبیه و تشویق")]
public class OrganisationAgentOfPunishmentEncourageController : AppBaseController
{
    private readonly OrganisationAgentOfPunishmentEncourageService _OrganisationAgentOfPunishmentEncourageService;
    public OrganisationAgentOfPunishmentEncourageController(OrganisationAgentOfPunishmentEncourageService Service, ILogger<OrganisationAgentOfPunishmentEncourageController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationAgentOfPunishmentEncourageService = Service;
        _OrganisationAgentOfPunishmentEncourageService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationAgentOfPunishmentEncourageDTO body)
    {
        return Ok(await _OrganisationAgentOfPunishmentEncourageService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationAgentOfPunishmentEncourageDTO body)
    {
        var result = await _OrganisationAgentOfPunishmentEncourageService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageService.DeleteRecord(id));
    }
}
