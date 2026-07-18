using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;


[Route("api/BatchPayRollRequest")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("درخواست گروهی")]
public class BatchPayRollRequestController : AppBaseController
{
    private readonly BatchPayRollRequestService _BatchPayRollRequestService;

    private UserResolverService _userResolverService;
    public BatchPayRollRequestController(BatchPayRollRequestService Service, ILogger<BatchPayRollRequestController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _BatchPayRollRequestService = Service;
        _BatchPayRollRequestService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _userResolverService = UserResolverService;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_BatchPayRollRequestService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_BatchPayRollRequestService.Get(id, currentUserId, isAdmin));
    }
    [HttpGet, Route("tryagain/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult tryagain(int id)
    {
        return this.AppOk(_BatchPayRollRequestService.UpdateRequestState(id, Enums.BatchPayRollRequestState.TryAgain));
    }
    [HttpGet, Route("stopRequest/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult stopRequest(int id)
    {
        return this.AppOk(_BatchPayRollRequestService.UpdateRequestState(id, Enums.BatchPayRollRequestState.CancelByUser));
    }

    [HttpGet, Route("delete/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult delete(int id)
    {
        return this.AppOk(_BatchPayRollRequestService.UpdateRequestState(id, Enums.BatchPayRollRequestState.Deleted));
    }
    [HttpGet, Route("GetCurrentPeriodEligibleEmployees/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentPeriodEligibleEmployees(long id)
    {
        return this.AppOk(_BatchPayRollRequestService.GetCurrentPeriodEligibleEmployees(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var customSource = _BatchPayRollRequestService._unitOfWork.Context.BatchPayRollRequests.Where(i => i.OrganisationChartId == currentUserDefaultOrganId);

        return this.AppOk(_BatchPayRollRequestService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, currentUserId, isAdmin));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public IActionResult Post([FromBody] BatchPayRollRequestDTO body)
    {
        body.UserId = _userResolverService.GetUserId();
        return Ok(_BatchPayRollRequestService.CreateForAsync(body));
    }
}
