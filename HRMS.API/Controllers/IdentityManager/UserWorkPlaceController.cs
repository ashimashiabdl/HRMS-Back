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
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserWorkPlace")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("محل خدمت کاربران")]
public class UserWorkPlaceController : AppBaseController
{
    private readonly UserWorkPlaceService _userWorkPlaceService;
    public UserWorkPlaceController(UserWorkPlaceService UserWorkPlaceService, ILogger<UserWorkPlaceController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _userWorkPlaceService = UserWorkPlaceService;
        _userWorkPlaceService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_userWorkPlaceService.GetAsKeyValuePair(currentUserId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_userWorkPlaceService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null, [FromQuery] long? WorkPlaceId = null)
    {
        if (UserId == 0)
        {
            UserId = null;
        }
        if (WorkPlaceId == 0)
        {
            WorkPlaceId = null;
        }
        var Filtered = ((CustomIdentityContext)_userWorkPlaceService._db).UserWorkPlaces
           .Include(i => i.User)
           .Include(i => i.WorkPlace)
           .Where(DateValidityExtension<UserWorkPlace>.GetDateValidationPredicate(IgnoreExpired).And(i => (i.UserId == UserId || UserId == null) && (i.WorkPlaceId == WorkPlaceId || WorkPlaceId == null)))
           ;
        return this.AppOk(_userWorkPlaceService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] UserWorkPlaceDTO body)
    {
        return Ok(await _userWorkPlaceService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] UserWorkPlaceDTO body)
    {
        var result = await _userWorkPlaceService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        var result = _userWorkPlaceService.DeleteRecord(id);
        return this.AppOk(result);
    }
}



