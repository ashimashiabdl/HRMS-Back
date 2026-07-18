using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/BatchSettlementRequestDetail")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("جزئیات درخواست تسویه حساب گروهی")]
public class BatchSettlementRequestDetailController : AppBaseController
{
    private readonly BatchSettlementRequestDetailService _service;

    public BatchSettlementRequestDetailController(
        BatchSettlementRequestDetailService service,
        ILogger<BatchSettlementRequestDetailController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{BatchSettlementRequestId?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true,
        [FromQuery] long? BatchSettlementRequestId = null)
    {
        if (BatchSettlementRequestId is not > 0)
        {
            return this.AppNotFound();
        }

        var filtered = _service._db.Set<BatchSettlementRequestDetail>()
            .Include(x => x.Employee)
            .Where(i => i.IsDeleted != true && i.BatchSettlementRequestId == BatchSettlementRequestId);

        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: filtered));
    }
}
