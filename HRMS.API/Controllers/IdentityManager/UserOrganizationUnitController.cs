using HR.SharedKernel.Attribute;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserOrganizationUnit")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("دسترسی واحد سازمانی")]
public class UserOrganizationUnitController : AppBaseController
{
    private readonly UserOrganizationUnitService _userOrganizationUnitService;
    public UserOrganizationUnitController(UserOrganizationUnitService UserOrganizationUnitService, ILogger<UserOrganizationUnitController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _userOrganizationUnitService = UserOrganizationUnitService;
        _userOrganizationUnitService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_userOrganizationUnitService.GetAsKeyValuePair(currentUserId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_userOrganizationUnitService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null, [FromQuery] long? OrganizationUnitId = null)
    {
        if (UserId == 0)
        {
            UserId = null;
        }
        if (OrganizationUnitId == 0)
        {
            OrganizationUnitId = null;
        }

        var Filtered = ((CustomIdentityContext)_userOrganizationUnitService._db).UserOrganizationUnits
           .Include(i => i.User)
           .Include(i => i.OrganizationUnit)
           .Where(DateValidityExtension<UserOrganizationUnit>.GetDateValidationPredicate(IgnoreExpired).And(i => (i.UserId == UserId || UserId == null) && (i.OrganizationUnitId == OrganizationUnitId || OrganizationUnitId == null)))
           ;
        return this.AppOk(_userOrganizationUnitService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] UserOrganizationUnitDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _userOrganizationUnitService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] UserOrganizationUnitDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _userOrganizationUnitService.UpdateForAsync(body);
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
        var result = _userOrganizationUnitService.DeleteRecord(id);
        return this.AppOk(result);
    }
}



