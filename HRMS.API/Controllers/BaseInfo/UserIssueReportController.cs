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
using System.ComponentModel;
using HRMS.API.Infrastructure.Upload;
using HRMS.API.Infrastructure.Upload;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/UserIssueReport")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("اعلام خرابی/گزارش مشکل کاربر")]
public class UserIssueReportController(UserIssueReportService Service, ILogger<UserIssueReportController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly UserIssueReportService _service = Service;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage, int pageSize, [FromQuery] string? filter = "", [FromQuery] string? activeSortColumn = "", [FromQuery] string? Sortdirection = "", [FromQuery] bool IgnoreExpired = false, [FromQuery] long? CreatedByUserId = null)
    {
        // اگر activeSortColumn خالی باشد، به صورت پیش‌فرض بر اساس CreateDate به صورت نزولی مرتب می‌کنیم
        if (string.IsNullOrWhiteSpace(activeSortColumn))
        {
            activeSortColumn = "CreateDate";
            Sortdirection = "desc";
        }
        
        var result = _service.GetPagedData(currentPage, pageSize, filter ?? "", activeSortColumn, Sortdirection, IgnoreExpired);
        
        // Filter by CreatedByUserId if provided
        if (CreatedByUserId.HasValue && CreatedByUserId.Value > 0 && result.Success && result.Payload is List<UserIssueReportDTO> dtos)
        {
            result.Payload = dtos.Where(d => d.CreatedByUserId == CreatedByUserId.Value).ToList();
        }
        
        return this.AppOk(result);
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] UserIssueReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                return Ok(await _service.CreateForAsync(body));
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

    [HttpPost("UploadTempFile")]
    [CustomAccessKey(AccessKey: "create")]
    [RequestSizeLimit(1_000_000)] // 1 MB
    [SkipFileSignatureValidation] // Skip strict signature validation for user issue reports to allow various image formats
    public async Task<IActionResult> UploadTempFile()
    {
        try
        {
            if (!Request.HasFormContentType)
            {
                return this.AppBadRequest(OperationResult.Failed("فرمت درخواست نامعتبر است. لطفاً فایل را به صورت multipart/form-data ارسال کنید"));
            }

            var formCollection = await Request.ReadFormAsync();
            
            if (formCollection.Files == null || formCollection.Files.Count == 0)
            {
                return this.AppBadRequest(OperationResult.Failed("فایلی ارسال نشده است"));
            }

            var file = formCollection.Files.FirstOrDefault();
            if (file == null)
            {
                return this.AppBadRequest(OperationResult.Failed("فایل یافت نشد"));
            }

            if (file.Length == 0)
            {
                return this.AppBadRequest(OperationResult.Failed("فایل خالی است"));
            }
            
            if (file.Length > 1_000_000)
            {
                return this.AppBadRequest(OperationResult.Failed("حداکثر حجم فایل 1 مگابایت است"));
            }

            string FileExtn = System.IO.Path.GetExtension(file.FileName);
            var toAdd = new HR.BaseInfo.Core.Entities.TempGlobalFile()
            {
                CreateDate = DateTime.Now,
                IPAddress = UserResolverService.GetIP(),
                title = HR.SharedKernel.Share.Helper.SanitizeFileName(file.FileName),
                IsDeleted = false,
                UniqueId = Guid.NewGuid(),
                Extension = FileExtn,
                Size = file.Length,
                MimeType = HR.SharedKernel.Share.Helper.GetMimeType(FileExtn),
            };
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                toAdd.Content = fileBytes;
            }

            _service._db.Set<HR.BaseInfo.Core.Entities.TempGlobalFile>().Add(toAdd);
            await _service._db.SaveChangesAsync();
            return Ok(toAdd.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return this.AppBadRequest(OperationResult.Failed($"خطا در آپلود فایل: {ex.Message}"));
        }
    }

    [HttpPost("PostWithTempFile/{tempFileId}")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> PostWithTempFile([FromBody] UserIssueReportDTO body, long tempFileId)
    {
        if (ModelState.IsValid)
        {
            var res = await _service.CreateFromTempFileAsync(body, tempFileId);
            return this.AppOk(res);
        }
        return this.AppBadRequest(ModelState);
    }

    [HttpGet("DownloadAttachment/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> DownloadAttachment(long id)
    {
        var (content, mime, title) = await _service.GetAttachmentAsync(id);
        if (content == null)
        {
            return this.AppNotFound();
        }
        var fileName = string.IsNullOrWhiteSpace(title) ? $"Attachment_{id}" : title;
        return File(content, mime ?? "application/octet-stream", fileName);
    }

    [HttpPost("AddResponse/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> AddResponse(long id, [FromBody] AddResponseRequest request)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Response))
                {
                    return this.AppBadRequest(OperationResult.Failed("پاسخ نمی‌تواند خالی باشد"));
                }
                return Ok(await _service.AddResponseAsync(id, request.Response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
            return this.AppBadRequest(ModelState);
    }
}

public class AddResponseRequest
{
    public string Response { get; set; } = string.Empty;
}


