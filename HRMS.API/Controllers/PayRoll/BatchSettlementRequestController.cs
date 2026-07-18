using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Share;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/BatchSettlementRequest")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("درخواست تسویه حساب گروهی")]
public class BatchSettlementRequestController : AppBaseController
{
    private readonly BatchSettlementRequestService _service;
    private readonly UserResolverService _userResolverService;

    public BatchSettlementRequestController(
        BatchSettlementRequestService service,
        ILogger<BatchSettlementRequestController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _service._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
        _userResolverService = userResolverService;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_service.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id, currentUserId, isAdmin));
    }

    [HttpGet, Route("tryagain/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult TryAgain(int id)
    {
        return this.AppOk(_service.UpdateRequestState(id, Enums.BatchSettlementRequestState.TryAgain));
    }

    [HttpGet, Route("stopRequest/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult StopRequest(int id)
    {
        return this.AppOk(_service.UpdateRequestState(id, Enums.BatchSettlementRequestState.CancelByUser));
    }

    [HttpGet, Route("delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_service.UpdateRequestState(id, Enums.BatchSettlementRequestState.Deleted));
    }

    [HttpGet, Route("GetEligibleSettlementCandidates")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetEligibleSettlementCandidates([FromQuery] long? payLocationId = null, [FromQuery] long? costCenterId = null)
    {
        return this.AppOk(_service.GetEligibleSettlementCandidates(payLocationId, costCenterId));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, false, currentUserId, isAdmin));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public IActionResult Post([FromBody] BatchSettlementRequestDTO body)
    {
        body.UserId = _userResolverService.GetUserId();
        return Ok(_service.CreateForAsync(body));
    }
}
