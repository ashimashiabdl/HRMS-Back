using HR.SharedKernel.Attribute;
using AutoMapper;

using HR.Organisation.Core.DTOs;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Organisation;

[Route("api/OrganisationProject")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("پروژه های سازمان")]
public class OrganisationProjectController : AppBaseController
{
    private readonly OrganisationProjectService _OrganisationProjectService;
    public OrganisationProjectController(OrganisationProjectService Service, ILogger<OrganisationProjectController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationProjectService = Service;
        _OrganisationProjectService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpPost, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair([FromBody] SettingRequestDateSensitive id)
    {
        return this.AppOk(_OrganisationProjectService.GetAsKeyValuePair(id.ImpleDate));
    }
    [HttpPost, Route("GetAsKeyValuePairByOrganisationChartId")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByOrganisationChartId([FromBody] SettingRequestDateSensitive request)
    {
        return this.AppOk(_OrganisationProjectService.GetAsKeyValuePairByOrganisationChartId(request.Id, request.ImpleDate));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationProjectService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_OrganisationProjectService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationProjectDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                body.OrganisationChartId = currentUserDefaultOrganId;
                return Ok(await _OrganisationProjectService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] OrganisationProjectDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganisationProjectService.UpdateForAsync(body);
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
        return this.AppOk(_OrganisationProjectService.DeleteRecord(id));
    }
}
