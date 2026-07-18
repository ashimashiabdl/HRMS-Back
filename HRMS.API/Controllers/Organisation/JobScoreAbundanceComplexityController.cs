using HR.SharedKernel.Attribute;
using AutoMapper;

using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
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

[Route("api/JobScoreAbundanceComplexity")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("تعریف امتیاز شغل ( پیچیدگی / فراوانی )")]
public class JobScoreAbundanceComplexityController : AppBaseController
{
    private readonly JobScoreAbundanceComplexityService _JobScoreAbundanceComplexityService;
    public JobScoreAbundanceComplexityController(JobScoreAbundanceComplexityService Service, ILogger<JobScoreAbundanceComplexityController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _JobScoreAbundanceComplexityService = Service;
        _JobScoreAbundanceComplexityService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.Get(id));
    }

    [HttpGet, Route("Get/{id}/{id1}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id, int id1)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.Get(id, id1));
    }
    [HttpGet, Route("GetRowForQuestionScoreDisplay/{OrganizationJobId}/{JobScoringFactorId}/{ComplexityId}/{AbundanceId}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetRowForQuestionScoreDisplay(long OrganizationJobId, long JobScoringFactorId, long ComplexityId, long AbundanceId)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.GetRowForQuestionScoreDisplay(OrganizationJobId, JobScoringFactorId, ComplexityId, AbundanceId));
    }
    [HttpGet, Route("GetJobScoreSummation/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetJobScoreSummation(int id, [FromQuery] bool ignoreExpired = true)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.GetJobScoreSummation(id, ignoreExpired));
    }
    [HttpGet, Route("GetJobScoreRange/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetJobScoreRange(int id, [FromQuery] bool ignoreExpired = true)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.GetJobScoreRange(id, ignoreExpired));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{OrganizationJobId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? OrganizationJobId = null, [FromQuery] long? JobScoringFactorId = null)
    {
        if (OrganizationJobId == 0)
        {
            OrganizationJobId = null;
        }
        if (JobScoringFactorId == 0)
        {
            JobScoringFactorId = null;
        }
        var Filtered = _JobScoreAbundanceComplexityService._db.Set<JobScoreAbundanceComplexity>()
        .Include(i => i.OrganizationJob)
        .Include(i => i.OrganizationJob.Job)
        .Include(i => i.JobScoringFactor.Group)
        .Where(DateValidityExtension<JobScoreAbundanceComplexity>.GetDateValidationPredicate()
        .And(i => i.OrganizationJob.OrganisationChartId == currentUserDefaultOrganId)
        .And(i => (OrganizationJobId == null) || OrganizationJobId == i.OrganizationJobId)
        .And(i => (JobScoringFactorId == null) || JobScoringFactorId == i.JobScoringFactorId)
        )
            ;
        return this.AppOk(_JobScoreAbundanceComplexityService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] JobScoreAbundanceComplexityDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _JobScoreAbundanceComplexityService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] JobScoreAbundanceComplexityDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _JobScoreAbundanceComplexityService.UpdateForAsync(body);
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
    [HttpPut("UpdateSelectedItem")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult UpdateSelectedItem([FromBody] JobScoreAbundanceComplexityDTO body)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.UpdateSelectedItem(body, body.IsForQuestion == true));
    }

    [HttpPut("GroupPut")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult GroupPut([FromBody] GroupPutDTO body)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.GroupPut(body));
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_JobScoreAbundanceComplexityService.DeleteRecord(id));
    }
}
