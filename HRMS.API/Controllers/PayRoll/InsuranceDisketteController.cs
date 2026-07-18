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
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/InsuranceDiskette")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("دیسکت بیمه")]
public class InsuranceDisketteController : AppBaseController
{
    private readonly InsuranceDisketteService _InsuranceDisketteService;
    private readonly InsuranceDisketteItemService _InsuranceDisketteItemService;
    public InsuranceDisketteController(InsuranceDisketteService Service, InsuranceDisketteItemService InsuranceDisketteItemService, ILogger<InsuranceDisketteController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _InsuranceDisketteItemService = InsuranceDisketteItemService;
        _InsuranceDisketteService = Service;
        _InsuranceDisketteService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_InsuranceDisketteService.Get(id));
    }
    [HttpGet, Route("GetCurrentInsuranceDisketteCostCenters/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentInsuranceDisketteCostCenters(int id)
    {
        return this.AppOk(_InsuranceDisketteService.GetCurrentInsuranceDisketteCostCenters(id));
    }
    [HttpGet, Route("downloadDSKKAR00Disk/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadDSKKAR00Disk(int id)
    {
        return this.AppOk(_InsuranceDisketteService.downloadDSKKAR00Disk(id));
    }
    [HttpGet, Route("downloadDSKWOR00Disk/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadDSKWOR00Disk(int id)
    {
        return this.AppOk(_InsuranceDisketteService.downloadDSKWOR00Disk(id));
    }
    [HttpGet, Route("CheckIfPeriodIsValidForCreateInsuranceDiskette/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult CheckIfPeriodIsValidForCreateInsuranceDiskette(long id)
    {
        return this.AppOk(_InsuranceDisketteService.CheckIfPeriodIsValidForCreateInsuranceDiskette(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _InsuranceDisketteService._db.Set<HR.Payroll.Core.Data.InsuranceDiskette>()
                   .Include(i => i.InsuranceBranch)
                   .Include(i => i.PaymentPeriod)
                   .Include(i => i.ReportType)
                   .Include(i => i.PeymanRow)
                   .Include(i => i.InsuranceDisketteStatus)
                   .Include(i => i.BatchPayRollRequest)
                   .Where(DateValidityExtension<HR.Payroll.Core.Data.InsuranceDiskette>.GetDateValidationPredicate(IgnoreExpired)
                   .And(i => i.OrganisationChartId == currentUserDefaultOrganId)
                   );
        return this.AppOk(_InsuranceDisketteService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }

    [HttpGet, Route("GetPagedDetailData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedDetailData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long InsuranceDisketteId = 0)
    {
        var Filtered = _InsuranceDisketteService._db.Set<InsuranceDisketteItem>()
        .Include(i => i.Employee)

       .Where(i => i.InsuranceDisketteId == InsuranceDisketteId
       );

        return this.AppOk(_InsuranceDisketteItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public IActionResult Post([FromBody] InsuranceDisketteDTO body)
    {
        try
        {
            // بررسی ورودی
            if (body == null)
            {
                _logger.LogWarning("InsuranceDiskette Post: body is null");
                return this.AppBadRequest("اطلاعات دیسکت بیمه ارسال نشده است");
            }

            // تنظیم دوره پیش فرض
            body.PaymentPeriodId = currentUserDefaultPaymentPeiodId;

            // بررسی اینکه دوره پیش فرض تنظیم شده باشد
            if (!body.PaymentPeriodId.HasValue || body.PaymentPeriodId.Value <= 0)
            {
                _logger.LogWarning("InsuranceDiskette Post: PaymentPeriodId is invalid. UserId: {UserId}", currentUserId);
                return this.AppBadRequest("دوره پرداخت پیش فرض تنظیم نشده است");
            }

            // اعتبارسنجی سمت سرور
            _logger.LogInformation("InsuranceDiskette Post: Validating period {PeriodId} by user {UserId}", 
                body.PaymentPeriodId.Value, currentUserId);

            var serverSideValidation = _InsuranceDisketteService.CheckIfPeriodIsValidForCreateInsuranceDiskette(body.PaymentPeriodId.Value);
            
            if (serverSideValidation == null)
            {
                _logger.LogError("InsuranceDiskette Post: Validation service returned null for period {PeriodId}", 
                    body.PaymentPeriodId.Value);
                return this.AppBadRequest("خطا در اعتبارسنجی دوره");
            }

            if (serverSideValidation.Success == true)
            {
                _logger.LogInformation("InsuranceDiskette Post: Creating insurance diskette for period {PeriodId}", 
                    body.PaymentPeriodId.Value);

                var result = _InsuranceDisketteService.CreateForAsync(body);

                if (result == null)
                {
                    _logger.LogError("InsuranceDiskette Post: CreateForAsync returned null for period {PeriodId}", 
                        body.PaymentPeriodId.Value);
                    return this.AppBadRequest("خطا در ایجاد دیسکت بیمه");
                }

                if (result.Success)
                {
                    _logger.LogInformation("InsuranceDiskette Post: Successfully created insurance diskette for period {PeriodId}. Result: {@Result}", 
                        body.PaymentPeriodId.Value, result);
                }
                else
                {
                    _logger.LogWarning("InsuranceDiskette Post: Failed to create insurance diskette for period {PeriodId}. Message: {Message}", 
                        body.PaymentPeriodId.Value, result.Message);
                }

                return Ok(result);
            }
            else
            {
                _logger.LogWarning("InsuranceDiskette Post: Period validation failed for {PeriodId}. Message: {Message}", 
                    body.PaymentPeriodId.Value, serverSideValidation.Message);
                return this.AppBadRequest(serverSideValidation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsuranceDiskette Post: Unhandled exception occurred. UserId: {UserId}, PaymentPeriodId: {PeriodId}", 
                currentUserId, body?.PaymentPeriodId);
            return this.AppBadRequest("خطای غیرمنتظره در ایجاد دیسکت بیمه. لطفا با پشتیبانی تماس بگیرید");
        }
    }     

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_InsuranceDisketteService.DeleteRecord(id));
    }

    /// <summary>
    /// آپلود و نمایش محتوای فایل DBF (فرمت ایران سیستم)
    /// </summary>
    [HttpPost("UploadAndReadDbf")]
    [CustomAccessKey(AccessKey: "view")]
    [HRMS.API.Infrastructure.Upload.AllowUploadExtensions(".dbf")]
    public IActionResult UploadAndReadDbf(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return this.AppBadRequest("فایلی انتخاب نشده است");
        }

        if (!file.FileName.EndsWith(".dbf", StringComparison.OrdinalIgnoreCase) &&
            !file.FileName.EndsWith(".DBF", StringComparison.OrdinalIgnoreCase))
        {
            return this.AppBadRequest("فقط فایل‌های DBF قابل قبول هستند");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = _InsuranceDisketteService.ReadDbfFile(stream);
            
            if (result.Success)
            {
                return this.AppOk(result);
            }
            else
            {
                return this.AppBadRequest(result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading DBF file");
            return this.AppBadRequest($"خطا در خواندن فایل: {ex.Message}");
        }
    }
}
