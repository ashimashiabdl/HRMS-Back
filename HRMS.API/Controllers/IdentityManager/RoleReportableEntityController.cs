using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/RoleReportableEntity")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("موجودیت قابل گزارش نقش")]
public class RoleReportableEntityController : AppBaseController
{
    private readonly RoleReportableEntityService _service;
    public RoleReportableEntityController(RoleReportableEntityService service, ILogger<RoleReportableEntityController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
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
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? RoleId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, RoleId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] RoleReportableEntityDTO body)
    {
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] RoleReportableEntityDTO body)
    {
        var result = await _service.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}

