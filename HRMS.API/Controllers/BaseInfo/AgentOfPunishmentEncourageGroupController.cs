using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.Core.Interfaces;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/AgentOfPunishmentEncourageGroup")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("گروه عوامل تنبیه و تشویق")]
public class AgentOfPunishmentEncourageGroupController : AppBaseController
{
    private readonly AgentOfPunishmentEncourageGroupService _AgentOfPunishmentEncourageGroupService;
    public AgentOfPunishmentEncourageGroupController(AgentOfPunishmentEncourageGroupService Service, ILogger<AgentOfPunishmentEncourageGroupController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _AgentOfPunishmentEncourageGroupService = Service;
        _AgentOfPunishmentEncourageGroupService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_AgentOfPunishmentEncourageGroupService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_AgentOfPunishmentEncourageGroupService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_AgentOfPunishmentEncourageGroupService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] AgentOfPunishmentEncourageGroupDTO body)
    {
        return Ok(await _AgentOfPunishmentEncourageGroupService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] AgentOfPunishmentEncourageGroupDTO body)
    {
        var result = await _AgentOfPunishmentEncourageGroupService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(OperationResult.Failed("امکان حذف جدول پایه سیستم وجود ندارد، لطفا با مدیر تماس بگیرید", payload: id));
    }
}
