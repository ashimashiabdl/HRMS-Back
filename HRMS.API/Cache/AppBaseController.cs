using AutoMapper;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Dapper;
using System.Data;

namespace HRMS.API.Cache;

[AuthorizeHR]
[Route("api/[controller]")]
[ApiController]
public abstract class AppBaseController(UserResolverService UserResolverService, ILogger<AppBaseController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper) : ControllerBase
{
    protected readonly ILogger<AppBaseController> _logger = logger;
    protected readonly IHttpContextAccessor _accessor = accessor;
  
  
    protected IMapper _mapper = mapper;
    UserResolverService _userResolverService = UserResolverService;



    [HttpGet, Route("GetWarmUpSignal")]
    [AllowAnonymous]
    public IActionResult GetWarmUpSignal()
    {
        return this.AppOk(1);
    }

    public string CurrentUserName
    {
        get
        {
            return _userResolverService.GetUser();
        }
    }
    public string CurrentUserFullName
    {
        get
        {
            return _userResolverService.fullname();
        }
    }

    public long currentUserId
    {
        get
        {
            return _userResolverService.GetUserId();
        }
    }


    public bool isAdmin
    {
        get
        {
            return _userResolverService.IsAdmin();
        }
    }
    public long currentUserEmployeeId
    {
        get
        {
            return _userResolverService.currentEmployeeId();
        }
    }
    public long currentUserDefaultOrganId
    {
        get
        {
            long? currentUserDefaultOrganId = 0;
            // اطلاعات پیش فرض کاربر جاری در میکرو سرویس دیگری هست
            var q = "SELECT [DefaultOrganId] FROM [Identity].[User_Default_Setting]  WHERE UserId = @userId";
            var p = new DynamicParameters();
            p.Add("@userId", currentUserId, DbType.Int64);
            currentUserDefaultOrganId = Task.FromResult(dapper.Get<int?>(q, p, commandType: CommandType.Text)).Result;
            return currentUserDefaultOrganId > 0 ? currentUserDefaultOrganId.Value : 0;
        }
    }
    public long currentUserDefaultPaymentPeiodId
    {
        get
        {
            long? DefaultPaymentPeriodId = 0;
            // اطلاعات پیش فرض کاربر جاری در میکرو سرویس دیگری هست
            var q = "SELECT [DefaultPaymentPeriodId] FROM [Identity].[User_Default_Setting]  WHERE UserId = @userId";
            var p = new DynamicParameters();
            p.Add("@userId", currentUserId, DbType.Int64);
            DefaultPaymentPeriodId = Task.FromResult(dapper.Get<int?>(q, p, commandType: CommandType.Text)).Result;
            return DefaultPaymentPeriodId > 0 ? DefaultPaymentPeriodId.Value : 0;
        }
    }



}
