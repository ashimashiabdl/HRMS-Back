using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.Core.DTOs;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Data;
using HR.Order.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Order;

[Route("api/BatchRequestDetail")]
[ControllerGroup("OrderNameSpace", " احکام")]
[DisplayName("جزئیات گروهی احکام")]
public class BatchRequestDetailController : AppBaseController
{
    private readonly BatchRequestDetailService _batchRequestService;
    private OrderContext _context;
    public BatchRequestDetailController(OrderContext OrderContext, BatchRequestDetailService BatchRequestDetailService, ILogger<OrderController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = OrderContext;
        _batchRequestService = BatchRequestDetailService;
        _batchRequestService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_batchRequestService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long BatchRequestId = 0)
    {
        var Filtered = _context.BatchRequestDetails.Include(i => i.Employee).Include(i => i.InterdictOrder).Include(i => i.InterdictOrder.Status).Where(DateValidityExtension<BatchRequestDetail>.GetDateValidationPredicate().And(i => i.BatchRequestId == BatchRequestId));
        var paged = _batchRequestService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered);

        return this.AppOk(paged);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] BatchRequestDetailDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _batchRequestService.UpdateForAsync(body);
                return this.AppOk(result);
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
