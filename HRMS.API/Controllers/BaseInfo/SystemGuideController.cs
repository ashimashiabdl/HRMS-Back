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

[Route("api/SystemGuide")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
[DisplayName("راهنمای سامانه")]
public class SystemGuideController(SystemGuideService Service, ILogger<SystemGuideController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService)
    : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly SystemGuideService _service = Service;

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] SystemGuideDTO dto)
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

    [HttpGet("GetByTitle/{title}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetByTitle(string title)
    {
        return this.AppOk(_service.GetByTitle(title));
    }

    [HttpGet("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage, int pageSize, string? filter, string? activeSortColumn, string? Sortdirection, bool IgnoreExpired = false)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] SystemGuideDTO dto)
    {
        if (ModelState.IsValid)
        {
            if (dto.Id == null || dto.Id <= 0)
            {
                return this.AppBadRequest(OperationResult.Failed("شناسه معتبر نیست"));
            }
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
