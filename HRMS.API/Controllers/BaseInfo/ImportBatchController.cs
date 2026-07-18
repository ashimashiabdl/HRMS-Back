using System.ComponentModel;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/ImportBatch")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("دسته Import")]
public class ImportBatchController : AppBaseController
{
    private readonly ImportBatchAdminService _service;
    private readonly GenericImportService _importService;
    private readonly UserResolverService _userResolverService;

    public ImportBatchController(
        ImportBatchAdminService service,
        GenericImportService importService,
        UserResolverService userResolverService,
        ILogger<ImportBatchController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _importService = importService;
        _userResolverService = userResolverService;
    }

    [HttpGet("GetPagedData/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        [FromQuery] string? filter = null,
        [FromQuery] string? activeSortColumn = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] long? importProfileId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, sortDirection, importProfileId));
    }

    [HttpGet("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        var result = _service.GetDetail(id);
        return result.Success ? this.AppOk(result) : this.AppNotFound();
    }

    [HttpGet("GetTempRowsPaged/{batchId}/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetTempRowsPaged(
        long batchId,
        int currentPage = 1,
        int pageSize = 10,
        [FromQuery] string? filter = null,
        [FromQuery] string? activeSortColumn = null,
        [FromQuery] string? sortDirection = null)
    {
        return this.AppOk(_service.GetTempRowsPaged(batchId, currentPage, pageSize, filter, activeSortColumn, sortDirection));
    }

    [HttpGet("DownloadUploadedFile/{batchId}")]
    [CustomAccessKey(AccessKey: "download")]
    public IActionResult DownloadUploadedFile(long batchId)
    {
        var result = _service.DownloadUploadedFile(batchId);
        if (!result.Success || result.Payload is not ImportFileDownloadDTO file || file.Content == null)
            return this.AppBadRequest(result.Success ? OperationResult.Failed("فایل یافت نشد.") : result);

        var fileName = ComposeDownloadFileName(
            string.IsNullOrWhiteSpace(file.FileName) ? $"Import_Batch_{batchId}" : file.FileName,
            file.Extension);

        return File(file.Content, file.MimeType ?? "application/octet-stream", fileName);
    }

    private static string ComposeDownloadFileName(string? fileName, string? extension)
    {
        var baseName = string.IsNullOrWhiteSpace(fileName) ? "import-file" : fileName.Trim();
        if (string.IsNullOrWhiteSpace(extension))
            return baseName;

        var ext = extension.StartsWith('.') ? extension : "." + extension;
        var existingExt = Path.GetExtension(baseName);
        if (!string.IsNullOrEmpty(existingExt)
            && string.Equals(existingExt, ext, StringComparison.OrdinalIgnoreCase))
        {
            return baseName;
        }

        return baseName + ext;
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

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _service.DeleteBatchAsync(id);
        return result.Success ? this.AppOk(result) : this.AppBadRequest(result);
    }
}
