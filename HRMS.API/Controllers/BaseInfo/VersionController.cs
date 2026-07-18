using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/Version")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("مدیریت نسخه‌ها")]
public class VersionController(VersionService service, ILogger<VersionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) 
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly VersionService _service = service;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id) => this.AppOk(_service.Get(id));

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpGet, Route("GetFiltered")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetFiltered([FromQuery] string? status = null, [FromQuery] string? releaseType = null, [FromQuery] string? environment = null)
    {
        return this.AppOk(_service.GetFilteredVersions(status, releaseType, environment));
    }

    [HttpGet, Route("GetLatestReleased")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetLatestReleased()
    {
        return this.AppOk(_service.GetLatestReleasedVersion());
    }

    [HttpGet, Route("GetVersionWithChangeLog/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetVersionWithChangeLog(long id)
    {
        return this.AppOk(_service.GetVersionWithChangeLog(id));
    }

    [HttpGet, Route("GetCurrentSystemVersion")]
    [AllowAnonymous]
    public IActionResult GetCurrentSystemVersion()
    {
        return this.AppOk(_service.GetCurrentSystemVersion());
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] VersionDTO body) => this.AppOk(await _service.CreateForAsync(body));

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] VersionDTO body) => this.AppOk(await _service.UpdateForAsync(body));

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}
