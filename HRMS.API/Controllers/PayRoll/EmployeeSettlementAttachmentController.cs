using System.ComponentModel;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Upload;
using HRMS.API.Cache;
using HRMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/EmployeeSettlementAttachment")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("پیوست سند تسویه حساب")]
public class EmployeeSettlementAttachmentController : AppBaseController
{
    private readonly EmployeeSettlementAttachmentService _service;
    private readonly FileUploadValidationOptions _uploadOptions;

    public EmployeeSettlementAttachmentController(
        EmployeeSettlementAttachmentService service,
        IOptions<FileUploadValidationOptions> uploadOptions,
        ILogger<EmployeeSettlementAttachmentController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _uploadOptions = uploadOptions.Value;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet("GetBySettlementId/{settlementId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetBySettlementId(long settlementId)
    {
        return this.AppOk(_service.GetBySettlementId(settlementId));
    }

    [HttpPost("Upload")]
    [CustomAccessKey(AccessKey: "create")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] EmployeeSettlementAttachmentUploadForm form)
    {
        if (form.SettlementId <= 0)
        {
            return this.AppBadRequest("ابتدا تسویه حساب را ذخیره کنید؛ آپلود پیوست فقط پس از دریافت شناسه تسویه امکان‌پذیر است");
        }

        if (form.File == null)
        {
            return this.AppBadRequest("فایل انتخاب نشده است");
        }

        var result = await _service.UploadAsync(
            form.SettlementId,
            form.SettlementDocumentAttachmentTypeId,
            form.File,
            form.Description);

        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }

    [HttpGet("Download/{id}")]
    [CustomAccessKey(AccessKey: "Download")]
    public async Task<IActionResult> Download(long id)
    {
        var (content, mimeType, fileName, error) = await _service.GetFileForDownloadAsync(id);
        if (error != null)
        {
            return this.AppBadRequest(error);
        }

        return File(content!, mimeType ?? "application/octet-stream", fileName);
    }

    [HttpGet("GetUploadValidationSettings")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetUploadValidationSettings()
    {
        var settings = new
        {
            MaxFileSizeBytes = _uploadOptions.MaxFileSizeBytes,
            AllowedExtensions = _uploadOptions.AllowedExtensions ?? [],
        };

        return this.AppOk(OperationResult.Succeeded(payload: settings));
    }
}
