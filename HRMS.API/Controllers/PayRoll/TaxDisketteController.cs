using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Models;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/TaxDiskette")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("دیسکت مالیات")]
public class TaxDisketteController : AppBaseController
{
    private readonly TaxDisketteService _TaxDisketteService;
    private readonly TaxDisketteWhService _taxDisketteWhService;
    private readonly TaxDisketteWpService _taxDisketteWpService;
    private readonly TaxDisketteWkService _taxDisketteWkService;
    public TaxDisketteController(TaxDisketteService Service, TaxDisketteWhService TaxDisketteWhService, TaxDisketteWpService TaxDisketteWpService, TaxDisketteWkService TaxDisketteWkService, ILogger<TaxDisketteController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _taxDisketteWhService = TaxDisketteWhService;
        _taxDisketteWpService = TaxDisketteWpService;
        _taxDisketteWkService = TaxDisketteWkService;
        _TaxDisketteService = Service;
        _TaxDisketteService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("CheckTaxResponseFileExists/{batchPayRollRequestId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult CheckTaxResponseFileExists(long batchPayRollRequestId)
    {
        return this.AppOk(_TaxDisketteService.CheckTaxResponseFileExists(batchPayRollRequestId));
    }
    [HttpGet, Route("downloadTaxDisk/{id}/{id1}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadTaxDisk(long id, long id1)
    {
        return this.AppOk(_TaxDisketteService.downloadTaxDisk(id, id1));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_TaxDisketteService.Get(id));
    }
    [HttpGet, Route("GetCurrentTaxDisketteCostCenters/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetCurrentTaxDisketteCostCenters(int id)
    {
        return this.AppOk(_TaxDisketteService.GetCurrentTaxDisketteCostCenters(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_TaxDisketteService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpGet, Route("GetPagedDetailData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedDetailData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long TaxDisketteId = 0)
    {
        var Filtered = _taxDisketteWhService.All(false)
       .Include(i => i.Employee)
       .Where(i => i.TaxDisketteId == TaxDisketteId
        );
        return this.AppOk(_taxDisketteWhService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }

    [HttpGet, Route("GetPagedWPDetailData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedWPDetailData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long TaxDisketteId = 0)
    {
        var Filtered = _taxDisketteWpService.All(false)
           .Include(i => i.Employee)
           .Where(i => i.TaxDisketteId == TaxDisketteId);
        return this.AppOk(_taxDisketteWpService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
    }

    [HttpGet, Route("GetWKSummary/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetWKSummary(long id)
    {
        var row = _taxDisketteWkService.All(false)
            .Include(i => i.PaymentType)
            .SingleOrDefault(i => i.TaxDisketteId == id);
        if (row == null)
        {
            // Return an empty summary object instead of 404 so UI can render zeros
            var empty = new HR.Payroll.Core.DTOs.TaxDisketteWkDTO()
            {
                TaxDisketteId = id,
                PaymentTypeId = 0,
                WorkplaceStatusId = 0,
                PurePaymentAmount = 0,
                ExceptionsSubjectToTheBudgetLawOf1404 = 0,
                CurrencyCode = 0,
                ExchangeRateOfCurrency = 0
            };
            return this.AppOk(empty);
        }
        return this.AppOk(_mapper.Map<HR.Payroll.Core.DTOs.TaxDisketteWkDTO>(row));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public IActionResult Post([FromBody] TaxDisketteDTO body)
    {
        try
        {
            body.PaymentPeriodId = currentUserDefaultPaymentPeiodId;
            var result = _TaxDisketteService.CreateForAsync(body);
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایجاد دیسکت مالیات");
            return this.AppBadRequest($"خطا در ایجاد دیسکت مالیات: {ex.Message}");
        }
    }
    [HttpGet, Route("CheckIfPeriodIsValidForCreateTaxDiskette/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult CheckIfPeriodIsValidForCreateTaxDiskette(long id)
    {
        return this.AppOk(_TaxDisketteService.CheckIfPeriodIsValidForCreateTaxDiskette(id));
    }        //[HttpPut("Put")]
    //
    //public async Task<IActionResult> Put([FromBody] TaxDisketteDTO body)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            
    //            var result = await _TaxDisketteService.UpdateForAsync(body);
    //            return this.AppOk(result);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, ex.Message);
    //            return this.AppBadRequest("Internal Server Error");
    //        }
    //    }
    //    else
    //    {
    //        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
    //        foreach (var Error in allErrors)
    //        {
    //            _logger.LogInformation(Error.ErrorMessage);
    //        }
    //    }
    //    return this.AppBadRequest(ModelState);
    //}

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_TaxDisketteService.DeleteRecord(id));
    }

    [HttpPost("UploadTaxResponse")]
    [CustomAccessKey(AccessKey: "create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadTaxResponse([FromForm] TaxResponseUploadForm form)
    {
        var file = form.File;
        if (file == null || file.Length == 0)
        {
            return this.AppBadRequest("فایل ارسال نشده است");
        }

        if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return this.AppBadRequest("فقط فایل‌های TXT مجاز هستند");
        }

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            
            var result = await _TaxDisketteService.ProcessTaxResponseFile(form.BatchPayRollRequestId, content);
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در آپلود و پردازش فایل محاسبه مالیات - BatchPayRollRequestId: {BatchPayRollRequestId}", form.BatchPayRollRequestId);
            return this.AppBadRequest($"خطا در پردازش فایل: {ex.Message}");
        }
    }

  

    [HttpGet, Route("GetDiscrepancyList/{batchPayRollRequestId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetDiscrepancyList(long batchPayRollRequestId)
    {
        return this.AppOk(_TaxDisketteService.GetDiscrepancyList(batchPayRollRequestId));
    }
}
