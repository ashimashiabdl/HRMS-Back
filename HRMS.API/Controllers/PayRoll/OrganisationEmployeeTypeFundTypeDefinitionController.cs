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

namespace HRMS.API.Controllers.PayRoll;

[Route("api/OrganisationEmployeeTypeFundTypeDefinition")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("تعریف صندوق نوع استخدام")]
public class OrganisationEmployeeTypeFundTypeDefinitionController : AppBaseController
{
    private readonly OrganisationEmployeeTypeFundTypeDefinitionService _OrganisationEmployeeTypeFundTypeDefinitionService;
    public OrganisationEmployeeTypeFundTypeDefinitionController(OrganisationEmployeeTypeFundTypeDefinitionService Service, ILogger<OrganisationEmployeeTypeFundTypeDefinitionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganisationEmployeeTypeFundTypeDefinitionService = Service;
        _OrganisationEmployeeTypeFundTypeDefinitionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _OrganisationEmployeeTypeFundTypeDefinitionService._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationEmployeeTypeFundTypeDefinitionService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? FundTypeId = null)
    {
        return this.AppOk(_OrganisationEmployeeTypeFundTypeDefinitionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, FundTypeId));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeFundTypeDefinitionDTO body)
    {
        return Ok(await _OrganisationEmployeeTypeFundTypeDefinitionService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeFundTypeDefinitionDTO body)
    {
        var result = await _OrganisationEmployeeTypeFundTypeDefinitionService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationEmployeeTypeFundTypeDefinitionService.DeleteRecord(id));
    }
}

