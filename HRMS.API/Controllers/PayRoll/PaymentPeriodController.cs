using HR.SharedKernel.Attribute;
using HR.SharedKernel.Service;

using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;


[Route("api/PaymentPeriod")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("دوره حقوقی")]
public class PaymentPeriodController : AppBaseController
{
    private readonly PaymentPeriodService _PaymentPeriodService;
    public PaymentPeriodController(PaymentPeriodService Service, ILogger<PaymentPeriodController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _PaymentPeriodService = Service;
        _PaymentPeriodService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetcurrentUserDefaultPaymentPeiodId")]
    [CustomAccessKey(AccessKey: "GetcurrentUserDefaultPaymentPeiodId")]

    public IActionResult GetcurrentUserDefaultPaymentPeiodId()
    {
        var period = _PaymentPeriodService.GetIdAsync(currentUserDefaultPaymentPeiodId).Result;
        return this.AppOk(OperationResult.Succeeded(payload: period));
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_PaymentPeriodService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_PaymentPeriodService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_PaymentPeriodService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] PaymentPeriodDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _PaymentPeriodService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] PaymentPeriodDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _PaymentPeriodService.UpdateForAsync(body);
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
        return this.AppOk(_PaymentPeriodService.DeleteRecord(id));
    }

    [HttpPost("ClosePeriodAndMarkFichesPayed/{id}")]
    [CustomAccessKey(AccessKey: "ClosePeriodAndMarkFichesPayed")]
    public IActionResult ClosePeriodAndMarkFichesPayed(long id)
    {
        return Ok(_PaymentPeriodService.ClosePeriodAndMarkFichesPayed(id));
    }
}
