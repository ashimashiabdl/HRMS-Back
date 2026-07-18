using AutoMapper;
using HR.BaseInfo.infrastructure.Services;
using HR.Report.Core.DTOs;
using HR.Report.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace HRMS.API.Controllers.Report;

/// <summary>
/// کنترلر تحلیل اطلاعات ناقص
/// </summary>
[Route("api/MissingDataAnalysis")]
[ControllerGroup("Report", "گزارش گیری")]
[DisplayName("تحلیل اطلاعات ناقص")]
public class MissingDataAnalysisController : AppBaseController
{
    private readonly MissingDataAnalysisService _service;
    private readonly TempGlobalFileService _tempGlobalFileService;

    public MissingDataAnalysisController(
        MissingDataAnalysisService service,
        TempGlobalFileService tempGlobalFileService,
        ILogger<MissingDataAnalysisController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _tempGlobalFileService = tempGlobalFileService;
    }

    /// <summary>
    /// دریافت فهرست موجودیت‌های قابل بررسی
    /// </summary>
    [HttpGet("GetAvailableEntities")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAvailableEntities()
    {
        return this.AppOk(_service.GetAvailableEntities());
    }

    /// <summary>
    /// دریافت فیلدهای یک موجودیت
    /// </summary>
    [HttpGet("GetEntityFields/{entityId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetEntityFields(long entityId)
    {
        return this.AppOk(_service.GetEntityFields(entityId));
    }

    /// <summary>
    /// تحلیل اطلاعات ناقص
    /// </summary>
    [HttpPost("Analyze")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Analyze([FromBody] MissingDataAnalysisRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }

        try
        {
            var result = _service.AnalyzeMissingData(request);
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تحلیل اطلاعات ناقص");
            return this.AppBadRequest("خطا در تحلیل اطلاعات ناقص");
        }
    }

    /// <summary>
    /// دانلود گزارش اکسل اطلاعات ناقص
    /// </summary>
    [HttpPost("DownloadExcel")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult DownloadExcel([FromBody] MissingDataAnalysisRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }

        try
        {
            var result = _service.ExportToExcel(request);
            
            if (!result.Success)
            {
                return this.AppBadRequest(result.Message);
            }

            var fileId = (long)result.Payload;
            var file = _tempGlobalFileService.GetIdAsync(fileId).GetAwaiter().GetResult();
            
            if (file == null)
            {
                return this.AppBadRequest("فایل یافت نشد");
            }

            file.temp_inBase64 = Convert.ToBase64String(file.Content);
            file.Content = null;
            file.title = file.title?.Replace(" ", "_") ?? "گزارش_اطلاعات_ناقص";

            return Ok(HR.SharedKernel.DTOs.OperationResult.Succeeded(payload: file));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دانلود فایل اکسل");
            return this.AppBadRequest("خطا در دانلود فایل اکسل");
        }
    }
}

