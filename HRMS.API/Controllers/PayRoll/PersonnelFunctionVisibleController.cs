using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/PersonnelFunctionVisible")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("نمایش‌پذیری کارکرد کارکنان")]
public class PersonnelFunctionVisibleController(PersonnelFunctionVisibleService Service, ILogger<PersonnelFunctionVisibleController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly PersonnelFunctionVisibleService _service = Service;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;

        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] PersonnelFunctionVisibleDTO body)
    {
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        if (_service._unitOfWork.Context.PersonnelFunctionVisibles.Any(i => i.OrganisationChartId == currentUserDefaultOrganId))
        {
            return this.AppBadRequest("یک رکورد فعال وجود دارد ویرایش بفرمایید");
        }
        return Ok(await _service.CreateForAsync(body));
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] PersonnelFunctionVisibleDTO body)
    {
        body.title = "";
        var result = await _service.UpdateForAsync(body);
        return this.AppOk(result);
    }

    [HttpGet, Route("GetVisibilitySettings")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetVisibilitySettings()
    {
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        return this.AppOk(_service.GetVisibilitySettings());
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> Delete(int id)
    {
        return this.AppOk(await Task.Run(() => _service.DeleteRecord(id)));
    }
}


