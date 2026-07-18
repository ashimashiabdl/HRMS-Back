using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using Hr.Employee.infrastructure.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using HR.Identity.infrastructure.Services;

namespace HRMS.API.Controllers.Order;

[Route("api/Order")]
[ControllerGroup("OrderNameSpace", " احکام")]
[DisplayName("صدور احکام")]
public class OrderController : AppBaseController
{
    private readonly OrderService _orderService;
    private readonly EmployeeService _EmployeeService;
    private readonly UserPayLocationService _userPayLocationService;
    private readonly UserCostCenterService _userCostCenterService;
    IConfiguration _configuration;
    public OrderController(OrderService OrderService, EmployeeService EmployeeService, UserPayLocationService userPayLocationService, UserCostCenterService userCostCenterService, IConfiguration configuration, ILogger<OrderController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _EmployeeService = EmployeeService;
        _orderService = OrderService;
        _userPayLocationService = userPayLocationService;
        _userCostCenterService = userCostCenterService;
        _orderService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _orderService._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
        this._configuration = configuration;
    }
    [HttpGet("GetOrderLandingPageSummary")]
    [CustomAccessKey(AccessKey: "GetOrderLandingPageSummary")]
    public IActionResult GetOrderLandingPageSummary()
    {
        return this.AppOk(_orderService.GetOrderLandingPageSummary(currentUserId));
    }

    [HttpGet("GetOrderYearlyLandingPageSummary")]
    [CustomAccessKey(AccessKey: "GetOrderYearlyLandingPageSummary")]
    public IActionResult GetOrderYearlyLandingPageSummary()
    {
        return this.AppOk(_orderService.GetOrderYearlyLandingPageSummary(currentUserId));
    }

    [HttpGet("GetInterdictCountsByStatus")]
    [CustomAccessKey(AccessKey: "GetInterdictCountsByStatus")]
    public IActionResult GetInterdictCountsByStatus()
    {
        return this.AppOk(_orderService.GetInterdictCountsByStatus());
    }

    [HttpGet("GetInterdictCountsByPayLocation")]
    [CustomAccessKey(AccessKey: "GetInterdictCountsByPayLocation")]
    public IActionResult GetInterdictCountsByPayLocation()
    {
        return this.AppOk(_orderService.GetInterdictCountsByPayLocation());
    }

