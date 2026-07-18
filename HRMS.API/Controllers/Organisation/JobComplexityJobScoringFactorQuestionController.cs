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

[Route("api/JobComplexityJobScoringFactorQuestion")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("تعریف سوال برای هر ردیف پیچیدگی عامل امتیاز آور")]
public class JobComplexityJobScoringFactorQuestionController : AppBaseController
{
    private readonly JobComplexityJobScoringFactorQuestionService _JobComplexityJobScoringFactorQuestionService;
    public JobComplexityJobScoringFactorQuestionController(JobComplexityJobScoringFactorQuestionService Service, ILogger<JobComplexityJobScoringFactorQuestionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _JobComplexityJobScoringFactorQuestionService = Service;
        _JobComplexityJobScoringFactorQuestionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetComplexityQuestionsAsKeyValuePair/{id}/{id1}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetComplexityQuestionsAsKeyValuePair(long id, long id1)
    {
        return this.AppOk(_JobComplexityJobScoringFactorQuestionService.GetComplexityQuestionsAsKeyValuePair(id, id1));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_JobComplexityJobScoringFactorQuestionService.Get(id));
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

        var Filtered = _JobComplexityJobScoringFactorQuestionService._db.Set<JobComplexityJobScoringFactorQuestion>()
        .Include(i => i.OrganizationJob)
        .Include(i => i.OrganizationJob.Job)
        .Include(i => i.JobScoringFactor.Group)
        .Where(DateValidityExtension<JobComplexityJobScoringFactorQuestion>.GetDateValidationPredicate()
        .And(i => i.OrganizationJob.OrganisationChartId == currentUserDefaultOrganId)
        .And(i => (OrganizationJobId == null) || OrganizationJobId == i.OrganizationJobId)
        .And(i => (JobScoringFactorId == null) || JobScoringFactorId == i.JobScoringFactorId)
        )
            ;
        return this.AppOk(_JobComplexityJobScoringFactorQuestionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] JobComplexityJobScoringFactorQuestionDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _JobComplexityJobScoringFactorQuestionService.CreateForAsync(body));
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
    public async Task<IActionResult> Put([FromBody] JobComplexityJobScoringFactorQuestionDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _JobComplexityJobScoringFactorQuestionService.UpdateForAsync(body);
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
        return this.AppOk(_JobComplexityJobScoringFactorQuestionService.DeleteRecord(id));
    }
}
