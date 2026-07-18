using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Infrastructure.Services;
using HRMS.API.Cache;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.Attendance;

[Route("api/EmployeeAttendanceException")]
[ControllerGroup("Attendance", "حضور و غیاب")]
[DisplayName("استثناء عدم حضور کارمند")]
public class EmployeeAttendanceExceptionController : AppBaseController
{
    private readonly EmployeeAttendanceExceptionService _service;

    public EmployeeAttendanceExceptionController(
        EmployeeAttendanceExceptionService service,
        ILogger<EmployeeAttendanceExceptionController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair([FromQuery] long EmployeeId = 0)
    {
        return this.AppOk(_service.GetAsKeyValuePair(EmployeeId > 0 ? EmployeeId : null));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true,
        [FromQuery] long EmployeeId = 0)
    {
        return this.AppOk(_service.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            EmployeeId: EmployeeId > 0 ? EmployeeId : null));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] EmployeeAttendanceExceptionDTO body)
    {
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] EmployeeAttendanceExceptionDTO body)
    {
        return this.AppOk(await _service.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}