    /// <summary>
    /// واکشی تنظیمات حکم با حساسیت به تاریخ اجرای حکم
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("GetOrderConsequesnces")]
    [CustomAccessKey(AccessKey: "create")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult GetOrderConsequesnces([FromBody] BaseOrderRequest req)
    {
        return this.AppOk(_orderService.GetCurrentOrderConsequencs(req));

    }
    /// <summary>
    /// واکشی تنظیمات حکم با حساسیت به تاریخ اجرای حکم
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("GetOrderListByEmployeeID")]
    [CustomAccessKey(AccessKey: "create")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult GetOrderListByEmployeeID(GetOrderListByEmployeeIdRequest req)
    {
        req.CurrentUserId = currentUserId;
        return this.AppOk(_orderService.GetOrderListByEmployeeID(req));
    }

    /// <summary>
    /// صدور حکم
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("IssueOrder")]
    [CustomAccessKey(AccessKey: "create")]
    [OrderEmployeeAccessCheck(employeeIdPropertyName: "EmployeeId")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult IssueOrder(IssueOrderRequest req)
    {
        req.UserId = currentUserId;
        return this.AppOk(_orderService.IssueOrder(req));
    }
    /// <summary>
    ///حذف منطقی حکم
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpDelete("DeleteRecord/{Id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult DeleteRecord(long Id)
    {
        return this.AppOk(_orderService.DeleteRecord(Id));
    }
    [HttpGet("PrintPdf/{Id}/{id1}")]
    [CustomAccessKey(AccessKey: "PrintPdf")]
    public IActionResult PrintPdf(long Id, int id1)
    {
        var resp = _orderService.DownloadOrderPDF(Id, id1 > 0 ? true : false);
        if (resp.Success == true)
        {
            if (resp.Payload != null)
            {
                try
                {
                    System.IO.Stream stream = new System.IO.MemoryStream(resp.Payload);
                    return new Microsoft.AspNetCore.Mvc.FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "downloaded.pdf"
                    };
                }
                catch (Exception ex)
                {

                    return BadRequest(resp);
                }
             
            }
            return BadRequest(resp);
        }
        else
        {
            return BadRequest(resp);
        }
    }
    [HttpGet("GetOrderForPrint/{Id}")]
    [CustomAccessKey(AccessKey: "GetOrderForPrint")]

    public IActionResult GetOrderForPrint(long Id)
    {
        return this.AppOk(_orderService.GetOrderForPrint(Id));
    }

    [HttpGet("GetInterdictOrderArchiveStatus/{Id}")]
    [CustomAccessKey(AccessKey: "GetInterdictOrderArchiveStatus")]
    public IActionResult GetInterdictOrderArchiveStatus(long Id)
    {
        return this.AppOk(_orderService.GetInterdictOrderArchiveStatus(Id));
    }

    [HttpGet("DownloadInterdictOrderArchivePdf/{Id}/{isRaw}")]
    [CustomAccessKey(AccessKey: "DownloadInterdictOrderArchivePdf")]
    public IActionResult DownloadInterdictOrderArchivePdf(long Id, int isRaw)
    {
        var resp = _orderService.DownloadInterdictOrderArchivePdf(Id, isRaw > 0);
        if (resp.Success != true || resp.Payload is not byte[] pdfBytes)
        {
            return BadRequest(resp);
        }

        var stream = new MemoryStream(pdfBytes);
        return new FileStreamResult(stream, "application/pdf")
        {
            FileDownloadName = "order-archive.pdf"
        };
    }

    [HttpPost("RebuildInterdictOrderArchive/{Id}")]
    [CustomAccessKey(AccessKey: "RebuildInterdictOrderArchive")]
    public IActionResult RebuildInterdictOrderArchive(long Id)
    {
        return this.AppOk(_orderService.RebuildInterdictOrderArchive(Id));
    }

    [HttpGet("SendOrderToCartable/{Id}")]
    [CustomAccessKey(AccessKey: "SendOrderToCartable")]

    public IActionResult SendOrderToCartable(long Id)
    {
        return this.AppOk(_orderService.SendOrderToCartable(Id));
    }

    [HttpPut("UpdateDraftOrderDates")]
    [CustomAccessKey(AccessKey: "UpdateDraftOrderDates")]
    public IActionResult UpdateDraftOrderDates([FromBody] UpdateDraftOrderDatesDTO body)
    {
        return this.AppOk(_orderService.UpdateDraftOrderDates(body));
    }

    [HttpPost("GetBatchRequestFilteredPeople")]
    [CustomAccessKey(AccessKey: "create")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult GetBatchRequestFilteredPeople(FilterBatchDTO Id)
    {
        return this.AppOk(_orderService.GetBatchRequestFilteredPeople(Id));
    }
    [HttpGet("GetSelectedOrderLastOrderForCorrectAndCancellation/{Id}")]
    [CustomAccessKey(AccessKey: "GetSelectedOrderLastOrderForCorrectAndCancellation")]

    public IActionResult GetSelectedOrderLastOrderForCorrectAndCancellation(long Id)
    {
        return this.AppOk(_orderService.GetSelectedOrderLastOrderForCorrectAndCancellation(Id));
    }
    [HttpGet("GetOrderFlat/{Id}")]
    [CustomAccessKey(AccessKey: "GetOrderFlat")]

    public IActionResult GetOrderFlat(long Id)
    {
        return this.AppOk(_orderService.GetOrderFlat(Id));
    }
    [HttpGet("GetLastOrderDetailByEmployeeId/{Id}")]
    [CustomAccessKey(AccessKey: "GetLastOrderDetailByEmployeeId")]
    public IActionResult GetLastOrderDetailByEmployeeId(long Id)
    {

        var result = _EmployeeService.Get(Id);
        if (!_EmployeeService.CheckAccess(currentUserId, Id))
        {
            return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }

        return this.AppOk(_orderService.GetLastOrderDetailByEmployeeId(Id));
    }
    [HttpGet("GetAllPendingOrders")]
    [CustomAccessKey(AccessKey: "GetAllPendingOrders")]

    public IActionResult GetAllPendingOrders()
    {
        return this.AppOk(_orderService.GetAllPendingOrders());
    }
    [HttpGet("FinalApproveOrder/{Id}")]
    [CustomAccessKey(AccessKey: "FinalApproveOrder")]

    public IActionResult FinalApproveOrder(long Id)
    {
        return this.AppOk(_orderService.FinalApproveOrder(Id));
    }


    [HttpGet("FinalApproveOrderAll")]
    [CustomAccessKey(AccessKey: "FinalApproveOrderAll")]

    public IActionResult FinalApproveOrderAll()
    {
        return this.AppOk(_orderService.FinalApproveOrderAll());
    }
    [HttpGet("RejectOrderSingle/{Id}")]
    [CustomAccessKey(AccessKey: "RejectOrderSingle")]

    public IActionResult RejectOrderSingle(long Id)
    {
        return this.AppOk(_orderService.RejectOrderSingle(Id));
    }
    [HttpGet("RejectOrderAll")]
    [CustomAccessKey(AccessKey: "RejectOrderAll")]

    public IActionResult RejectOrderAll()
    {
        return this.AppOk(_orderService.RejectOrderAll());
    }

    [HttpGet("GetCurrentMonthFirstDay")]
    [CustomAccessKey(AccessKey: "GetCurrentMonthFirstDay")]

    public IActionResult GetCurrentMonthFirstDay()
    {
        return this.AppOk(OperationResult.Succeeded(payload: HR.SharedKernel.Utilities.GetCurrentMonthFirstDay()));
    }
    [HttpGet("GetOrderCartableDetail/{Id}")]
    [CustomAccessKey(AccessKey: "GetOrderCartableDetail")]
    public IActionResult GetOrderCartableDetail(long Id)
    {
        return this.AppOk(_orderService.GetOrderCartableDetail(Id));
    }

    /// <summary>
    /// مقایسه حکم جاری با حکم قبلی
    /// </summary>
    /// <param name="interdictOrderId">شناسه حکم</param>
    /// <returns>نتیجه مقایسه</returns>
    [HttpGet("CompareInterdictOrders/{interdictOrderId}")]
    [CustomAccessKey(AccessKey: "CompareInterdictOrders")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult CompareInterdictOrders(long interdictOrderId)
    {
        var result = _orderService.CompareInterdictOrders(interdictOrderId);
        return this.AppOk(result);
    }

    /// <summary>
    /// دریافت فهرست احکام صادره توسط کاربر جاری
    /// </summary>
    /// <returns>فهرست احکام صادره</returns>
    [HttpGet("GetMyIssuedOrdersList")]
    [CustomAccessKey(AccessKey: "GetMyIssuedOrdersList")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult GetMyIssuedOrdersList()
    {
        var result = _orderService.GetMyIssuedOrdersList(currentUserId);
        return this.AppOk(result);
    }

    /// <summary>
    /// دریافت فهرست احکام در حال انقضا (StatusId = 9)
    /// </summary>
    /// <param name="endDateFilter">تاریخ فیلتر - احکامی که تا این تاریخ منقضی می‌شوند (پیش‌فرض: یک ماه آینده)</param>
    /// <param name="includeExpired">آیا احکام منقضی شده نیز نمایش داده شوند (پیش‌فرض: false)</param>
    /// <param name="payLocationId">شناسه محل پرداخت - اگر null باشد از currentUserDefaultOrganId استفاده می‌شود</param>
    /// <param name="costCenterId">شناسه مرکز هزینه - اگر null باشد همه مراکز هزینه مجاز کاربر نمایش داده می‌شوند</param>
    /// <returns>فهرست احکام در حال انقضا</returns>
    [HttpGet("GetExpiringOrdersList")]
    [CustomAccessKey(AccessKey: "GetExpiringOrdersList")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult GetExpiringOrdersList([FromQuery] DateTime? endDateFilter = null, [FromQuery] bool includeExpired = false, [FromQuery] long? payLocationId = null, [FromQuery] long? costCenterId = null)
    {
        // دریافت فهرست PayLocation های مجاز کاربر
        var userPayLocationsResult = _userPayLocationService.GetAsKeyValuePair(currentUserId);
        if (!userPayLocationsResult.Success || userPayLocationsResult.Payload == null)
        {
            return this.AppBadRequest("خطا در دریافت دسترسی‌های محل پرداخت کاربر");
        }

        var userPayLocationIds = ((List<HR.SharedKernel.Data.KeyValuePair>)userPayLocationsResult.Payload).Select(x => x.key).ToList();

        // تعیین PayLocationId برای فیلتر
        long selectedPayLocationId;
        if (payLocationId.HasValue)
        {
            // بررسی اینکه PayLocationId ارسالی در دسترسی‌های کاربر موجود است
            if (!userPayLocationIds.Contains(payLocationId.Value))
            {
                return this.AppBadRequest("شما به محل پرداخت انتخاب شده دسترسی ندارید");
            }
            selectedPayLocationId = payLocationId.Value;
        }
        else
        {
            // استفاده از currentUserDefaultOrganId به عنوان پیش‌فرض
            selectedPayLocationId = currentUserDefaultOrganId;
            
            // بررسی اینکه currentUserDefaultOrganId در دسترسی‌های کاربر موجود است
            if (selectedPayLocationId > 0 && !userPayLocationIds.Contains(selectedPayLocationId))
            {
                // اگر currentUserDefaultOrganId در دسترسی‌ها نیست، اولین PayLocation را انتخاب می‌کنیم
                if (userPayLocationIds.Any())
                {
                    selectedPayLocationId = userPayLocationIds.First();
                }
                else
                {
                    return this.AppBadRequest("شما به هیچ محل پرداختی دسترسی ندارید");
                }
            }
        }

        // دریافت فهرست CostCenter های مجاز کاربر برای PayLocation انتخاب شده
        List<long>? userCostCenterIds = null;
        if (costCenterId.HasValue)
        {
            var userCostCentersResult = _userCostCenterService.GetAsKeyValuePairByPayLocationId(currentUserId, selectedPayLocationId);
            if (!userCostCentersResult.Success || userCostCentersResult.Payload == null)
            {
                return this.AppBadRequest("خطا در دریافت دسترسی‌های مرکز هزینه کاربر");
            }

            userCostCenterIds = ((List<HR.SharedKernel.Data.KeyValuePair>)userCostCentersResult.Payload).Select(x => x.key).ToList();
            
            // بررسی اینکه CostCenterId ارسالی در دسترسی‌های کاربر موجود است
            if (!userCostCenterIds.Contains(costCenterId.Value))
            {
                return this.AppBadRequest("شما به مرکز هزینه انتخاب شده دسترسی ندارید");
            }
        }

        var result = _orderService.GetExpiringOrdersList(endDateFilter, includeExpired, selectedPayLocationId, costCenterId);
        return this.AppOk(result);
    }

    /// <summary>
    /// دریافت کارتابل تسویه حساب — احکام تایید نهایی با وضعیت استخدام IsEmployed = false
    /// </summary>
    [HttpGet("GetSettlementCartableList")]
    [CustomAccessKey(AccessKey: "GetSettlementCartableList")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult GetSettlementCartableList([FromQuery] long? payLocationId = null, [FromQuery] long? costCenterId = null)
    {
        var userPayLocationsResult = _userPayLocationService.GetAsKeyValuePair(currentUserId);
        if (!userPayLocationsResult.Success || userPayLocationsResult.Payload == null)
        {
            return this.AppBadRequest("خطا در دریافت دسترسی‌های محل پرداخت کاربر");
        }

        var userPayLocationIds = ((List<HR.SharedKernel.Data.KeyValuePair>)userPayLocationsResult.Payload).Select(x => x.key).ToList();

        long selectedPayLocationId;
        if (payLocationId.HasValue)
        {
            if (!userPayLocationIds.Contains(payLocationId.Value))
            {
                return this.AppBadRequest("شما به محل پرداخت انتخاب شده دسترسی ندارید");
            }
            selectedPayLocationId = payLocationId.Value;
        }
        else
        {
            selectedPayLocationId = currentUserDefaultOrganId;

            if (selectedPayLocationId > 0 && !userPayLocationIds.Contains(selectedPayLocationId))
            {
                if (userPayLocationIds.Any())
                {
                    selectedPayLocationId = userPayLocationIds.First();
                }
                else
                {
                    return this.AppBadRequest("شما به هیچ محل پرداختی دسترسی ندارید");
                }
            }
        }

        if (costCenterId.HasValue)
        {
            var userCostCentersResult = _userCostCenterService.GetAsKeyValuePairByPayLocationId(currentUserId, selectedPayLocationId);
            if (!userCostCentersResult.Success || userCostCentersResult.Payload == null)
            {
                return this.AppBadRequest("خطا در دریافت دسترسی‌های مرکز هزینه کاربر");
            }

            var userCostCenterIds = ((List<HR.SharedKernel.Data.KeyValuePair>)userCostCentersResult.Payload).Select(x => x.key).ToList();

            if (!userCostCenterIds.Contains(costCenterId.Value))
            {
                return this.AppBadRequest("شما به مرکز هزینه انتخاب شده دسترسی ندارید");
            }
        }

        var result = _orderService.GetSettlementCartableList(selectedPayLocationId, costCenterId);
        return this.AppOk(result);
    }
}
