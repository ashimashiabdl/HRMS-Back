using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.Employee.infrastructure.Services;
using HR.Employee.Core.DTOs;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.Employee;

[Route("api/Experience")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("سابقه")]
[EmployeeAccessCheck]
public class ExperienceController : AppBaseController
{
    private readonly ExperienceService _experienceService;
    private readonly EmployeeService _employeeService;

    public ExperienceController(ExperienceService service, EmployeeService employeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _experienceService = service;
        _employeeService = employeeService;
        _experienceService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_experienceService.Get(id));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] ExperienceDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        return Ok(await _experienceService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] ExperienceDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        var result = await _experienceService.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (!(EmployeeId > 0))
        {
            return this.AppNotFound();
        }
        return this.AppOk(_experienceService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: EmployeeId));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_experienceService.DeleteRecord(id));
    }
}


