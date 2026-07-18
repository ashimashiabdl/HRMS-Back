using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.Organisation.Infrastructure.Services;
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

namespace HRMS.API.Controllers.Organisation;

[Route("api/OrganisationJobSkillYearSetting")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("تنظیمات سال مهارت شغل")]
public class OrganisationJobSkillYearSettingController : AppBaseController
{
    private readonly OrganisationJobSkillYearSettingService _OrganisationJobSkillYearSettingService;
    private OrganisationContext _context;
    public OrganisationJobSkillYearSettingController(OrganisationContext context, OrganisationJobSkillYearSettingService Service, ILogger<OrganisationJobSkillYearSettingController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _OrganisationJobSkillYearSettingService = Service;
        _OrganisationJobSkillYearSettingService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationJobSkillYearSettingService.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _context.OrganisationJobSkillYearSettings
            .Include(i => i.OrganizationJob)
            .ThenInclude(j => j.Job)
            .Include(i => i.SkillLevel)
            .Where(DateValidityExtension<OrganisationJobSkillYearSetting>.GetDateValidationPredicate().And(i => i.OrganisationChartId == currentUserDefaultOrganId));
        
        return this.AppOk(_OrganisationJobSkillYearSettingService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationJobSkillYearSettingDTO body)
    {
        body.title = "";
        return Ok(await _OrganisationJobSkillYearSettingService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationJobSkillYearSettingDTO body)
    {
        body.title = "";
        var result = await _OrganisationJobSkillYearSettingService.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationJobSkillYearSettingService.DeleteRecord(id));
    }
}

