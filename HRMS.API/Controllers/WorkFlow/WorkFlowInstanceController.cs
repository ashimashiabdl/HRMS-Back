using HR.SharedKernel.Attribute;
using AutoMapper;

using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using Microsoft.AspNetCore.Http.Timeouts;

namespace HRMS.API.Controllers.WorkFlow;

[Route("api/WorkFlowInstance")]
[ControllerGroup("WorkFlow", "گردش کار")]
[DisplayName("نمونه گردش کار")]
public class WorkFlowInstanceController : AppBaseController
{
    private readonly WorkFlowInstanceService _WorkFlowInstanceService;
    private readonly DefinitionService _definitionService;

    public WorkFlowInstanceController(
        WorkFlowInstanceService Service,
        DefinitionService definitionService,
        ILogger<WorkFlowInstanceController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _WorkFlowInstanceService = Service;
        _definitionService = definitionService;
        _WorkFlowInstanceService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_WorkFlowInstanceService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_WorkFlowInstanceService.Get(id));
    }
    [HttpGet, Route("DoActionOnInstance/{id}/{id1}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult DoActionOnInstance(int id, int id1, [FromQuery] string? comment = null, [FromQuery] long? userSignatureId = null)
    {
        return this.AppOk(_WorkFlowInstanceService.DoActionOnInstance(id, id1, comment, userSignatureId));
    }

    [HttpPost, Route("DoActionOnInstance")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult DoActionOnInstancePost([FromBody] WorkFlowInstanceActionRequestDto body)
    {
        return this.AppOk(_WorkFlowInstanceService.DoActionOnInstance(
            body.ActionId,
            body.InstanceId,
            body.Comment,
            body.UserSignatureId));
    }

    [HttpPost, Route("DoActionOnInstancesBatch")]
    [CustomAccessKey(AccessKey: "batchAction")]
    [RequestTimeout(600_000)]
    public IActionResult DoActionOnInstancesBatch([FromBody] WorkFlowInstanceBatchActionRequestDto body)
    {
        return this.AppOk(_WorkFlowInstanceService.DoActionOnInstancesBatch(body));
    }
    [HttpGet, Route("GetDefinitionsByInstanceId/{instanceId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetDefinitionsByInstanceId(long instanceId)
    {
        _definitionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        return this.AppOk(_definitionService.GetDefinitionsByInstanceId(instanceId));
    }

    [HttpGet, Route("GetCartable")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetCartable(
        [FromQuery] string? filter = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] long? workFlowTypeId = null)
    {
        var result = _WorkFlowInstanceService.GetCartable(currentUserId, filter, fromDate, toDate, workFlowTypeId);
        return this.AppOk(result);
    }

    [HttpGet, Route("GetCartableArchive")]
    [CustomAccessKey(AccessKey: "GetCartableArchive")]
    public IActionResult GetCartableArchive(
        [FromQuery] string? filter = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = _WorkFlowInstanceService.GetCartableArchive(currentUserId, filter, fromDate, toDate);
        return this.AppOk(result);
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_WorkFlowInstanceService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] WorkFlowInstanceDTO body)
    {
        return Ok(await _WorkFlowInstanceService.CreateForAsync(body));

    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] WorkFlowInstanceDTO body)
    {
        var result = await _WorkFlowInstanceService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_WorkFlowInstanceService.DeleteRecord(id));
    }
}
