using HR.SharedKernel.Attribute;
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
using HR.Payroll.Core.DTOs;
using Hr.Employee.infrastructure.Services;
using HRMS.API.Controllers.PayRoll;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/TaminInsuranceJobList")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("فهرست شغل های تامین اجتماعی")]
public class TaminInsuranceJobListController : AppBaseController
{
    private readonly TaminInsuranceJobListService _TaminInsuranceJobListService;
    public TaminInsuranceJobListController(TaminInsuranceJobListService TaminInsuranceJobListService, ILogger<TaminInsuranceJobListController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _TaminInsuranceJobListService = TaminInsuranceJobListService;
        _TaminInsuranceJobListService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return HR.SharedKernel.API.ApiControllerExtensions.AppOk(this, OperationResult.Succeeded(payload: _TaminInsuranceJobListService.GetAsKeyValuePair()));
    }
    [HttpGet, Route("GetAsKeyValuePairLazy2")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairLazy2()
    {
        return HR.SharedKernel.API.ApiControllerExtensions.AppOk(this, OperationResult.Succeeded(payload: _TaminInsuranceJobListService.GetAsKeyValuePair()));
    }
    [HttpGet, Route("GetAsKeyValuePairLazy/{filter}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairLazy(string filter)
    {
        return Ok(_TaminInsuranceJobListService.GetAsKeyValuePair(filter));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_TaminInsuranceJobListService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_TaminInsuranceJobListService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] TaminInsuranceJobListDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _TaminInsuranceJobListService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] TaminInsuranceJobListDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _TaminInsuranceJobListService.UpdateForAsync(body);
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
        return this.AppOk(OperationResult.Failed("امکان حذف جدول پایه سیستم وجود ندارد، لطفا با مدیر تماس بگیرید", payload: id));
    }
}
