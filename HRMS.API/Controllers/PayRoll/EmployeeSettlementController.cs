using System.ComponentModel;
using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HR.Identity.infrastructure.Services;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/EmployeeSettlement")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("تسویه حساب کارکنان")]
public class EmployeeSettlementController : AppBaseController
{
    private readonly EmployeeSettlementService _service;
    private readonly OrganisationEmployeeTypeSettlementItemService _settlementItemService;
    private readonly OrderService _orderService;
    private readonly UserPayLocationService _userPayLocationService;
    private readonly UserCostCenterService _userCostCenterService;

    public EmployeeSettlementController(
        EmployeeSettlementService service,
        OrganisationEmployeeTypeSettlementItemService settlementItemService,
        OrderService orderService,
        UserPayLocationService userPayLocationService,
        UserCostCenterService userCostCenterService,
        ILogger<EmployeeSettlementController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _settlementItemService = settlementItemService;
        _orderService = orderService;
        _userPayLocationService = userPayLocationService;
        _userCostCenterService = userCostCenterService;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _service._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
        _settlementItemService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetSettlementEligibilityByEmployeeId/{employeeId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetSettlementEligibilityByEmployeeId(long employeeId, [FromQuery] DateTime? settlementDate = null)
    {
        return this.AppOk(_service.GetSettlementEligibilityByEmployeeId(employeeId, settlementDate));
    }

    [HttpGet, Route("GetFicheCountInSettlementDateRange/{employeeId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetFicheCountInSettlementDateRange(
        long employeeId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        return this.AppOk(_service.GetFicheCountInSettlementDateRange(employeeId, startDate, endDate));
    }

    [HttpGet, Route("GetSettlementItemsByEmployeeId/{employeeId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetSettlementItemsByEmployeeId(long employeeId)
    {
        var interdictOrder = _orderService._unitOfWork.Context.InterdictOrders
            .Include(i => i.RecruitOrder)
            .Where(i => i.RecruitOrder.EmployeeId == employeeId
                && i.StatusId == (long)Enums.OrderStatus.FinalOrder
                && !i.IsDeleted)
            .OrderByDescending(i => i.Id)
            .FirstOrDefault();

        if (interdictOrder == null)
        {
            return this.AppNotFound("حکم نهایی برای کارمند یافت نشد");
        }

        var employeeTypeId = interdictOrder.RecruitOrder.EmployeeTypeId;
        var items = _settlementItemService.All()
            .Include(i => i.SettlementItem)
            .Include(i => i.PaymentType)
            .Include(i => i.EnterType)
            .Include(i => i.MeasurementUnit)
            .Include(i => i.OrganisationFormula)
                .ThenInclude(f => f.Formula)
            .Include(i => i.EmployeeType)
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.EmployeeTypeId == employeeTypeId)
            .OrderBy(i => i.Priority ?? int.MaxValue)
            .ThenBy(i => i.SettlementItem != null ? i.SettlementItem.title : string.Empty)
            .ThenByDescending(i => i.Id)
            .ToList();

        var dtos = _mapper.Map<List<OrganisationEmployeeTypeSettlementItemDTO>>(items);

        return this.AppOk(OperationResult.Succeeded(payload: dtos));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true,
        [FromQuery] long EmployeeId = 0)
    {
        if (EmployeeId <= 0)
            return this.AppBadRequest("EmployeeId is required");

        return this.AppOk(_service.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            EmployeeId: EmployeeId));
    }

    /// <summary>
    /// کارتابل سازمانی تسویه حساب — همه تسویه‌های واحد جاری کاربر (OrganisationChartId = currentUserDefaultOrganId).
    /// </summary>
    [HttpGet("GetOrganisationSettlementCartableList")]
    [CustomAccessKey(AccessKey: "GetOrganisationSettlementCartableList")]
    public IActionResult GetOrganisationSettlementCartableList(
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] long? costCenterId = null,
        [FromQuery] long? employeeTypeId = null,
        [FromQuery] long? settlementStatusId = null)
    {
        if (currentUserDefaultOrganId <= 0)
        {
            return this.AppBadRequest("واحد سازمانی جاری مشخص نیست");
        }

        var userPayLocationsResult = _userPayLocationService.GetAsKeyValuePair(currentUserId);
        if (!userPayLocationsResult.Success || userPayLocationsResult.Payload == null)
        {
            return this.AppBadRequest("خطا در دریافت دسترسی‌های محل پرداخت کاربر");
        }

        var userPayLocationIds = ((List<HR.SharedKernel.Data.KeyValuePair>)userPayLocationsResult.Payload)
            .Select(x => x.key)
            .ToList();

        if (!userPayLocationIds.Contains(currentUserDefaultOrganId))
        {
            return this.AppBadRequest("شما به واحد سازمانی جاری دسترسی ندارید");
        }

        if (costCenterId.HasValue && costCenterId.Value > 0)
        {
            var userCostCentersResult = _userCostCenterService.GetAsKeyValuePairByPayLocationId(
                currentUserId,
                currentUserDefaultOrganId);
            if (!userCostCentersResult.Success || userCostCentersResult.Payload == null)
            {
                return this.AppBadRequest("خطا در دریافت دسترسی‌های مرکز هزینه کاربر");
            }

            var userCostCenterIds = ((List<HR.SharedKernel.Data.KeyValuePair>)userCostCentersResult.Payload)
                .Select(x => x.key)
                .ToList();

            if (!userCostCenterIds.Contains(costCenterId.Value))
            {
                return this.AppBadRequest("شما به مرکز هزینه انتخاب شده دسترسی ندارید");
            }
        }

        return this.AppOk(_service.GetOrganisationSettlementCartableList(
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            costCenterId,
            employeeTypeId,
            settlementStatusId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] EmployeeSettlementDTO body)
    {
        body.OrganisationChartId = currentUserDefaultOrganId;
        body.title = "";
        return Ok(await _service.CreateForAsync(body));
    }


    [HttpGet("SendSettlementToCartable/{id}")]
    [CustomAccessKey(AccessKey: "SendSettlementToCartable")]
    public IActionResult SendSettlementToCartable(long id)
    {
        return this.AppOk(_service.SendSettlementToCartable(id));
    }

    [HttpGet("GetSettlementCartableDetail/{id}")]
    [CustomAccessKey(AccessKey: "GetSettlementCartableDetail")]
    public IActionResult GetSettlementCartableDetail(long id)
    {
        return this.AppOk(_service.GetSettlementCartableDetail(id));
    }

    [HttpPost("PreviewSettlement")]
    [CustomAccessKey(AccessKey: "CalculateSettlement")]
    public IActionResult PreviewSettlement([FromBody] EmployeeSettlementDTO? body)
    {
        if (body == null)
        {
            return this.AppBadRequest("بدنه درخواست خالی است. لطفاً اطلاعات فرم را ارسال کنید.");
        }

        body.OrganisationChartId = currentUserDefaultOrganId;
        return this.AppOk(_service.PreviewSettlementCalculation(body, buildTreeTrace: true));
    }

    [HttpPost("CalculateSettlement/{saveSettlement}")]
    [CustomAccessKey(AccessKey: "CalculateSettlement")]
    public IActionResult CalculateSettlement([FromBody] EmployeeSettlementDTO? body, int saveSettlement)
    {
        if (body == null)
        {
            return this.AppBadRequest("بدنه درخواست خالی است. لطفاً اطلاعات فرم را ارسال کنید.");
        }

        if (saveSettlement > 0)
        {
            return this.AppBadRequest(
                "ذخیره از مسیر محاسبه مجاز نیست. ابتدا پیش‌نمایش بگیرید و سپس از دکمه ذخیره (Post/Put) استفاده کنید.");
        }

        body.OrganisationChartId = currentUserDefaultOrganId;
        return this.AppOk(_service.PreviewSettlementCalculation(body, buildTreeTrace: true));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }

    [HttpGet("PrintPdf/{Id}/{id1}")]
    [CustomAccessKey(AccessKey: "PrintPdf")]
    public IActionResult PrintPdf(long Id, int id1)
    {
        var resp = _service.DownloadSettlementPDF(Id, id1 > 0);
        if (resp.Success == true && resp.Payload != null)
        {
            var stream = new MemoryStream(resp.Payload);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = "settlement.pdf",
            };
        }

        return BadRequest(resp);
    }

    [HttpGet("GetEmployeeSettlementArchiveStatus/{Id}")]
    [CustomAccessKey(AccessKey: "GetEmployeeSettlementArchiveStatus")]
    public IActionResult GetEmployeeSettlementArchiveStatus(long Id)
    {
        return this.AppOk(_service.GetEmployeeSettlementArchiveStatus(Id));
    }

    [HttpGet("DownloadEmployeeSettlementArchivePdf/{Id}/{isRaw}")]
    [CustomAccessKey(AccessKey: "DownloadEmployeeSettlementArchivePdf")]
    public IActionResult DownloadEmployeeSettlementArchivePdf(long Id, int isRaw)
    {
        var resp = _service.DownloadEmployeeSettlementArchivePdf(Id, isRaw > 0);
        if (resp.Success != true || resp.Payload is not byte[] pdfBytes)
        {
            return BadRequest(resp);
        }

        var stream = new MemoryStream(pdfBytes);
        return new FileStreamResult(stream, "application/pdf")
        {
            FileDownloadName = "settlement-archive.pdf",
        };
    }

    [HttpPost("RebuildEmployeeSettlementArchive/{Id}")]
    [CustomAccessKey(AccessKey: "RebuildEmployeeSettlementArchive")]
    public IActionResult RebuildEmployeeSettlementArchive(long Id)
    {
        return this.AppOk(_service.RebuildEmployeeSettlementArchive(Id));
    }
}
