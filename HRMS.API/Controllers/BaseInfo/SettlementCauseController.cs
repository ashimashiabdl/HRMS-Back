using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/SettlementCause")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("علت تسویه حساب")]
public class SettlementCauseController : AppBaseController
{
    private readonly SettlementCauseService _service;
    public SettlementCauseController(SettlementCauseService service, ILogger<SettlementCauseController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_service.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] SettlementCauseDTO body)
    {
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] SettlementCauseDTO body)
    {
        return this.AppOk(await _service.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}
