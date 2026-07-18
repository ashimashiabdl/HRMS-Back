using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/BatchLog")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("لاگ عملیات گروهی")]
public class BatchLogController(
    BatchLogService service,
    ILogger<BatchLogController> logger,
    IHttpContextAccessor accessor,
    IMapper mapper,
    IDapper dapper,
    UserResolverService UserResolverService)
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly BatchLogService _service = service;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id) => this.AppOk(_service.Get(id));

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true,
        [FromQuery] long? interdictOrderId = null,
        [FromQuery] long? employeeId = null,
        [FromQuery] long? paymentPeriodId = null,
        [FromQuery] int? logTypeId = null)
    {
        if (string.IsNullOrWhiteSpace(activeSortColumn))
        {
            activeSortColumn = "CreateDate";
            Sortdirection = "desc";
        }

        var filteredData = _service._unitOfWork.Context.BatchLogs
            .Include(i => i.PaymentPeriod)
            .Include(i => i.Employee)
            .AsQueryable();

        if (interdictOrderId.HasValue && interdictOrderId.Value > 0)
            filteredData = filteredData.Where(i => i.InterdictOrderId == interdictOrderId.Value);

        if (employeeId.HasValue && employeeId.Value > 0)
            filteredData = filteredData.Where(i => i.EmployeeId == employeeId.Value);

        if (paymentPeriodId.HasValue && paymentPeriodId.Value > 0)
            filteredData = filteredData.Where(i => i.PaymentPeriodId == paymentPeriodId.Value);

        if (logTypeId.HasValue && logTypeId.Value > 0)
            filteredData = filteredData.Where(i => i.LogTypeId == logTypeId.Value);

        return this.AppOk(_service.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            CustomDataSource: filteredData));
    }
}
