using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.WorkFlow;

[Route("api/NodeRoleRel")]
[ControllerGroup("WorkFlow", "گردش کار")]
[DisplayName("ارتباط گره ها با نقش")]
public class NodeRoleRelController : AppBaseController
{
    private readonly NodeRoleRelService _nodeRoleRelService;

    public NodeRoleRelController(NodeRoleRelService service, ILogger<NodeRoleRelController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _nodeRoleRelService = service;
        _nodeRoleRelService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_nodeRoleRelService.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_nodeRoleRelService.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_nodeRoleRelService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] NodeRoleRelDTO body)
    {
        return Ok(await _nodeRoleRelService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] NodeRoleRelDTO body)
    {
        return this.AppOk(await _nodeRoleRelService.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_nodeRoleRelService.DeleteRecord(id));
    }
}
