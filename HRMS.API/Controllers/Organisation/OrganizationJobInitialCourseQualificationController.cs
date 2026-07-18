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

[Route("api/OrganizationJobInitialCourseQualification")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("شرایط احراز شغل آموزش های اولیه")]
public class OrganizationJobInitialCourseQualificationController : AppBaseController
{
    private readonly OrganizationJobInitialCourseQualificationService _OrganizationJobInitialCourseQualificationService;
    public OrganizationJobInitialCourseQualificationController(OrganizationJobInitialCourseQualificationService Service, ILogger<OrganizationJobInitialCourseQualificationController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganizationJobInitialCourseQualificationService = Service;
        _OrganizationJobInitialCourseQualificationService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }


    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganizationJobInitialCourseQualificationService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? OrganizationJobId = null)
    {
        if (OrganizationJobId == 0)
        {
            OrganizationJobId = null;
        }

        var Filtered = _OrganizationJobInitialCourseQualificationService._db.Set<OrganizationJobInitialCourseQualification>()
       .Include(i => i.OrganizationJob.Job)
       .Where(DateValidityExtension<OrganizationJobInitialCourseQualification>.GetDateValidationPredicate(IgnoreExpired)
       .And(i => i.OrganizationJob.OrganisationChartId == currentUserDefaultOrganId)
       .And(i => OrganizationJobId == null || OrganizationJobId == i.OrganizationJobId)
       );
        return this.AppOk(_OrganizationJobInitialCourseQualificationService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganizationJobInitialCourseQualificationDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _OrganizationJobInitialCourseQualificationService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] OrganizationJobInitialCourseQualificationDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganizationJobInitialCourseQualificationService.UpdateForAsync(body);
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
        return this.AppOk(_OrganizationJobInitialCourseQualificationService.DeleteRecord(id));
    }
}
