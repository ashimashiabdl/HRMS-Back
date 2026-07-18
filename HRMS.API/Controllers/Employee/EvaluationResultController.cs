using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.Employee.infrastructure.Services;

using HRMS.API.Controllers.SystemSetting;
using HR.BaseInfo.Core.DTOs;
using HR.Employee.Core.DTOs;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HRMS.API.Controllers.Employee;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/EvaluationResult")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("ارزشیابی")]
[EmployeeAccessCheck]
public class EvaluationResultController : AppBaseController
{
    private readonly EvaluationResultService _EvaluationResultService;
    private IConfiguration _configuration;
    private readonly EmployeeService _EmployeeService;

    public EvaluationResultController(IConfiguration configuration, EvaluationResultService Service, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _EvaluationResultService = Service;
        _configuration = configuration;
        _EmployeeService = EmployeeService;
        _EvaluationResultService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_EvaluationResultService.Get(id));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] EvaluationResultDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                body.OrganisationChartId = currentUserDefaultOrganId;
                return Ok(await _EvaluationResultService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] EvaluationResultDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                body.OrganisationChartId = currentUserDefaultOrganId;
                var result = await _EvaluationResultService.UpdateForAsync(body);
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
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (ModelState.IsValid)
        {
            try
            {

                if (EmployeeId > 0)
                {

                }
                else
                {
                    return this.AppNotFound();
                }
                return this.AppOk(_EvaluationResultService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: EmployeeId));

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
        return this.AppOk(_EvaluationResultService.DeleteRecord(id));
    }
}
