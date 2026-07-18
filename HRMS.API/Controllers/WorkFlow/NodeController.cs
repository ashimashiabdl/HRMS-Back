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

[Route("api/Node")]
[ControllerGroup("WorkFlow", "گردش کار")]
[DisplayName("گره ها")]
public class NodeController : AppBaseController
{
    private readonly NodeService _nodeService;
    public NodeController(NodeService Service, ILogger<NodeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _nodeService = Service;
        _nodeService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_nodeService.GetAsKeyValuePair());
    }

    [HttpGet, Route("GetAsKeyValuePairByWorkFlowId/{workFlowId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByWorkFlowId(long workFlowId)
    {
        return this.AppOk(_nodeService.GetAsKeyValuePairByWorkFlowId(workFlowId));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_nodeService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? workFlowTypeId = null)
    {
        if (workFlowTypeId > 0)
        {
            return this.AppOk(_nodeService.GetPagedDataByWorkFlowType(
                workFlowTypeId.Value,
                currentPage,
                pageSize,
                filter,
                activeSortColumn,
                Sortdirection,
                IgnoreExpired));
        }

        return this.AppOk(_nodeService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] NodeDTO body)
    {
        return Ok(await _nodeService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] NodeDTO body)
    {
        return this.AppOk(await _nodeService.UpdateForAsync(body));
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_nodeService.DeleteRecord(id));
    }
}
