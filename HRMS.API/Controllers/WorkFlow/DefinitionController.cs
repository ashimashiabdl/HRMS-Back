using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.WorkFlow;

[Route("api/Definition")]
[ControllerGroup("WorkFlow", "گردش کار")]
[DisplayName("تعریف گردش کار ها")]
public class DefinitionController : AppBaseController
{
    private readonly DefinitionService _DefinitionService;
    public DefinitionController(DefinitionService Service, ILogger<DefinitionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _DefinitionService = Service;
        _DefinitionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_DefinitionService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_DefinitionService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? workFlowTypeId = null)
    {
        if (workFlowTypeId > 0)
        {
            return this.AppOk(_DefinitionService.GetPagedDataByWorkFlowType(
                workFlowTypeId.Value,
                currentPage,
                pageSize,
                filter,
                activeSortColumn,
                Sortdirection,
                IgnoreExpired));
        }

        return this.AppOk(_DefinitionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpGet, Route("GetDiagram/{workFlowTypeId}/{workFlowId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetDiagram(long workFlowTypeId, long? workFlowId = null)
    {
        return this.AppOk(_DefinitionService.GetDiagram(workFlowTypeId, workFlowId));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] DefinitionDTO? body)
    {
        if (body == null)
        {
            return this.AppBadRequest("بدنه درخواست خالی است. اطلاعات مسیر گردش کار را با Content-Type: application/json ارسال کنید.");
        }

        return Ok(await _DefinitionService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] DefinitionDTO? body)
    {
        if (body == null)
        {
            return this.AppBadRequest("بدنه درخواست خالی است. اطلاعات مسیر گردش کار را با Content-Type: application/json ارسال کنید.");
        }

        return this.AppOk(await _DefinitionService.UpdateForAsync(body));
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_DefinitionService.DeleteRecord(id));
    }
    [HttpGet, Route("GetDefinitionsByInstanceId/{instanceId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetDefinitionsByInstanceId(long instanceId)
    {
        return this.AppOk(_DefinitionService.GetDefinitionsByInstanceId(instanceId));
    }
}
