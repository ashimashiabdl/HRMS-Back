using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.Entities;
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
using System.Collections.ObjectModel;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Organisation;

[Route("api/OrganizationJob")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("مشاغل سازمان")]
public class OrganizationJobController : AppBaseController
{
    private readonly OrganizationJobService _OrganizationJobService;
    private OrganisationContext _context;
    public OrganizationJobController(OrganisationContext context, OrganizationJobService Service, ILogger<OrganizationJobController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _OrganizationJobService = Service;
        _OrganizationJobService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }


    [HttpGet, Route("GetAsKeyValuePair/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair(int id = 0)
    {

        return this.AppOk(_OrganizationJobService.GetAsKeyValuePair(id));
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {

        return this.AppOk(_OrganizationJobService.GetAsKeyValuePair(currentUserDefaultOrganId));
    }
    [HttpPost, Route("GetAsKeyValuePairByOrganisationChartId")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByOrganisationChartId([FromBody] SettingRequestDateSensitive request)
    {
        return this.AppOk(_OrganizationJobService.GetAsKeyValuePairByOrganisationChartId(request.Id));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganizationJobService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {

        var Filtered = _context.OrganizationJobs
       .Include(i => i.Job)
       .Include(i => i.JobNature)
       .Include(i => i.StaffingRule)
       .Include(i => i.OrganisationJobCategory)
       .Include(i => i.OrganisationJobCategory.JobCategory)
       .Include(i => i.OrganisationJobGroup)
       .Include(i => i.OrganisationJobGroup.JobGroup)
       .Include(i => i.OrganisationJobSeries)
       .Include(i => i.OrganisationJobSeries.JobSeries)
       .Include(i => i.MaxEducationGrade)
       .Include(i => i.ProcessArea)
       .Where(DateValidityExtension<OrganizationJob>.GetDateValidationPredicate().And(i => i.OrganisationChartId == currentUserDefaultOrganId))
        ;
        return this.AppOk(_OrganizationJobService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganizationJobDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _OrganizationJobService.CreateForAsync(body));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganizationJobDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganizationJobService.UpdateForAsync(body);
                return this.AppOk(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganizationJobService.DeleteRecord(id));
    }
}
