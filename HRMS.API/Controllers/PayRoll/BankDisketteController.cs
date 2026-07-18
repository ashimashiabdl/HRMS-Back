using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Organisation.Infrastructure.Services;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/BankDiskette")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("دیسکت بانک")]
public class BankDisketteController : AppBaseController
{
    private readonly BankDisketteService _BankDisketteService;
    private readonly BankDisketteItemService _BankDisketteItemService;
    private readonly BankDisketteGroupAndFileService _bankDisketteGroupAndFileService;
    public BankDisketteController(BankDisketteService Service, BankDisketteGroupAndFileService BankDisketteGroupAndFileService, BankDisketteItemService BankDisketteItemService, ILogger<BankDisketteController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {

        _bankDisketteGroupAndFileService = BankDisketteGroupAndFileService;
        _BankDisketteItemService = BankDisketteItemService;
        _BankDisketteService = Service;
        _BankDisketteService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_BankDisketteService.Get(id));
    }
    [HttpGet, Route("GetCurrentBankDisketteCostCenters/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentBankDisketteCostCenters(int id)
    {
        return this.AppOk(_BankDisketteService.GetCurrentBankDisketteCostCenters(id));
    }
    [HttpGet, Route("GetCurrentDisketteFilesBySepration/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentDisketteFilesBySepration(int id)
    {
        return this.AppOk(_BankDisketteService.GetCurrentDisketteFilesBySepration(id));
    }
    [HttpGet, Route("downloadBankDisk/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadBankDisk(int id)
    {
        return this.AppOk(_BankDisketteService.downloadBankDisk(id));
    }
    [HttpGet, Route("CheckIfPeriodIsValidForCreateBankDisk/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult CheckIfPeriodIsValidForCreateBankDisk(long id)
    {
        return this.AppOk(_BankDisketteService.CheckIfPeriodIsValidForCreateBankDisk(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _BankDisketteService._db.Set<BankDiskette>()
       .Include(i => i.PaymentPeriod)
       .Include(i => i.BankDisketteStatus)
       //.Include(i => i.BankDisketteTemplate.Bank)
       .Where(DateValidityExtension<BankDiskette>.GetDateValidationPredicate(IgnoreExpired)
       .And(i => i.OrganisationChartId == currentUserDefaultOrganId)
       );
        return this.AppOk(_BankDisketteService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpGet, Route("GetPagedDetailData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedDetailData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long BankDisketteId = 0, [FromQuery] long BankDisketteGroupAndFileId = 0)
    {
        var Filtered = _BankDisketteService._db.Set<BankDisketteItem>()
            .Include(i => i.Employee)
        .Where(i => i.BankDisketteId == BankDisketteId && i.BankDisketteGroupAndFileId == BankDisketteGroupAndFileId);

        //);
        return this.AppOk(_BankDisketteItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: Filtered));
    }


    [HttpGet, Route("GetPagedDetailErrorData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedDetailErrorData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long BankDisketteId = 0)
    {
        var Filtered = _BankDisketteService._db.Set<BankDisketteItem>()
            .Include(i => i.Employee)
        .Where(i => i.BankDisketteId == BankDisketteId && i.BankDisketteGroupAndFileId == null);

        //);
        return this.AppOk(_BankDisketteItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: Filtered));
    }

    [HttpGet, Route("GetPagedDetailGroupData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedDetailGroupData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long BankDisketteId = 0)
    {
        var Filtered = _BankDisketteService._db.Set<BankDisketteGroupAndFile>()
            .Include(i => i.BankDisketteTemplate.Bank)
        .Where(i => i.BankDisketteId == BankDisketteId);


        var paged = _bankDisketteGroupAndFileService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: Filtered);

        foreach (var item in (List<BankDisketteGroupAndFileDTO>)paged.Payload)
        {
            item.PersonCount = _BankDisketteItemService.All().Where(i => i.BankDisketteId == BankDisketteId && i.BankDisketteGroupAndFileId == Convert.ToInt64(item.Id)).Count();
            item.SumAmount = _BankDisketteItemService.All().Where(i => i.BankDisketteId == BankDisketteId && i.BankDisketteGroupAndFileId == Convert.ToInt64(item.Id)).Sum(i => i.Amount);
        }

        return this.AppOk(paged);
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public IActionResult Post([FromBody] BankDisketteDTO body)
    {
        try
        {
            // بررسی ورودی
            if (body == null)
            {
                _logger.LogWarning("BankDiskette Post: body is null");
                return this.AppBadRequest("اطلاعات دیسکت بانک ارسال نشده است");
            }

            // بررسی ModelState
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("BankDiskette Post: ModelState is invalid");
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var Error in allErrors)
                {
                    _logger.LogWarning("BankDiskette Post: ModelState error: {ErrorMessage}", Error.ErrorMessage);
                }
                return this.AppBadRequest(ModelState);
            }

            // تنظیم دوره پیش فرض
            body.PaymentPeriodId = currentUserDefaultPaymentPeiodId;

            // بررسی اینکه دوره پیش فرض تنظیم شده باشد
            if (!body.PaymentPeriodId.HasValue || body.PaymentPeriodId.Value <= 0)
            {
                _logger.LogWarning("BankDiskette Post: PaymentPeriodId is invalid. UserId: {UserId}", currentUserId);
                return this.AppBadRequest("دوره پرداخت پیش فرض تنظیم نشده است");
            }

            // اعتبارسنجی سمت سرور
            _logger.LogInformation("BankDiskette Post: Validating period {PeriodId} by user {UserId}", 
                body.PaymentPeriodId.Value, currentUserId);

            var serverSideValidation = _BankDisketteService.CheckIfPeriodIsValidForCreateBankDisk(body.PaymentPeriodId.Value);
            
            if (serverSideValidation == null)
            {
                _logger.LogError("BankDiskette Post: Validation service returned null for period {PeriodId}", 
                    body.PaymentPeriodId.Value);
                return this.AppBadRequest("خطا در اعتبارسنجی دوره");
            }

            if (serverSideValidation.Success == true)
            {
                // بررسی نوع محاسبه
                if (body.CalculateAllFichesInCurrentPeriod == true)
                {
                    body.CostCenterIdList = new List<long>();
                    _logger.LogInformation("BankDiskette Post: Calculate all fiches mode for period {PeriodId}", body.PaymentPeriodId.Value);
                }
                else
                {
                    if (body.CostCenterIdList == null || !body.CostCenterIdList.Any())
                    {
                        _logger.LogWarning("BankDiskette Post: CostCenterIdList is null or empty for period {PeriodId}", body.PaymentPeriodId.Value);
                        return this.AppBadRequest("فهرست مراکز هزینه ارسال نشده است!");
                    }
                    _logger.LogInformation("BankDiskette Post: Creating diskette with {CostCenterCount} cost centers for period {PeriodId}", 
                        body.CostCenterIdList.Count, body.PaymentPeriodId.Value);
                }

                // ایجاد دیسکت بانک
                _logger.LogInformation("BankDiskette Post: Creating bank diskette for period {PeriodId}", 
                    body.PaymentPeriodId.Value);

                var result = _BankDisketteService.CreateForAsync(body);

                if (result == null)
                {
                    _logger.LogError("BankDiskette Post: CreateForAsync returned null for period {PeriodId}", 
                        body.PaymentPeriodId.Value);
                    return this.AppBadRequest("خطا در ایجاد دیسکت بانک");
                }

                if (result.Success)
                {
                    _logger.LogInformation("BankDiskette Post: Successfully created bank diskette for period {PeriodId}. Result: {@Result}", 
                        body.PaymentPeriodId.Value, result);
                }
                else
                {
                    _logger.LogWarning("BankDiskette Post: Failed to create bank diskette for period {PeriodId}. Message: {Message}", 
                        body.PaymentPeriodId.Value, result.Message);
                }

                return Ok(result);
            }
            else
            {
                _logger.LogWarning("BankDiskette Post: Period validation failed for {PeriodId}. Message: {Message}", 
                    body.PaymentPeriodId.Value, serverSideValidation.Message);
                return this.AppOk(serverSideValidation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BankDiskette Post: Unhandled exception occurred. UserId: {UserId}, PaymentPeriodId: {PeriodId}", 
                currentUserId, body?.PaymentPeriodId);
            return this.AppBadRequest("خطای غیرمنتظره در ایجاد دیسکت بانک. لطفا با پشتیبانی تماس بگیرید");
        }
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_BankDisketteService.DeleteRecord(id));
    }
}
