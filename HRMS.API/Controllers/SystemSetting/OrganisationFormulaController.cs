using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.Core.Enums;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Mvc;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationFormula")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("فرمول های سازمان")]
public class OrganisationFormulaController : AppBaseController
{
    private readonly OrganisationFormulaService _organisationFormulaService;

    public OrganisationFormulaController(OrganisationFormulaService Service, ILogger<OrganisationFormulaController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _organisationFormulaService = Service;
        _organisationFormulaService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_organisationFormulaService.GetAsKeyValuePair());
    }

    [HttpGet, Route("GetAsKeyValuePairByUsageLocation/{usageLocationId}/{includeOrganisationFormulaId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByUsageLocation(FormulaUsageLocationId usageLocationId, long includeOrganisationFormulaId = 0)
    {
        return this.AppOk(_organisationFormulaService.GetAsKeyValuePairByUsageLocation(usageLocationId, includeOrganisationFormulaId));
    }

    [HttpGet, Route("GetFormulaUsageLocationsAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetFormulaUsageLocationsAsKeyValuePair()
    {
        return this.AppOk(_organisationFormulaService.GetFormulaUsageLocationsAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_organisationFormulaService.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_organisationFormulaService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationFormulaDTO body)
    {
        return Ok(await _organisationFormulaService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationFormulaDTO body)
    {
        var result = await _organisationFormulaService.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_organisationFormulaService.DeleteRecord(id));
    }
}
