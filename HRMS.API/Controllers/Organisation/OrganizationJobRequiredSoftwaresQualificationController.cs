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

[Route("api/OrganizationJobRequiredSoftwaresQualification")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("شرایط احراز شغل نرم افزار های مورد نیاز")]
public class OrganizationJobRequiredSoftwaresQualificationController : AppBaseController
{
    private readonly OrganizationJobRequiredSoftwaresQualificationService _OrganizationJobRequiredSoftwaresQualificationService;
    public OrganizationJobRequiredSoftwaresQualificationController(OrganizationJobRequiredSoftwaresQualificationService Service, ILogger<OrganizationJobRequiredSoftwaresQualificationController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _OrganizationJobRequiredSoftwaresQualificationService = Service;
        _OrganizationJobRequiredSoftwaresQualificationService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }


    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganizationJobRequiredSoftwaresQualificationService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? OrganizationJobId = null)
    {
        if (OrganizationJobId == 0)
        {
            OrganizationJobId = null;
        }

        var Filtered = _OrganizationJobRequiredSoftwaresQualificationService._db.Set<OrganizationJobRequiredSoftwaresQualification>()
       .Include(i => i.OrganizationJob.Job)
       .Where(DateValidityExtension<OrganizationJobRequiredSoftwaresQualification>.GetDateValidationPredicate(IgnoreExpired)
       .And(i => i.OrganizationJob.OrganisationChartId == currentUserDefaultOrganId)
       .And(i => OrganizationJobId == null || OrganizationJobId == i.OrganizationJobId)
       );
        return this.AppOk(_OrganizationJobRequiredSoftwaresQualificationService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganizationJobRequiredSoftwaresQualificationDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _OrganizationJobRequiredSoftwaresQualificationService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] OrganizationJobRequiredSoftwaresQualificationDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganizationJobRequiredSoftwaresQualificationService.UpdateForAsync(body);
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
        return this.AppOk(_OrganizationJobRequiredSoftwaresQualificationService.DeleteRecord(id));
    }
}
