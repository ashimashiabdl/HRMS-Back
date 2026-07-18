using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/Feedback")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("انتقادات و پیشنهادات")]
public class FeedbackController(FeedbackService Service, ILogger<FeedbackController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService)
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly FeedbackService _service = Service;

    [HttpPost("Create")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Create([FromBody] FeedbackDTO dto)
    {
        if (ModelState.IsValid)
        {
            var result = await _service.CreateFeedbackAsync(dto);
            return this.AppOk(result);
        }
        return this.AppBadRequest(ModelState);
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

    [HttpPut("Update/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Update(long id, [FromBody] FeedbackDTO dto)
    {
        if (ModelState.IsValid)
        {
            dto.Id = id;
            var result = await _service.UpdateForAsync(dto);
            return this.AppOk(result);
        }
        return this.AppBadRequest(ModelState);
    }

    [HttpPut("Respond/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Respond(long id, [FromBody] FeedbackDTO dto)
    {
        if (ModelState.IsValid)
        {
            var existing = _service.Get(id);
            if (existing.Success && existing.Payload is FeedbackDTO existingDto)
            {
                existingDto.Response = dto.Response;
                existingDto.ResponseDate = DateTime.Now;
                existingDto.RespondedByUserId = UserResolverService.GetUserId();
                existingDto.Status = "بررسی شده";
                var result = await _service.UpdateForAsync(existingDto);
                return this.AppOk(result);
            }
            return this.AppNotFound("انتقاد یا پیشنهاد یافت نشد");
        }
        return this.AppBadRequest(ModelState);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}

