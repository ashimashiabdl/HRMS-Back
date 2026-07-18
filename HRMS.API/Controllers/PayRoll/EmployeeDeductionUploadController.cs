using System.ComponentModel;
using AutoMapper;
using HR.BaseInfo.infrastructure.Data;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using HRMS.API.Cache;
using HRMS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/EmployeeDeductionUpload")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("آپلود گروهی کسورات")]
public class EmployeeDeductionUploadController : AppBaseController
{
    private static readonly string[] CsvExtensions = { ".csv" };
    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB

    private readonly EmployeeDeductionUploadService _uploadService;
    private readonly BaseInfoContext _baseInfoContext;
    private readonly IConfiguration _configuration;
    private readonly UserResolverService _userResolverService;

    public EmployeeDeductionUploadController(
        EmployeeDeductionUploadService uploadService,
        BaseInfoContext baseInfoContext,
        IConfiguration configuration,
        ILogger<EmployeeDeductionUploadController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _uploadService = uploadService;
        _baseInfoContext = baseInfoContext;
        _configuration = configuration;
        _userResolverService = userResolverService;
        _uploadService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    /// <summary>
    /// پسوندهای مجاز برای این آپلود (فقط CSV)
    /// </summary>
    private List<string> GetAllowedCsvExtensions()
    {
        return new List<string> { ".csv" };
    }

    [HttpPost("Upload")]
    [CustomAccessKey(AccessKey: "create")]
    [RequestSizeLimit(MaxFileSize)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] EmployeeDeductionUploadForm form)
    {
        if (currentUserDefaultOrganId <= 0)
            return this.AppBadRequest(OperationResult.Failed("سازمان پیش‌فرض مشخص نشده است."));

        if (string.IsNullOrWhiteSpace(form.Title))
            return this.AppBadRequest(OperationResult.Failed("عنوان دسته آپلود الزامی است."));

        if (form.DeductionTypeId <= 0)
            return this.AppBadRequest(OperationResult.Failed("انتخاب نوع کسور الزامی است."));

        if (form.StartDeductPaymentPeriodId <= 0)
            return this.AppBadRequest(OperationResult.Failed("انتخاب دوره پرداخت الزامی است."));

        var file = form.File;
        if (file == null || file.Length == 0)
            return this.AppBadRequest(OperationResult.Failed("فایل انتخاب نشده است."));

        if (file.Length > MaxFileSize)
            return this.AppBadRequest(OperationResult.Failed("حداکثر حجم فایل ۲۰ مگابایت است."));

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
        var allowed = GetAllowedCsvExtensions();
        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            return this.AppBadRequest(OperationResult.Failed($"فرمت فایل مجاز نیست. فقط فایل CSV پذیرفته می‌شود."));

        byte[] content;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            content = ms.ToArray();
        }

        var fileEntity = new HR.BaseInfo.Core.Entities.File
        {
            CreateDate = DateTime.Now,
            IPAddress = _userResolverService.GetIP(),
            title = Helper.SanitizeFileName(file.FileName),
            IsDeleted = false,
            UniqueId = Guid.NewGuid(),
            Extension = ext,
            Size = file.Length,
            MimeType = Helper.GetMimeType(ext),
            Content = content
        };
        _baseInfoContext.Files.Add(fileEntity);
        await _baseInfoContext.SaveChangesAsync();
        var fileId = fileEntity.Id;

        var result = await _uploadService.ProcessUploadAsync(
            content,
            fileId,
            form.Title,
            form.DeductionTypeId,
            currentUserDefaultOrganId,
            CurrentUserName,
            CurrentUserFullName,
            form.StartDeductPaymentPeriodId,
            form.PaymentDate);

        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }

    [HttpGet("GetPreviewPaged/{batchId}/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPreviewPaged(
        long batchId,
        int currentPage = 1,
        int pageSize = 10,
        [FromQuery] string? filter = null,
        [FromQuery] string? activeSortColumn = null,
        [FromQuery] string? sortDirection = null)
    {
        var result = _uploadService.GetPreviewPaged(batchId, currentPage, pageSize, filter, activeSortColumn, sortDirection);
        return this.AppOk(result);
    }

    [HttpGet("GetUploadReport/{batchId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetUploadReport(long batchId)
    {
        var result = _uploadService.GetUploadReport(batchId);
        return result.Success ? this.AppOk(result) : this.AppNotFound();
    }

    [HttpPost("Finalize/{batchId}")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Finalize(long batchId)
    {
        var result = await _uploadService.FinalizeAsync(batchId);
        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }

    [HttpGet("GetAllowedExcelExtensions")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAllowedExcelExtensions()
    {
        var allowed = GetAllowedCsvExtensions();
        return this.AppOk(OperationResult.Succeeded(payload: new { allowedExtensions = allowed }));
    }

    /// <summary>
    /// لیست صفحه‌بندی‌شده دسته‌های آپلود کسورات
    /// </summary>
    [HttpGet("GetBatchesPaged/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetBatchesPaged(
        int currentPage = 1,
        int pageSize = 10,
        [FromQuery] string? filter = null,
        [FromQuery] string? activeSortColumn = null,
        [FromQuery] string? sortDirection = null)
    {
        if (currentUserDefaultOrganId <= 0)
            return this.AppBadRequest(OperationResult.Failed("سازمان پیش‌فرض مشخص نشده است."));
        var result = _uploadService.GetBatchesPaged(currentPage, pageSize, filter, activeSortColumn, sortDirection);
        return this.AppOk(result);
    }

    /// <summary>
    /// حذف دسته آپلود و وابستگی‌ها در صورت استفاده نشدن در فیش؛ وگرنه پیام خطا
    /// </summary>
    [HttpDelete("DeleteBatch/{batchId}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> DeleteBatch(long batchId)
    {
        var result = await _uploadService.TryDeleteBatchAsync(batchId);
        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }
}
