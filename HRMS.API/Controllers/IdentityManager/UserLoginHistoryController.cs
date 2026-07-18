using HR.SharedKernel.Attribute;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserLoginHistory")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("تاریخچه ورود کاربر")]
public class UserLoginHistoryController : AppBaseController
{
    private readonly UserLoginHistoryService _UserLoginHistoryService;
    public UserLoginHistoryController(UserLoginHistoryService UserLoginHistoryService, ILogger<UserLoginHistoryController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _UserLoginHistoryService = UserLoginHistoryService;
        _UserLoginHistoryService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetPagedDatasuccess/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedDatasuccess(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "")
    {
        var filtered = _UserLoginHistoryService._db.Set<UserLoginHistory>()
            .AsNoTracking()
            .Include(i => i.AspNetUser)
            .Where(i => i.IsSuccess == true && i.AspNetUserId == currentUserId);
        return this.AppOk(_UserLoginHistoryService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filtered));
    }
    [HttpGet, Route("GetPagedDataFail/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedDataFail(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "")
    {
        var filtered = _UserLoginHistoryService._db.Set<UserLoginHistory>()
            .AsNoTracking()
            .Include(i => i.AspNetUser)
            .Where(i => i.IsSuccess == false && i.AspNetUserId == currentUserId);
        return this.AppOk(_UserLoginHistoryService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filtered));
    }
    [HttpGet, Route("GetPagedDatasuccessForSelectedUser/{currentPage}/{pageSize}/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedDatasuccessForSelectedUser(int currentPage = 0, int pageSize = 10, long id = 0, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "")
    {
        var filtered = _UserLoginHistoryService._db.Set<UserLoginHistory>()
            .AsNoTracking()
            .Include(i => i.AspNetUser)
            .Where(i => i.IsSuccess == true && i.AspNetUserId == id);
        return this.AppOk(_UserLoginHistoryService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filtered));
    }
    [HttpGet, Route("GetPagedDataFailForSelectedUser/{currentPage}/{pageSize}/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedDataFailForSelectedUser(int currentPage = 0, int pageSize = 10, long id = 0, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "")
    {
        var filtered = _UserLoginHistoryService._db.Set<UserLoginHistory>()
            .AsNoTracking()
            .Include(i => i.AspNetUser)
            .Where(i => i.IsSuccess == false && i.AspNetUserId == id);
        return this.AppOk(_UserLoginHistoryService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filtered));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? AspNetUserId = null, [FromQuery] DateTime? StartDate = null, [FromQuery] DateTime? EndDate = null)
    {
        IQueryable<UserLoginHistory> filtered = _UserLoginHistoryService._db.Set<UserLoginHistory>()
            .AsNoTracking()
            .Include(i => i.AspNetUser);
        
        // اعمال فیلتر AspNetUserId
        if (AspNetUserId.HasValue && AspNetUserId.Value > 0)
        {
            filtered = filtered.Where(i => i.AspNetUserId == AspNetUserId.Value);
        }
        
        // اعمال فیلتر بازه تاریخی روی CreateDate
        if (StartDate.HasValue)
        {
            // تنظیم زمان به ابتدای روز
            var startDateValue = StartDate.Value.Date;
            filtered = filtered.Where(i => i.CreateDate.HasValue && i.CreateDate.Value.Date >= startDateValue);
        }
        
        if (EndDate.HasValue)
        {
            // تنظیم زمان به انتهای روز
            var endDateValue = EndDate.Value.Date.AddDays(1).AddTicks(-1);
            filtered = filtered.Where(i => i.CreateDate.HasValue && i.CreateDate.Value <= endDateValue);
        }
        
        return this.AppOk(_UserLoginHistoryService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: filtered));
    }

}



