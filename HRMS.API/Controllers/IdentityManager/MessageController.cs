using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/Message")]
[ControllerGroup("IdentityManager", "مدیریت احراز هویت")]
[DisplayName("پیام‌ها")]
public class MessageController(MessageService Service, ILogger<MessageController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) 
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly MessageService _service = Service;

    [HttpGet, Route("GetInbox")]
    [CustomAccessKey(AccessKey: "GetInbox")]
    public IActionResult GetInbox(
        [FromQuery] bool showAsReceiver = true,
        [FromQuery] int currentPage = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string sortDirection = "",
        [FromQuery] long? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        return this.AppOk(_service.GetInbox(showAsReceiver, currentPage, pageSize, filter, activeSortColumn, sortDirection, userId, fromDate, toDate));
    }

    [HttpGet, Route("GetMessageWithThread/{id}")]
    [CustomAccessKey(AccessKey: "GetMessageWithThread")]
    public IActionResult GetMessageWithThread(long id)
    {
        return this.AppOk(_service.GetMessageWithThread(id));
    }

    [HttpPost("SendMessage")]
    [CustomAccessKey(AccessKey: "SendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO body)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }
        return Ok(await _service.SendMessage(body));
    }

    [HttpGet, Route("DownloadAttachment/{id}")]
    [CustomAccessKey(AccessKey: "DownloadAttachment")]
    public IActionResult DownloadAttachment(long id)
    {
        return this.AppOk(_service.DownloadAttachment(id));
    }

    [HttpGet, Route("GetActiveUsers")]
    [CustomAccessKey(AccessKey: "GetActiveUsers")]
    public IActionResult GetActiveUsers()
    {
        return this.AppOk(_service.GetActiveUsers());
    }

    [HttpGet, Route("GetUnreadMessageCount")]
    [CustomAccessKey(AccessKey: "GetUnreadMessageCount")]
    public IActionResult GetUnreadMessageCount()
    {
        return this.AppOk(_service.GetUnreadMessageCount());
    }

    [HttpPut("MarkAsRead/{id}")]
    [CustomAccessKey(AccessKey: "MarkAsRead")]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        try
        {
            var result = _service.GetMessageWithThread(id);
            // GetMessageWithThread already marks as read
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return this.AppBadRequest("خطا در بروزرسانی وضعیت پیام");
        }
    }

    [HttpGet, Route("GetAllMessages")]
    [CustomAccessKey(AccessKey: "GetAllMessages")]
    public IActionResult GetAllMessages([FromQuery] long? senderId = null, [FromQuery] long? receiverId = null)
    {
        return this.AppOk(_service.GetAllMessages(senderId, receiverId));
    }

    [HttpPost("SendMessageToAllUsers")]
    [CustomAccessKey(AccessKey: "SendMessageToAllUsers")]
    public async Task<IActionResult> SendMessageToAllUsers([FromBody] SendMessageDTO body)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }
        return this.AppOk(await _service.SendMessageToAllUsers(body));
    }

    [HttpGet, Route("DownloadAttachmentFile/{id}")]
    [CustomAccessKey(AccessKey: "DownloadAttachmentFile")]
    public IActionResult DownloadAttachmentFile(long id)
    {
        var result = _service.DownloadAttachment(id);
        if (!result.Success)
        {
            return this.AppBadRequest(result.Message);
        }

        var attachment = result.Payload as MessageAttachmentDTO;
        if (attachment == null || string.IsNullOrEmpty(attachment.ContentBase64))
        {
            return this.AppBadRequest("فایل یافت نشد");
        }

        var fileBytes = Convert.FromBase64String(attachment.ContentBase64);
        var fileName = attachment.FileName;
        if (!string.IsNullOrEmpty(attachment.Extension) && !fileName.EndsWith(attachment.Extension))
        {
            fileName += attachment.Extension;
        }

        return File(fileBytes, attachment.MimeType ?? "application/octet-stream", fileName);
    }
}
