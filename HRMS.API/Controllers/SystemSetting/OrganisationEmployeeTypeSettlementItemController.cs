using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationEmployeeTypeSettlementItem")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("آیتم تسویه نوع استخدام سازمان")]
public class OrganisationEmployeeTypeSettlementItemController : AppBaseController
{
    private readonly OrganisationEmployeeTypeSettlementItemService _service;

    public OrganisationEmployeeTypeSettlementItemController(OrganisationEmployeeTypeSettlementItemService service, ILogger<OrganisationEmployeeTypeSettlementItemController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{SelectedEmployeeType?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? SelectedEmployeeTypeId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] OrganisationEmployeeTypeSettlementItemDTO body)
    {
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] OrganisationEmployeeTypeSettlementItemDTO body)
    {
        return this.AppOk(await _service.UpdateForAsync(body));
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}
