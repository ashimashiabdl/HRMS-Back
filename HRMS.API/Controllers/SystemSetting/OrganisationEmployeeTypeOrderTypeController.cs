using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationEmployeeTypeOrderType")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("تنظیمات حکم")]
public class OrganisationEmployeeTypeOrderTypeController : AppBaseController
{
    private readonly OrganisationEmployeeTypeOrderTypeService _organisationEmployeeTypeOrderTypeService;
    public OrganisationEmployeeTypeOrderTypeController(OrganisationEmployeeTypeOrderTypeService Service, ILogger<OrganisationEmployeeTypeOrderTypeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _organisationEmployeeTypeOrderTypeService = Service;
        _organisationEmployeeTypeOrderTypeService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsTree/{id?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsTree(int? id = 0)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeService.GetAsTree(id));
    }
    [HttpGet, Route("GetAsTreeByOrganisationChartId/{organisationChartId}/{employeeTypeId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsTreeByOrganisationChartId(long organisationChartId, int? employeeTypeId = 0)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeService.GetAsTreeByOrganisationChartId(organisationChartId, employeeTypeId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{SelectedEmployeeType?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? SelectedEmployeeTypeId = null)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeOrderTypeDTO body)
    {
        return Ok(await _organisationEmployeeTypeOrderTypeService.CreateForAsync(body));

    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeOrderTypeDTO body)
    {
        var result = await _organisationEmployeeTypeOrderTypeService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeService.DeleteRecord(id));
    }
}
