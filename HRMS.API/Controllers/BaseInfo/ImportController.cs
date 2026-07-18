using System.ComponentModel;
using System.Text.Json;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Import;
using HR.SharedKernel.Share;
using HRMS.API.Cache;
using HRMS.API.Infrastructure.Upload;
using HRMS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/Import")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("Import اکسل")]
public class ImportController : AppBaseController
{
    private const long MaxFileSize = 20 * 1024 * 1024;

    private readonly GenericImportService _importService;
    private readonly ImportProfileCrudService _profileService;
    private readonly ImportContextService _importContextService;
    private readonly BaseInfoContext _baseInfoContext;
    private readonly UserResolverService _userResolverService;

    public ImportController(
        GenericImportService importService,
        ImportProfileCrudService profileService,
        ImportContextService importContextService,
        BaseInfoContext baseInfoContext,
        ILogger<ImportController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _importService = importService;
        _profileService = profileService;
        _importContextService = importContextService;
        _baseInfoContext = baseInfoContext;
        _userResolverService = userResolverService;
    }

    [HttpGet("GetProfiles")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetProfiles()
    {
        return this.AppOk(_profileService.GetActiveProfiles());
    }

    [HttpGet("GetProfile/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetProfile(long id)
    {
        var result = _profileService.GetProfileDetail(id);
        return result.Success ? this.AppOk(result) : this.AppNotFound();
    }

    [HttpGet("GetAllowedExtensions/{importProfileId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAllowedExtensions(long importProfileId)
    {
        var allowed = _importService.GetAllowedExtensions(importProfileId);
        return this.AppOk(OperationResult.Succeeded(payload: new { allowedExtensions = allowed }));
    }

    [HttpGet("GetTemplate/{importProfileId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetTemplate(long importProfileId, [FromQuery] int contextMode = 1)
    {
        var profile = _profileService.GetProfileDetail(importProfileId);
        if (!profile.Success || profile.Payload is not ImportProfileDetailDTO detail)
            return this.AppNotFound();

        var mode = contextMode == 2 ? ImportContextMode.RowExcelKeys : ImportContextMode.BatchContext;
        var content = _importService.GenerateTemplate(importProfileId, mode);
        if (content == null || content.Length == 0)
            return this.AppNotFound();

        var safeName = Helper.SanitizeFileName($"{detail.title}-template.xlsx");
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", safeName);
    }

    [HttpPost("DetectColumns")]
    [CustomAccessKey(AccessKey: "view")]
    [RequestSizeLimit(MaxFileSize)]
    [AllowUploadExtensions(".xlsx", ".csv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> DetectColumns([FromForm] ImportDetectColumnsForm form)
    {
        if (form.ImportProfileId <= 0)
            return this.AppBadRequest(OperationResult.Failed("انتخاب پروفایل Import الزامی است."));

        var file = form.File;
        if (file == null || file.Length == 0)
            return this.AppBadRequest(OperationResult.Failed("فایل انتخاب نشده است."));

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
        var allowed = _importService.GetAllowedExtensions(form.ImportProfileId);
        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            return this.AppBadRequest(OperationResult.Failed($"فرمت فایل مجاز نیست. مجاز: {string.Join(", ", allowed)}"));

        byte[] content;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            content = ms.ToArray();
        }

        var mode = form.ContextMode == 2 ? ImportContextMode.RowExcelKeys : ImportContextMode.BatchContext;
        var result = _importService.DetectColumns(content, ext, form.ImportProfileId, mode);
        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }

    [HttpPost("Upload")]
    [CustomAccessKey(AccessKey: "create")]
    [RequestSizeLimit(MaxFileSize)]
    [AllowUploadExtensions(".xlsx", ".csv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] ImportUploadForm form)
    {
        if (form.ImportProfileId <= 0)
            return this.AppBadRequest(OperationResult.Failed("انتخاب پروفایل Import الزامی است."));

        if (string.IsNullOrWhiteSpace(form.Title))
            return this.AppBadRequest(OperationResult.Failed("عنوان دسته Import الزامی است."));

        var file = form.File;
        if (file == null || file.Length == 0)
            return this.AppBadRequest(OperationResult.Failed("فایل انتخاب نشده است."));

        if (file.Length > MaxFileSize)
            return this.AppBadRequest(OperationResult.Failed("حداکثر حجم فایل ۲۰ مگابایت است."));

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
        var allowed = _importService.GetAllowedExtensions(form.ImportProfileId);
        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            return this.AppBadRequest(OperationResult.Failed($"فرمت فایل مجاز نیست. مجاز: {string.Join(", ", allowed)}"));

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

        List<ImportColumnMappingItem>? columnMapping = null;
        if (!string.IsNullOrWhiteSpace(form.ColumnMappingJson))
        {
            try
            {
                columnMapping = JsonSerializer.Deserialize<List<ImportColumnMappingItem>>(
                    form.ColumnMappingJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return this.AppBadRequest(OperationResult.Failed("فرمت نگاشت ستون‌ها نامعتبر است."));
            }
        }

        var profile = await _profileService.GetProfileWithFieldsAsync(form.ImportProfileId);
        if (profile == null)
            return this.AppBadRequest(OperationResult.Failed("پروفایل Import یافت نشد."));

        if (_importContextService.RequiresOrganisationContext(profile) && currentUserDefaultOrganId <= 0)
        {
            return this.AppBadRequest(OperationResult.Failed("سازمان پیش‌فرض مشخص نشده است. از منوی بالا واحد سازمانی/محل خدمت را انتخاب کنید."));
        }

        var mode = form.ContextMode == 2 ? ImportContextMode.RowExcelKeys : ImportContextMode.BatchContext;

        var result = await _importService.ProcessUploadAsync(
            content,
            ext,
            fileEntity.Id,
            form.ImportProfileId,
            form.Title,
            CurrentUserName,
            CurrentUserFullName,
            _userResolverService.GetIP(),
            columnMapping,
            form.ContextJson,
            currentUserDefaultOrganId,
            mode);

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
        return this.AppOk(_importService.GetPreviewPaged(batchId, currentPage, pageSize, filter, activeSortColumn, sortDirection));
    }

    [HttpGet("GetUploadReport/{batchId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetUploadReport(long batchId)
    {
        var result = _importService.GetUploadReport(batchId);
        return result.Success ? this.AppOk(result) : this.AppNotFound();
    }

    [HttpPost("Finalize/{batchId}")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Finalize(long batchId)
    {
        var result = await _importService.FinalizeAsync(batchId, _userResolverService.GetIP());
        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }
}
