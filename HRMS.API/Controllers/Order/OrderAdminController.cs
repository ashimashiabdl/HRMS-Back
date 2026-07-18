using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Controllers.Order;

[Route("api/OrderAdmin")]
[ControllerGroup("OrderNameSpace", " احکام")]
public class OrderAdminController : AppBaseController
{
    private readonly OrderService _orderService;

    public OrderAdminController(
        OrderService OrderService,
        ILogger<OrderController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService UserResolverService)
        : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _orderService = OrderService;
        _orderService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    public class ProtectConnectionStringRequest
    {
        [Required]
        public string? plain { get; set; }
    }

    [HttpPost("ProtectConnectionString")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult ProtectConnectionString([FromBody] ProtectConnectionStringRequest body)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];
        ConnectionStringProtector.SetLogger(_logger);

        if (string.IsNullOrWhiteSpace(body?.plain))
        {
            _logger.LogWarning("[{RequestId}] ProtectConnectionString: empty input", requestId);
            return this.AppOk(OperationResult.Failed("ConnectionString خالی است"));
        }

        _logger.LogInformation(
            "[{RequestId}] ProtectConnectionString started. InputLength={Length}",
            requestId,
            body.plain.Length);

        string? lm = null;
        string? cu = null;
        var errors = new List<string>();

        try
        {
            lm = ConnectionStringProtector.ProtectWithLocalMachine(body.plain);
            if (!string.Equals(ConnectionStringProtector.TryUnprotect(lm), body.plain, StringComparison.Ordinal))
            {
                _logger.LogWarning("[{RequestId}] LocalMachine unprotect mismatch", requestId);
            }
        }
        catch (Exception ex)
        {
            errors.Add($"LocalMachine: {ex.GetType().Name} - {ex.Message}");
            _logger.LogError(ex, "[{RequestId}] LocalMachine protect failed", requestId);
        }

        try
        {
            cu = ConnectionStringProtector.ProtectWithCurrentUser(body.plain);
            if (!string.Equals(ConnectionStringProtector.TryUnprotect(cu), body.plain, StringComparison.Ordinal))
            {
                _logger.LogWarning("[{RequestId}] CurrentUser unprotect mismatch", requestId);
            }
        }
        catch (Exception ex)
        {
            errors.Add($"CurrentUser: {ex.GetType().Name} - {ex.Message}");
            _logger.LogError(ex, "[{RequestId}] CurrentUser protect failed", requestId);
        }

        if (lm == null && cu == null)
        {
            return this.AppOk(OperationResult.Failed(
                $"خطا در رمزنگاری - هیچ روشی موفق نشد. جزئیات: {string.Join(" | ", errors)}"));
        }

        return this.AppOk(OperationResult.Succeeded(payload: new
        {
            dpapi_lm = lm,
            dpapi_cu = cu,
            success_lm = lm != null,
            success_cu = cu != null,
            errors = errors.Count > 0 ? errors : null,
            requestId
        }));
    }

    [HttpGet("FinalApproveOrder/{Id}")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult FinalApproveOrder(long Id)
    {
        return this.AppOk(_orderService.FinalApproveOrder(Id));
    }

    [HttpGet("UpdateSelectedOrderStatus/{Id}/{Id1}")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult UpdateSelectedOrderStatus(long Id, long id1)
    {
        return this.AppOk(_orderService.UpdateOrderStatus(Id, id1));
    }

    [HttpGet("GetRecruitOrderProperties")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetRecruitOrderProperties()
    {
        return this.AppOk(_orderService.GetRecruitOrderProperties());
    }

    [HttpGet("GetInterdictOrderProperties")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetInterdictOrderProperties()
    {
        return this.AppOk(_orderService.GetInterdictOrderProperties());
    }

    [HttpPut("SetRecruitFieldValue")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult SetRecruitFieldValue([FromBody] AdminFormOrderFieldDTO body)
    {
        return this.AppOk(_orderService.SetRecruitFieldValue(body));
    }

    [HttpPut("SetInterdictFieldValue")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult SetInterdictFieldValue([FromBody] AdminFormOrderFieldDTO body)
    {
        return this.AppOk(_orderService.SetInterdictFieldValue(body));
    }

    [HttpPost("UpdateReportDataSource")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult UpdateReportDataSource()
    {
        return this.AppOk(_orderService.UpdateReportDataSource());
    }
}
