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

[Route("api/OrganisationEmployeeTypeWageItem")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("تنظیمات عوامل حقوقی نوع استخدام سازمان")]
public class OrganisationEmployeeTypeWageItemController : AppBaseController
{
    private readonly OrganisationEmployeeTypeWageItemService _organisationEmployeeTypeWageItemService;
    public OrganisationEmployeeTypeWageItemController(OrganisationEmployeeTypeWageItemService Service, ILogger<OrganisationEmployeeTypeWageItemController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _organisationEmployeeTypeWageItemService = Service;
        _organisationEmployeeTypeWageItemService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_organisationEmployeeTypeWageItemService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{SelectedEmployeeType?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? SelectedEmployeeTypeId = null)
    {
        try
        {
            return this.AppOk(_organisationEmployeeTypeWageItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId));

        }
        catch (Exception sdsdsds)
        {
            throw;
        }

    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeWageItemDTO body)
    {
        return Ok(await _organisationEmployeeTypeWageItemService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeWageItemDTO body)
    {
        var result = await _organisationEmployeeTypeWageItemService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_organisationEmployeeTypeWageItemService.DeleteRecord(id));
    }
}
