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
using HRMS.API.Controllers.Employee;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/BankAccount")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("حساب های بانکی")]
[EmployeeAccessCheck]
public class BankAccountController : AppBaseController
{
    private readonly BankAccountService _BankAccountService;
    private readonly EmployeeService _EmployeeService;

    public BankAccountController(IConfiguration configuration, BankAccountService Service, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _BankAccountService = Service;
        _EmployeeService = EmployeeService;
        _BankAccountService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_BankAccountService.Get(id));
    }
    [HttpGet, Route("GetAsKeyValuePairValue/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePairValue(long id)
    {
        return this.AppOk(_BankAccountService.GetAsKeyValuePair(id));
    }

    [HttpGet, Route("GetDefaultAccountNumber/{employeeId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetDefaultAccountNumber(long employeeId)
    {
        return this.AppOk(_BankAccountService.GetDefaultAccountNumber(employeeId));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] BankAccountDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        return Ok(await _BankAccountService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] BankAccountDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        var result = await _BankAccountService.UpdateForAsync(body);
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
        return this.AppOk(_BankAccountService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: EmployeeId));

    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_BankAccountService.DeleteRecord(id));
    }
}
