using HR.SharedKernel.Attribute;
using HR.SharedKernel.Service;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using HRMS.API.Cache;
using System.ComponentModel;
using HR.SharedKernel.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/PaymentPeriodEmployeeBonus")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("کارانه به ازای دوره کارمند")]
public class PaymentPeriodEmployeeBonusController : AppBaseController
{
    private readonly PaymentPeriodEmployeeBonusService _PaymentPeriodEmployeeBonusService;
    public PaymentPeriodEmployeeBonusController(PaymentPeriodEmployeeBonusService Service, ILogger<PaymentPeriodEmployeeBonusController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _PaymentPeriodEmployeeBonusService = Service;
        _PaymentPeriodEmployeeBonusService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_PaymentPeriodEmployeeBonusService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (EmployeeId > 0)
        {
            var filtered = _PaymentPeriodEmployeeBonusService.All()
                .Include(i => i.PaymentPeriod)
                .Include(i => i.Coefficient)
                .Where(i => i.EmployeeId == EmployeeId);
            return this.AppOk(_PaymentPeriodEmployeeBonusService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, null, EmployeeId, CustomDataSource: filtered));

        }
        else
        {
            return this.AppNotFound();
        }
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] PaymentPeriodEmployeeBonusDTO body)
    {
        if (string.IsNullOrEmpty(body.title))
        {
            body.title = "";
        }
   

        if (_PaymentPeriodEmployeeBonusService.All().Any(i => i.EmployeeId == body.EmployeeId && i.CoefficientId == body.CoefficientId && i.PaymentPeriodId == body.PaymentPeriodId))
        {
            return BadRequest(OperationResult.Failed("در دوره انتخابی رکورد مورد نظر وجود دارد"));
        }

        return Ok(await _PaymentPeriodEmployeeBonusService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] PaymentPeriodEmployeeBonusDTO body)
    {
        if (string.IsNullOrEmpty(body.title))
        {
            body.title = "";
        }

        if (_PaymentPeriodEmployeeBonusService.All().Any(i => i.Id != body.Id && i.EmployeeId == body.EmployeeId && i.CoefficientId == body.CoefficientId && i.PaymentPeriodId == body.PaymentPeriodId))
        {
            return BadRequest(OperationResult.Failed("در دوره انتخابی رکورد مورد نظر وجود دارد"));
        }
        var result = await _PaymentPeriodEmployeeBonusService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_PaymentPeriodEmployeeBonusService.DeleteRecord(id));
    }
}
