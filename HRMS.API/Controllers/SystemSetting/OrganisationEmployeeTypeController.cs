using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
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

[Route("api/OrganisationEmployeeType")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("نوع استخدام های سازمان")]
public class OrganisationEmployeeTypeController : AppBaseController
{
    private readonly OrganisationEmployeeTypeService _organisationEmployeeTypeService;
    public OrganisationEmployeeTypeController(OrganisationEmployeeTypeService Service, ILogger<OrganisationEmployeeTypeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _organisationEmployeeTypeService = Service;
        _organisationEmployeeTypeService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_organisationEmployeeTypeService.GetAsKeyValuePair());
    }
    [HttpGet, Route("GetAsTree/{id?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsTree(int? id = 0)
    {
        return this.AppOk(_organisationEmployeeTypeService.GetAsTree(id));
    }
    [HttpGet, Route("GetAsTreeByOrganisationChartId/{organisationChartId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsTreeByOrganisationChartId(long organisationChartId)
    {
        return this.AppOk(_organisationEmployeeTypeService.GetAsTreeByOrganisationChartId(organisationChartId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_organisationEmployeeTypeService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_organisationEmployeeTypeService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeDTO body)
    {
        return Ok(await _organisationEmployeeTypeService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeDTO body)
    {
        var result = await _organisationEmployeeTypeService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_organisationEmployeeTypeService.DeleteRecord(id));
    }
}
