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

[Route("api/OrganisationPositionJob")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("شغل های پست")]
public class OrganisationPositionJobController : AppBaseController
{
    private readonly OrganisationPositionJobService _OrganisationPositionJobService;
    private OrganisationContext _context;
    public OrganisationPositionJobController(OrganisationContext context, OrganisationPositionJobService Service, ILogger<OrganisationPositionJobController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _OrganisationPositionJobService = Service;
        _OrganisationPositionJobService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_OrganisationPositionJobService.GetAsKeyValuePair());
    }
    [HttpGet, Route("GetCurrentPositionJobs/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetCurrentPositionJobs(long id)
    {
        return this.AppOk(_OrganisationPositionJobService.GetCurrentPositionJobs(id));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationPositionJobService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _context.OrganisationPositionJobs
               .Include(i => i.OrganizationJob)
               .Include(i => i.OrganizationJob.Job)
              .Include(i => i.OrganisationPosition)
              .Include(i => i.OrganisationPosition.Position)
              .Where(DateValidityExtension<OrganisationPositionJob>.GetDateValidationPredicate().And(i => i.OrganisationChartId == currentUserDefaultOrganId));
        return this.AppOk(_OrganisationPositionJobService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationPositionJobDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _OrganisationPositionJobService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] OrganisationPositionJobDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganisationPositionJobService.UpdateForAsync(body);
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
        return this.AppOk(_OrganisationPositionJobService.DeleteRecord(id));
    }
}
