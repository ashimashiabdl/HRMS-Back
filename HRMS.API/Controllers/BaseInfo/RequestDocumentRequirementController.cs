using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/RequestDocumentRequirement")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("الزامات اسناد درخواست")]
public class RequestDocumentRequirementController : AppBaseController
{
    private readonly RequestDocumentRequirementService _service;

    public RequestDocumentRequirementController(
        RequestDocumentRequirementService service,
        ILogger<RequestDocumentRequirementController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair() => this.AppOk(_service.GetAsKeyValuePair());

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id) => this.AppOk(_service.Get(id));

    [HttpGet, Route("GetWithDetails/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetWithDetails(long id) => this.AppOk(_service.GetWithDetails(id));

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] RequestDocumentRequirementDTO body)
        => this.AppOk(await _service.CreateForAsync(body));

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] RequestDocumentRequirementDTO body)
        => this.AppOk(await _service.UpdateForAsync(body));

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id) => this.AppOk(_service.DeleteRecord(id));
}
