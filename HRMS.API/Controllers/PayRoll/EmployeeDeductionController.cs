using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/EmployeeDeduction")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("کسور کارکنان")]
public class EmployeeDeductionController : AppBaseController
{
    private readonly EmployeeDeductionService _service;
    private readonly FicheItemService _ficheItemService;
    public EmployeeDeductionController(EmployeeDeductionService Service, FicheItemService FicheItemService, ILogger<EmployeeDeductionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _service = Service;
        _ficheItemService = FicheItemService;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_service.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long EmployeeId = 0, [FromQuery] long? DeductionTypeId = null)
    {
        if (EmployeeId <= 0)
        {
            return this.AppBadRequest("EmployeeId is required");
        }

        IQueryable<HR.Payroll.Core.Data.EmployeeDeduction> all = _service.All(IgnoreExpired)
            .Where(i => i.EmployeeId == EmployeeId)
            .Include(i => i.DeductionType);

        if (DeductionTypeId.HasValue && DeductionTypeId.Value > 0)
        {
            all = all.Where(i => i.DeductionTypeId == DeductionTypeId.Value);
        }
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: all));
    }

    [HttpGet, Route("getDeductionCartexWithServersidePaging/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult getDeductionCartexWithServersidePaging(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] long EmployeeId = 0, [FromQuery] long EmployeeDeductionId = 0)
    {
        if (EmployeeDeductionId > 0 && EmployeeId > 0)
        {
            var filtered = _ficheItemService.All()
                .Include(i => i.WageItem)
                .Include(i => i.Fiche)
                .Include(i => i.Fiche.PaymentPeriod)
                .Where(i => i.EmployeeDeductionId == EmployeeDeductionId)
                .OrderByDescending(i => i.Fiche.PaymentPeriod.ShamsiYear)
                .ThenByDescending(i => i.Fiche.PaymentPeriod.ShamsiMonth);

            return this.AppOk(_ficheItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filtered));
        }
        return this.AppBadRequest("Internal Server Error");
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] EmployeeDeductionDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        body.title = "";
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] EmployeeDeductionDTO body)
    {
        body.title = "";
        body.OrganisationChartId = currentUserDefaultOrganId;

        var result = await _service.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }

    [HttpDelete("DeletePhysical/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult DeletePhysical(int id)
    {
        return this.AppOk(_service.PhysicalDelete(id));
    }
}


