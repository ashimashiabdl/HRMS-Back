using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/OrganisationEmployeeTypeCoefficientBonusWageItem")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("ضرایب/آیتم مزد نوع استخدام (کارانه)")]
public class OrganisationEmployeeTypeCoefficientBonusWageItemController : AppBaseController
{
    private readonly OrganisationEmployeeTypeCoefficientBonusWageItemService _service;
    public OrganisationEmployeeTypeCoefficientBonusWageItemController(OrganisationEmployeeTypeCoefficientBonusWageItemService Service, ILogger<OrganisationEmployeeTypeCoefficientBonusWageItemController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _service = Service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _service._db.Set<OrganisationEmployeeTypeCoefficientBonusWageItem>()
       .Include(i => i.WageItem)
       .Include(i => i.EmployeeType)
       .Include(i => i.Coefficient)
       .Where(DateValidityExtension<OrganisationEmployeeTypeCoefficientBonusWageItem>.GetDateValidationPredicate(IgnoreExpired).And(i => i.OrganisationChartId == currentUserDefaultOrganId))
        ;
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeCoefficientBonusWageItemDTO body)
    {
        body.title = "";
        return Ok(await _service.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeCoefficientBonusWageItemDTO body)
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


