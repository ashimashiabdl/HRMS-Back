using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HR.SharedKernel.Upload;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/UserFileUpload")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("آپلود فایل کاربر")]
public class UserFileUploadController(UserFileUploadService Service, ILogger<UserFileUploadController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService, IOptions<FileUploadValidationOptions> uploadOptions)
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly UserFileUploadService _service = Service;
    private readonly FileUploadValidationOptions _uploadOptions = uploadOptions.Value;
    private const long RequestSizeLimitBytes = 25 * 1024 * 1024; // حد درخواست (بیشتر از MaxFileSizeBytes برای فرم)

    [HttpPost("UploadFile")]
    [CustomAccessKey(AccessKey: "create")]
    [RequestSizeLimit(RequestSizeLimitBytes)]
    public async Task<IActionResult> UploadFile([FromForm] string description, [FromForm] string? organizationName)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return this.AppBadRequest(OperationResult.Failed("توضیحات الزامی است"));
        }

        var formCollection = await Request.ReadFormAsync();
        var file = formCollection.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return this.AppBadRequest(OperationResult.Failed("فایل انتخاب نشده است"));
        }

        if (file.Length > _uploadOptions.MaxFileSizeBytes)
        {
            return this.AppBadRequest(OperationResult.Failed($"حداکثر حجم فایل {_uploadOptions.MaxFileSizeBytes / (1024 * 1024)} مگابایت است. لطفاً فایل را به چند قسمت کوچکتر تقسیم کنید."));
        }

        string fileExtension = System.IO.Path.GetExtension(file.FileName);
        var fileEntity = new HR.BaseInfo.Core.Entities.File
        {
            CreateDate = DateTime.Now,
            IPAddress = UserResolverService.GetIP(),
            title = HR.SharedKernel.Share.Helper.SanitizeFileName(file.FileName),
            IsDeleted = false,
            UniqueId = Guid.NewGuid(),
            Extension = fileExtension,
            Size = file.Length,
            MimeType = HR.SharedKernel.Share.Helper.GetMimeType(fileExtension),
        };

        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            fileEntity.Content = fileBytes;
        }

        _service._db.Set<HR.BaseInfo.Core.Entities.File>().Add(fileEntity);
        await _service._db.SaveChangesAsync();

        var dto = new UserFileUploadDTO
        {
            FileId = fileEntity.Id,
            Description = description,

            OrganizationName = organizationName
        };

        var result = await _service.CreateFromFileAsync(dto, fileEntity.Id);
        return this.AppOk(result);
    }

    [HttpGet("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage, int pageSize, string? filter, string? activeSortColumn, string? Sortdirection, bool IgnoreExpired = false)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpGet("DownloadFile/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> DownloadFile(long id)
    {
        var (content, mime, title) = await _service.GetFileContentAsync(id);
        if (content == null)
        {
            return this.AppNotFound("فایل یافت نشد");
        }
        var fileName = string.IsNullOrWhiteSpace(title) ? $"File_{id}" : title;
        return File(content, mime ?? "application/octet-stream", fileName);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }

    [HttpGet("GetUploadValidationSettings")]
    [CustomAccessKey(AccessKey: "GetUploadValidationSettings")]
    public IActionResult GetUploadValidationSettings()
    {
        var settings = new
        {
            MaxFileSizeBytes = _uploadOptions.MaxFileSizeBytes,
            AllowedExtensions = _uploadOptions.AllowedExtensions ?? new List<string>()
        };

        return this.AppOk(OperationResult.Succeeded(payload: settings));
    }
}

