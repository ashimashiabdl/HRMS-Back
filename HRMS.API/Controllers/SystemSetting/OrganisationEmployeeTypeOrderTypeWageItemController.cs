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

[Route("api/OrganisationEmployeeTypeOrderTypeWageItem")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("تنظیمات عوامل حقوقی حکم به نوع استخدام سازمان")]
public class OrganisationEmployeeTypeOrderTypeWageItemController : AppBaseController
{
    private readonly OrganisationEmployeeTypeOrderTypeWageItemService _organisationEmployeeTypeOrderTypeWageItemService;
    public OrganisationEmployeeTypeOrderTypeWageItemController(OrganisationEmployeeTypeOrderTypeWageItemService Service, ILogger<OrganisationEmployeeTypeOrderTypeWageItemController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _organisationEmployeeTypeOrderTypeWageItemService = Service;
        _organisationEmployeeTypeOrderTypeWageItemService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeWageItemService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{SelectedEmployeeType?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? SelectedEmployeeTypeId = null)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeWageItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeOrderTypeWageItemDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _organisationEmployeeTypeOrderTypeWageItemService.CreateForAsync(body));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeOrderTypeWageItemDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _organisationEmployeeTypeOrderTypeWageItemService.UpdateForAsync(body);
                return this.AppOk(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_organisationEmployeeTypeOrderTypeWageItemService.DeleteRecord(id));
    }
}
