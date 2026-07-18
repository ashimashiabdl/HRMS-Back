using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;


[Route("api/BlackList")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("فهرست سیاه")]
public class BlackListController : AppBaseController
{
    private readonly BlackListService _BlackListService;
    public BlackListController(BlackListService Service, ILogger<BlackListController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _BlackListService = Service;

        _BlackListService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        var resp = _BlackListService.Get(id);

        return this.AppOk(resp);
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (EmployeeId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        
        // اعمال فیلتر IgnoreExpired به صورت دستی بر روی CustomDataSource
        var Filtered = _BlackListService.All(IgnoreExpired)
            .Include(i => i.Employee)
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.EmployeeId == EmployeeId);
            
        if (string.IsNullOrEmpty(activeSortColumn))
        {
            activeSortColumn = "id";
        }
        
        // IgnoreExpired را false ارسال کنید چون قبلاً در Filtered اعمال شده است
        var resp = _BlackListService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired: false, CustomDataSource: Filtered,IgnoreDefaultOrganId:true);
        return this.AppOk(resp);
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] BlackListDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _BlackListService.CreateForAsync(body));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] BlackListDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _BlackListService.UpdateForAsync(body);
                return this.AppOk(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }
        else
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var Error in allErrors)
            {
                _logger.LogInformation(Error.ErrorMessage);
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_BlackListService.DeleteRecord(id));
    }
}
