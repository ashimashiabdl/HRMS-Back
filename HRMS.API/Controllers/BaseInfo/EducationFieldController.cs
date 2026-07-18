using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/EducationField")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("رشته های تحصیلی")]
public class EducationFieldController : AppBaseController
{
    private readonly EducationFieldService _EducationFieldService;

    public EducationFieldController(
        EducationFieldService Service,
        ILogger<EducationFieldController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService UserResolverService)
        : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _EducationFieldService = Service;
        _EducationFieldService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    /// <summary>
    /// جستجوی تنبل — فقط با تایپ کاربر نتیجه برمی‌گردد.
    /// </summary>
    [HttpGet, Route("GetAsKeyValuePairLazy/{filter}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairLazy(string filter)
    {
        return this.AppOk(_EducationFieldService.GetAsKeyValuePairLazy(filter));
    }

    [HttpGet, Route("GetAsKeyValuePairLazy")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairLazyEmpty()
    {
        return this.AppOk(_EducationFieldService.GetAsKeyValuePairLazy(string.Empty));
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_EducationFieldService.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_EducationFieldService.Get(id));
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
        return this.AppOk(_EducationFieldService.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] EducationFieldDTO body)
    {
        return Ok(await _EducationFieldService.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] EducationFieldDTO body)
    {
        return this.AppOk(await _EducationFieldService.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(OperationResult.Failed("امکان حذف جدول پایه سیستم وجود ندارد، لطفا با مدیر تماس بگیرید", payload: id));
    }
}
