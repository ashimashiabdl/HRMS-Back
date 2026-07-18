using HR.SharedKernel.Attribute;
using AutoMapper;

using Hr.Employee.infrastructure.Services;

using HRMS.API.Controllers.SystemSetting;
using HR.BaseInfo.Core.DTOs;
using HR.Employee.Core.DTOs;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HRMS.API.Controllers.Employee;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/GroupPunishmentEncourage")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("تنبیه و تشویق گروهی")]
[EmployeeAccessCheck]
public class GroupPunishmentEncourageController : AppBaseController
{
    private readonly GroupPunishmentEncourageService _GroupPunishmentEncourageService;
    private readonly TempPunishmentEncourageService _tempPunishmentEncourageService;
    private readonly EmployeeService _EmployeeService;
    public GroupPunishmentEncourageController(GroupPunishmentEncourageService Service, TempPunishmentEncourageService TempPunishmentEncourageService, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _GroupPunishmentEncourageService = Service;
        _GroupPunishmentEncourageService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _tempPunishmentEncourageService = TempPunishmentEncourageService;
        _tempPunishmentEncourageService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _EmployeeService = EmployeeService;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_GroupPunishmentEncourageService.Get(id));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] GroupPunishmentEncourageDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        return Ok(await _GroupPunishmentEncourageService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] GroupPunishmentEncourageDTO body)
    {
        var result = await _GroupPunishmentEncourageService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_GroupPunishmentEncourageService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpGet, Route("GetPagedDataTemp/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedDataTemp(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] long? TempFileId = null)
    {
        if (TempFileId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        var datasource = _tempPunishmentEncourageService._unitOfWork.Context.TempPunishmentEncourages.Include(i => i.Employee).Where(i => i.TempFileId == TempFileId);
        return this.AppOk(_tempPunishmentEncourageService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: datasource));
    }
    [HttpGet, Route("GetExcelPreview/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetExcelPreview(int id)
    {
        return this.AppOk(_GroupPunishmentEncourageService.GetExcelPreview(id));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_GroupPunishmentEncourageService.DeleteRecord(id));
    }
}
