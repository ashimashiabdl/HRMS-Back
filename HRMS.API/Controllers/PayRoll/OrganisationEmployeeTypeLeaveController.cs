using AutoMapper;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;


namespace HRMS.API.Controllers.PayRoll;

[Route("api/OrganisationEmployeeTypeLeave")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("مرخصی نوع استخدام سازمان")]
public class OrganisationEmployeeTypeLeaveController : AppBaseController
{
    private readonly OrganisationEmployeeTypeLeaveService _service;
    private readonly OrderService _orderService;
    public OrganisationEmployeeTypeLeaveController(OrderService OrderService, OrganisationEmployeeTypeLeaveService service, ILogger<OrganisationEmployeeTypeLeaveController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _orderService = OrderService;
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _service._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
    }
    [HttpGet, Route("GetAsKeyValuePairByEmployeeTypeId/{Id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair(long Id)
    {
        var lastOrder = _orderService._unitOfWork.Context.InterdictOrders.Include(i => i.RecruitOrder).Where(i => i.RecruitOrder.EmployeeId == Id && i.StatusId == (long)Enums.OrderStatus.FinalOrder);
        if (lastOrder != null)
        {
            if (lastOrder.Any())
            {
                long employeeTypeId = lastOrder.Single().RecruitOrder.EmployeeTypeId;
                return this.AppOk(OperationResult.Succeeded(payload: _service.All().Include(i => i.LeaveType).Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.EmployeeTypeId == employeeTypeId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
                {
                    key = i.LeaveTypeId,
                    value = i.LeaveType.title
                })));
            }
        }
        return this.AppNotFound();
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeLeaveDTO body)
    {
        body.title = "-";
        body.OrganisationChartId = currentUserDefaultOrganId;
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeLeaveDTO body)
    {
        body.title = "";
        var result = await _service.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}


