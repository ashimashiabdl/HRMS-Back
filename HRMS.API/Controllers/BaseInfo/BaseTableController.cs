using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.Core.Interfaces;
using HR.SharedKernel;
using HR.SharedKernel.DTOs;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HR.SharedKernel.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using HR.BaseInfo.Core.DTOs;
using Microsoft.Build.Tasks;
using AutoMapper;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Attribute;
using System.ComponentModel;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/BaseTable")]
[DisplayName("اطلاعات پایه")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
public class BaseTableController : AppBaseController
{
    private readonly BaseTableService _baseTableService;
    public BaseTableController(BaseTableService BaseTableService, ILogger<BaseTableController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _baseTableService = BaseTableService;
        _baseTableService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_baseTableService.GetAsKeyValuePair());
    }
    [AllowAnonymous]
    [HttpGet, Route("GetCurrentOrganName")]
    
    public IActionResult GetCurrentOrganName()
    {
        return this.AppOk(_baseTableService.All().Single(i => i.Id == 40289));
    }

    [AllowAnonymous]
    [HttpGet, Route("GetCurrentOrganSubTitle")]
    public IActionResult GetCurrentOrganSubTitle()
    {
        return this.AppOk(_baseTableService.All().Single(i => i.Id == 40294));
    }
    [AllowAnonymous]
    [HttpGet, Route("GetCurrentOrganTagline")]
    public IActionResult GetCurrentOrganTagline()
    {
        return this.AppOk(_baseTableService.All().Single(i => i.Id == 40293));
    }
    [AllowAnonymous]
    [HttpGet, Route("GetCurrentOrganInfo")]
    public IActionResult GetCurrentOrganInfo()
    {
        const long OrganNameId = 40289;
        const long OrganSubTitleId = 40294;
        const long OrganTaglineId = 40293;
        const long OrganQuoteId = 40298;

        var organInfoById = _baseTableService.All()
            .AsNoTracking()
            .Where(i => i.Id == OrganNameId || i.Id == OrganSubTitleId || i.Id == OrganTaglineId || i.Id == OrganQuoteId)
            .ToDictionary(i => i.Id);

        var result = new
        {
            organName = organInfoById[OrganNameId],
            organSubTitle = organInfoById[OrganSubTitleId],
            organTagline = organInfoById[OrganTaglineId],
            organQuote = organInfoById.GetValueOrDefault(OrganQuoteId)
        };
        return this.AppOk(result);
    }
    [AllowAnonymous]
    [HttpGet, Route("GetMarqueeText")]
    public IActionResult GetMarqueeText()
    {
        return this.AppOk(_baseTableService.All().Single(i => i.Id == 40297));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_baseTableService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_baseTableService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] BaseTableDTO body)
    {
        return Ok(await _baseTableService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]


    public async Task<IActionResult> Put([FromBody] BaseTableDTO body)
    {
        var result = await _baseTableService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(OperationResult.Failed("امکان حذف جدول پایه سیستم وجود ندارد، لطفا با مدیر تماس بگیرید", payload: id));
    }
}
