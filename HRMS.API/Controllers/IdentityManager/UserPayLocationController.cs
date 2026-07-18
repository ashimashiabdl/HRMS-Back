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

[Route("api/UserPayLocation")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("دسترسی محل پرداخت")]
public class UserPayLocationController : AppBaseController
{
    private readonly UserPayLocationService _userPayLocationService;
    public UserPayLocationController(UserPayLocationService UserPayLocationService, ILogger<UserPayLocationController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _userPayLocationService = UserPayLocationService;
        _userPayLocationService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_userPayLocationService.GetAsKeyValuePair(currentUserId));
    }
    [HttpGet, Route("GetAsKeyValuePairByUserId/{userId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairByUserId(long userId)
    {
        return this.AppOk(_userPayLocationService.GetAsKeyValuePair(userId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_userPayLocationService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null, [FromQuery] long? PayLocationId = null)
    {

        if (UserId == 0)
        {
            UserId = null;
        }
        if (PayLocationId == 0)
        {
            PayLocationId = null;
        }

        var Filtered = ((CustomIdentityContext)_userPayLocationService._db).UserPayLocations
           .Include(i => i.User)
           .Include(i => i.PayLocation)
           .Where(DateValidityExtension<UserPayLocation>.GetDateValidationPredicate(IgnoreExpired).And(i => (i.UserId == UserId || UserId == null) && (i.PayLocationId == PayLocationId || PayLocationId == null)))
           ;
        return this.AppOk(_userPayLocationService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] UserPayLocationDTO body)
    {
        return Ok(await _userPayLocationService.CreateForAsync(body));
    }
    [HttpPost("AssignMultiple")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> AssignMultiple([FromBody] UserPayLocationDTO body)
    {
        var result = await _userPayLocationService.AssignMultipleAsync(body);
        return this.AppOk(result);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] UserPayLocationDTO body)
    {
        var result = await _userPayLocationService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        var result = _userPayLocationService.DeleteRecord(id);
        return this.AppOk(result);
    }
}



