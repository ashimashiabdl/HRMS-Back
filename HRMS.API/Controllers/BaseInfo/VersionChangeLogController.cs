using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/VersionChangeLog")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("تغییرات نسخه")]
public class VersionChangeLogController(VersionChangeLogService service, ILogger<VersionChangeLogController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService)
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly VersionChangeLogService _service = service;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id) => this.AppOk(_service.Get(id));

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpGet, Route("GetByVersionId/{versionId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetByVersionId(long versionId)
    {
        return this.AppOk(_service.GetByVersionId(versionId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] VersionChangeLogDTO body) => this.AppOk(await _service.CreateForAsync(body));

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] VersionChangeLogDTO body) => this.AppOk(await _service.UpdateForAsync(body));

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}
