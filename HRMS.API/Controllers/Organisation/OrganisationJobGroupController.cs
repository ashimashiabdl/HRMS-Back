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

[Route("api/OrganisationJobGroup")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("رشته شغلی")]
public class OrganisationJobGroupController : AppBaseController
{
    private readonly OrganisationJobGroupService _OrganisationJobGroupService;
    private OrganisationContext _context;
    public OrganisationJobGroupController(OrganisationContext context, OrganisationJobGroupService Service, ILogger<OrganisationJobGroupController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _OrganisationJobGroupService = Service;
        _OrganisationJobGroupService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair/{id?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair(string? id = "0")
    {
        if (string.IsNullOrWhiteSpace(id)
            || string.Equals(id, "null", StringComparison.OrdinalIgnoreCase)
            || !int.TryParse(id, out var parsedId)
            || parsedId == 0)
        {
            return this.AppOk(OperationResult.Succeeded());
        }

        return this.AppOk(_OrganisationJobGroupService.GetAsKeyValuePair(parsedId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationJobGroupService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {

        var Filtered = _context.OrganisationJobGroups
            .Include(i => i.OrganisationJobCategory)
            .Include(i => i.OrganisationJobCategory.JobCategory)
            .Where(DateValidityExtension<OrganisationJobGroup>.GetDateValidationPredicate().And(i => i.OrganisationChartId == currentUserDefaultOrganId))
            ;
        return this.AppOk(_OrganisationJobGroupService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationJobGroupDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _OrganisationJobGroupService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] OrganisationJobGroupDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganisationJobGroupService.UpdateForAsync(body);
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
        return this.AppOk(_OrganisationJobGroupService.DeleteRecord(id));
    }
}
