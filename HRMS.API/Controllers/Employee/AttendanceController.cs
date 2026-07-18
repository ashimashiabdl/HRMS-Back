using AutoMapper;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.Employee;

[Route("api/Attendance")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("اطلاعات ورود و خروج کارکنان")]
[EmployeeAccessCheck]
public class AttendanceController : AppBaseController
{
    private readonly AttendanceService _AttendanceService;
    private readonly EmployeeService _EmployeeService;

    public AttendanceController(AttendanceService Service, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _AttendanceService = Service;
        _EmployeeService = EmployeeService;
        _AttendanceService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_AttendanceService.Get(id));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] AttendanceDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        return Ok(await _AttendanceService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] AttendanceDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        var result = await _AttendanceService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (EmployeeId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        return this.AppOk(_AttendanceService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: EmployeeId));

    }

}
