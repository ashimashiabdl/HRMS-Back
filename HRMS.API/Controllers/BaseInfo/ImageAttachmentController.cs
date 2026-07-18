using System.ComponentModel;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/ImageAttachment")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("پیوست تصویر")]
public class ImageAttachmentController : AppBaseController
{
    private readonly ImageAttachmentService _service;

    public ImageAttachmentController(
        ImageAttachmentService service,
        ILogger<ImageAttachmentController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_service.GetAsKeyValuePair());
    }

    /// <summary>
    /// تصویر عمومی برای استفاده بدون احراز هویت (مثلاً تصویر فرم ورود). همیشه از ID=1 برای تصویر اصلی لاگین استفاده شود.
    /// </summary>
    [AllowAnonymous]
    [HttpGet, Route("PublicImage/{id}")]
    public IActionResult PublicImage(long id)
    {
        var result = _service.Get(id);
        if (!result.Success || result.Payload == null)
            return NotFound();
        var dto = result.Payload as ImageAttachmentDTO;
        if (dto?.Content == null || dto.Content.Length == 0)
            return NotFound();
        var mimeType = dto.MimeType ?? "image/jpeg";
        return File(dto.Content, mimeType);
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Post([FromForm] ImageAttachmentForm form)
    {
        var result = await _service.CreateFromFileAsync(form.File!, form.Title);
        if (!result.Success)
            return this.AppBadRequest(result.Message);
        return this.AppOk(result);
    }

    [HttpPut("Put/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Put(long id, [FromForm] ImageAttachmentForm form)
    {
        var result = await _service.UpdateFromFileAsync(id, form.File, form.Title);
        if (!result.Success)
            return this.AppBadRequest(result.Message);
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}
