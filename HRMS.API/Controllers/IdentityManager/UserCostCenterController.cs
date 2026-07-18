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

[Route("api/UserCostCenter")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("دسترسی مرکز هزینه")]
public class UserCostCenterController : AppBaseController
{
    private readonly UserCostCenterService _userCostCenterService;
    public UserCostCenterController(UserCostCenterService UserCostCenterService, ILogger<UserCostCenterController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService, ILogger<UserCostCenterService> serviceLogger) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _userCostCenterService = UserCostCenterService;
        _userCostCenterService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_userCostCenterService.GetAsKeyValuePair(currentUserId));
    }
    [HttpGet, Route("GetAsKeyValuePairByPayLocationId/{payLocationId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByPayLocationId(long payLocationId)
    {
        return this.AppOk(_userCostCenterService.GetAsKeyValuePairByPayLocationId(currentUserId, payLocationId));
    }
    [HttpGet, Route("GetAsKeyValuePairByUserId/{userId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByUserId(long userId)
    {
        return this.AppOk(_userCostCenterService.GetAsKeyValuePair(userId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_userCostCenterService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null, [FromQuery] long? CostCenterId = null)
    {
        if (UserId == 0)
        {
            UserId = null;
        }
        if (CostCenterId == 0)
        {
            CostCenterId = null;
        }

        var Filtered = ((CustomIdentityContext)_userCostCenterService._db).UserCostCenters
           .Include(i => i.User)
           .Include(i => i.CostCenter)
           .Where(DateValidityExtension<UserCostCenter>.GetDateValidationPredicate(IgnoreExpired).And(i => (i.UserId == UserId || UserId == null) && (i.CostCenterId == CostCenterId || CostCenterId == null)))
           ;
        return this.AppOk(_userCostCenterService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] UserCostCenterDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _userCostCenterService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] UserCostCenterDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _userCostCenterService.UpdateForAsync(body);
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
        var result = _userCostCenterService.DeleteRecord(id);
        return this.AppOk(result);
    }
    [HttpPost("AssignMultiple")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> AssignMultiple([FromBody] UserCostCenterDTO body)
    {
        var result = await _userCostCenterService.AssignMultipleAsync(body);
        return this.AppOk(result);
    }
}



