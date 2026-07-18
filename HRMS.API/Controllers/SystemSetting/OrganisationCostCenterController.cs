using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationCostCenter")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("مراکز هزینه سازمان")]
public class OrganisationCostCenterController : AppBaseController
{
    private readonly OrganisationCostCenterService _OrganisationCostCenterService;
    public OrganisationCostCenterController(OrganisationCostCenterService Service, ILogger<OrganisationCostCenterController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationCostCenterService = Service;
        _OrganisationCostCenterService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpPost, Route("GetCurrentOrganCostCentersAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentOrganCostCentersAsKeyValuePair([FromBody] SettingRequestDateSensitive id)
    {
        return this.AppOk(_OrganisationCostCenterService.GetCurrentOrganCostCentersAsKeyValuePair(id.ImpleDate));
    }
    [HttpPost, Route("GetCurrentOrganPeymanCostCentersAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentOrganPeymanCostCentersAsKeyValuePair([FromBody] SettingRequestDateSensitive id)
    {
        return this.AppOk(_OrganisationCostCenterService.GetCurrentOrganPeymanCostCentersAsKeyValuePair(id.ImpleDate));
    }
    [HttpPost, Route("GetCostCentersByOrganisationChartId")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetCostCentersByOrganisationChartId([FromBody] SettingRequestDateSensitive request)
    {
        return this.AppOk(_OrganisationCostCenterService.GetCostCentersByOrganisationChartId(request.Id, request.ImpleDate));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationCostCenterService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_OrganisationCostCenterService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationCostCenterDTO body)
    {
        return Ok(await _OrganisationCostCenterService.CreateForAsync(body));

    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationCostCenterDTO body)
    {
        var result = await _OrganisationCostCenterService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationCostCenterService.DeleteRecord(id));
    }
}
