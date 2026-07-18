using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/PersonnelLeave")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("مرخصی ماهانه کارکنان")]
public class PersonnelLeaveController : AppBaseController
{
    private readonly PersonnelLeaveService _service;
    public PersonnelLeaveController(PersonnelLeaveService service, ILogger<PersonnelLeaveController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _service._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null, [FromQuery] long? PaymentPeriodId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId, PaymentPeriodId));
    }

    [HttpGet, Route("GetPagedTempData/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedTempData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] long? PersonnelFunctionExcelFileId = null)
    {
        return this.AppOk(_service.GetPagedTempData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, PersonnelFunctionExcelFileId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] PersonnelLeaveDTO body)
    {
        body.title = "-";
        body.OrganisationChartId = currentUserDefaultOrganId;
        // PaymentPeriodId is now set by user in the form, not automatically
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] PersonnelLeaveDTO body)
    {
        body.title = "";
        var result = await _service.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}


