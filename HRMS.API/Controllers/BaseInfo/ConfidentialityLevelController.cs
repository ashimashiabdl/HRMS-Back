using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/ConfidentialityLevel")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("سطح محرمانگی")]
public class ConfidentialityLevelController(ConfidentialityLevelService Service, ILogger<ConfidentialityLevelController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly ConfidentialityLevelService _service = Service;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id) => this.AppOk(_service.Get(id));

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] ConfidentialityLevelDTO body) => this.AppOk(await _service.CreateForAsync(body));

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] ConfidentialityLevelDTO body) => this.AppOk(await _service.UpdateForAsync(body));

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetAsKeyValuePair() => this.AppOk(await Task.FromResult(_service.GetAsKeyValuePair()));
}


