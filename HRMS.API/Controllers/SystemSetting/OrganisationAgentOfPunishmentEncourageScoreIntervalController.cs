using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using Hr.SystemSetting.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationAgentOfPunishmentEncourageScoreInterval")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("بازه عوامل تنبیه و تشویق سازمان")]
public class OrganisationAgentOfPunishmentEncourageScoreIntervalController : AppBaseController
{
    private readonly OrganisationAgentOfPunishmentEncourageScoreIntervalService _OrganisationAgentOfPunishmentEncourageScoreIntervalService;
    public OrganisationAgentOfPunishmentEncourageScoreIntervalController(OrganisationAgentOfPunishmentEncourageScoreIntervalService Service, ILogger<OrganisationAgentOfPunishmentEncourageScoreIntervalController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationAgentOfPunishmentEncourageScoreIntervalService = Service;
        _OrganisationAgentOfPunishmentEncourageScoreIntervalService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageScoreIntervalService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageScoreIntervalService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageScoreIntervalService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationAgentOfPunishmentEncourageScoreIntervalDTO body)
    {
        return Ok(await _OrganisationAgentOfPunishmentEncourageScoreIntervalService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationAgentOfPunishmentEncourageScoreIntervalDTO body)
    {
        var result = await _OrganisationAgentOfPunishmentEncourageScoreIntervalService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationAgentOfPunishmentEncourageScoreIntervalService.DeleteRecord(id));
    }
}
