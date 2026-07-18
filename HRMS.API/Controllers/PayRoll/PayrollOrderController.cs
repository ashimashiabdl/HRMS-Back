using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Quality;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
namespace HRMS.API.Controllers.PayRoll;

[Route("api/PayrollOrder")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("احکام در حقوق و دستمزد")]
public class PayrollOrderController : AppBaseController
{
    private readonly PayrollOrderService _orderService;
    private readonly BatchLogService _batchLogService;
    public PayrollOrderController(PayrollOrderService OrderService, BatchLogService BatchLogService, ILogger<PayrollOrderController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _batchLogService = BatchLogService;
        _orderService = OrderService;
        _orderService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _orderService._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
    }

    [HttpGet, Route("GetArearsLogPagedData/{currentPage}/{pageSize}/{InterdictOrderId}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetArearsLogPagedData(int currentPage = 0, int pageSize = 10, long InterdictOrderId = 0, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "")
    {
        var filteredData = _batchLogService._unitOfWork.Context.BatchLogs.Include(i => i.PaymentPeriod).Where(i => i.InterdictOrderId == InterdictOrderId);
        activeSortColumn = "CreateDate";
        Sortdirection = "asc";
        return this.AppOk(_batchLogService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filteredData));
    }
    [HttpPost("GetOrderListByEmployeeID")]
    [CustomAccessKey(AccessKey: "create")]

    public IActionResult GetOrderListByEmployeeID(GetOrderListByEmployeeIdRequest req)
    {
        if (ModelState.IsValid)
        {
            try
            {

                req.CurrentUserId = currentUserId;
                return this.AppOk(_orderService.GetOrderListByEmployeeID(req, true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }        /// <summary>
             /// </summary>
             /// <param name="req"></param>
             /// <returns></returns>
    [HttpPost("getCurrentOrganPayRollPendigOrders")]
    [CustomAccessKey(AccessKey: "create")]

    public IActionResult getCurrentOrganPayRollPendigOrders([FromBody] PayRollOrderPagerDTO id)
    {
        return this.AppOk(_orderService.getCurrentOrganPayRollPendigOrders(id));
    }

    [HttpPut("PayRollApproveAll")]
    [CustomAccessKey(AccessKey: "update")]

    public IActionResult PayRollApproveAll([FromBody] PayRollApproveDTO req)
    {
        if (ModelState.IsValid)
        {
            try
            {

                List<long> validStatusId = new List<long>
            {
                (long)Enums.OrderStatus.FinalOrder,
                (long)Enums.OrderStatus.LastOrder,
            };


                var all = _orderService.All(IgnoreExpired: false)
               .Include(i => i.RecruitOrder)
               .Where(i => i.RecruitOrder.PayLocationId == currentUserDefaultOrganId && i.PayRollAprove != true && validStatusId.Contains(i.StatusId)).ToList()
                ;

                foreach (var item in all)
                {
                    try
                    {
                        req.InterdictId = item.Id;
                        _orderService.PayRollApprove(req);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

                return this.AppOk(OperationResult.Succeeded());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }


    [HttpPut("PayRollApprove")]
    [CustomAccessKey(AccessKey: "update")]

    public IActionResult PayRollApprove([FromBody] PayRollApproveDTO req)
    {
        if (ModelState.IsValid)
        {
            try
            {

                if (req.InterdictId > 0)
                {
                    return this.AppOk(_orderService.PayRollApprove(req));
                }
                else
                {
                    if (req.InterdictIdList == null)
                    {
                        return this.AppBadRequest(OperationResult.Failed("شناسه حکم ارسال نشده است"));
                    }
                    else
                    {
                        if (req.InterdictIdList.Any())
                        {
                            foreach (var item in req.InterdictIdList)
                            {
                                try
                                {
                                    req.InterdictId = item;
                                    _orderService.PayRollApprove(req);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex.Message, ex.Message);
                                }
                            }
                            return this.AppOk(OperationResult.Succeeded());
                        }
                        else
                        {
                            return this.AppBadRequest(OperationResult.Failed("شناسه حکم ارسال نشده است"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }

}
