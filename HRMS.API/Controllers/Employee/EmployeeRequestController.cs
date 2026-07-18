using AutoMapper;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/EmployeeRequest")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("کارتابل درخواست‌های کارکنان")]
[EmployeeAccessCheck]
public class EmployeeRequestController(
    EmployeeRequestService employeeRequestService,
    ILogger<EmployeeController> logger,
    IHttpContextAccessor accessor,
    IMapper mapper,
    IDapper dapper,
    UserResolverService userResolverService)
    : AppBaseController(userResolverService, logger, accessor, mapper, dapper)
{
    private readonly EmployeeRequestService _employeeRequestService = employeeRequestService;

    [HttpGet("Get/{id:long}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_employeeRequestService.GetForCartable(id, currentUserId));
    }

    [HttpGet("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_employeeRequestService.GetCartablePagedData(
            currentUserId,
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired));
    }

    [HttpGet("DownloadFile/{fileId:long}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult DownloadFile(long fileId)
    {
        return this.AppOk(_employeeRequestService.DownloadFileForCartable(fileId, currentUserId));
    }
}
