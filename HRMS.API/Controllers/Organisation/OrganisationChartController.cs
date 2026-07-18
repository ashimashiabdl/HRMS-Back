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
using Microsoft.AspNetCore.Authorization;
using HRMS.API.Cache;
using System.ComponentModel;
using System.Threading.Tasks;
using HR.Organisation.Core.Entities;

namespace HRMS.API.Controllers.Organisation;

[Route("api/OrganisationChart")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("چارت سازمانی")]
public class OrganisationChartController : AppBaseController
{
    private readonly OrganisationChartService _OrganisationChartService;
    public OrganisationChartController(OrganisationChartService Service, ILogger<OrganisationChartController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationChartService = Service;
        _OrganisationChartService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetcurrentUserDefaultOrganId")]
    [CustomAccessKey(AccessKey: "GetcurrentUserDefaultOrganId")]
    public async Task<IActionResult> GetcurrentUserDefaultOrganId()
    {
        if (currentUserDefaultOrganId > 0)
        {
            return this.AppOk(OperationResult.Succeeded(payload: (await _OrganisationChartService._db.Set<OrganisationChart>().FindAsync(currentUserDefaultOrganId)).title));
        }
        else
        {
            return this.AppOk(OperationResult.Succeeded(payload: "نا مشخص"));
        }
    }
    [HttpGet, Route("GetcurrentUserDefaultOrganIdValue")]
    [CustomAccessKey(AccessKey: "GetcurrentUserDefaultOrganIdValue")]
    public IActionResult GetcurrentUserDefaultOrganIdValue()
    {
        return this.AppOk(OperationResult.Succeeded(payload: currentUserDefaultOrganId));
    }
    [HttpGet, Route("GetCurrentNodePath/{id?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentNodePath(long? id = 0)
    {
        return this.AppOk(OperationResult.Succeeded(payload: _OrganisationChartService.GetCurrentNodePath(id)));
    }

    [HttpGet, Route("GetAsTree/{id?}")]
    [CustomAccessKey(AccessKey: "GetAsTree")]

    public IActionResult GetAsTree(int? id = 0)
    {
        return this.AppOk(_OrganisationChartService.GetAsTree(id));
    }

    [HttpGet, Route("GetAsTreeFromCurrentUserDefaultOrgan")]
    [CustomAccessKey(AccessKey: "GetAsTreeFromCurrentUserDefaultOrgan")]

    public IActionResult GetAsTreeFromCurrentUserDefaultOrgan()
    {
        if (currentUserDefaultOrganId <= 0)
        {
            return this.AppOk(OperationResult.Failed("کاربر سازمان پیش‌فرض ندارد"));
        }
        return this.AppOk(_OrganisationChartService.GetAsTreeFromRoot(currentUserDefaultOrganId));
    }

    [HttpGet, Route("GetAsTreeFromRoot/{rootId}")]
    [CustomAccessKey(AccessKey: "GetAsTreeFromCurrentUserDefaultOrgan")]
    public IActionResult GetAsTreeFromRoot(long rootId)
    {
        return this.AppOk(_OrganisationChartService.GetAsTreeFromRoot(rootId));
    }

    [HttpGet, Route("GetAsTreePrimeng/{id?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsTreePrimeng(int? id = 0)
    {
        return this.AppOk(_OrganisationChartService.GetAsTreePrimeng(id));
    }

    [HttpGet, Route("GetAllAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAllAsKeyValuePair(int? id = 0)
    {
        return this.AppOk(_OrganisationChartService.GetAllAsKeyValuePair());
    }
    [HttpPost, Route("GetAllPayLocationsAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "GetAllPayLocationsAsKeyValuePair")]

    public IActionResult GetAllPayLocationsAsKeyValuePair([FromBody] SettingRequestDateSensitive id)
    {
        return this.AppOk(_OrganisationChartService.GetAllPayLocationsAsKeyValuePair(id.ImpleDate));

    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationChartService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_OrganisationChartService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationChartDTO body)
    {
        return Ok(await _OrganisationChartService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationChartDTO body)
    {
        var result = await _OrganisationChartService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpPut("UpdateNodeParent")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> UpdateNodeParent([FromBody] OrganisationChartDTO body)
    {

        var result = await _OrganisationChartService.UpdateNodeParent(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationChartService.DeleteRecord(id));
    }
}
