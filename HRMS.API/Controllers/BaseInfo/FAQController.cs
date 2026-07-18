using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/FAQ")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("سوالات متداول")]
public class FAQController(FAQService Service, ILogger<FAQController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService)
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly FAQService _service = Service;

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] FAQDTO dto)
    {
        if (ModelState.IsValid)
        {
            var result = await _service.CreateForAsync(dto);
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

    [HttpGet("GetActiveFAQs")]
    [AllowAnonymous]
    public IActionResult GetActiveFAQs()
    {
        return this.AppOk(_service.GetActiveFAQs());
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] FAQDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (dto.Id == null || dto.Id <= 0)
            {
                return this.AppBadRequest(OperationResult.Failed("شناسه سوال معتبر نیست"));
            }
            var result = await _service.UpdateForAsync(dto);
            return this.AppOk(result);
        }
        return this.AppBadRequest(ModelState);
    }

    [HttpPut("Update/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Update(long id, [FromBody] FAQDTO dto)
    {
        if (ModelState.IsValid)
        {
            dto.Id = id;
            var result = await _service.UpdateForAsync(dto);
            return this.AppOk(result);
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

