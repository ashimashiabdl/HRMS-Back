using AutoMapper;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.Organisation;

[Route("api/OrganisationPosition")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("پست های سازمان")]
public class OrganisationPositionController : AppBaseController
{
    private readonly OrganisationPositionService _OrganisationPositionService;

    public OrganisationPositionController(
        OrganisationPositionService Service,
        ILogger<OrganisationPositionController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService UserResolverService)
        : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationPositionService = Service;
        _OrganisationPositionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_OrganisationPositionService.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationPositionService.Get(id));
    }

    [HttpPost, Route("GetChartNodePositions/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetChartNodePositions([FromBody] SettingRequestDateSensitive id)
    {
        return this.AppOk(_OrganisationPositionService.GetChartNodePositions(id.Id, id.ImpleDate));
    }

    [HttpGet, Route("GetRelatedNodeIds")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetRelatedNodeIds()
    {
        return this.AppOk(_OrganisationPositionService.GetRelatedNodeIds());
    }

    [HttpGet, Route("GetByRelatedNodeId/{relatedNodeId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetByRelatedNodeId(long relatedNodeId, [FromQuery] DateTime? impleDate = null)
    {
        return this.AppOk(_OrganisationPositionService.GetByRelatedNodeId(relatedNodeId, impleDate));
    }

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
        return this.AppOk(_OrganisationPositionService.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationPositionDTO body)
    {
        return Ok(await _OrganisationPositionService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationPositionDTO body)
    {
        return this.AppOk(await _OrganisationPositionService.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationPositionService.DeleteRecord(id));
    }
}
