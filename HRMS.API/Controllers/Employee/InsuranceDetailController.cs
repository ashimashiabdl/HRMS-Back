using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.Employee.infrastructure.Data;
using Hr.Employee.infrastructure.Services;

using HRMS.API.Controllers.SystemSetting;
using HR.BaseInfo.Core.DTOs;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using HRMS.API.Controllers.Employee;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/InsuranceDetail")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("جزئیات بیمه")]
[EmployeeAccessCheck]
public class InsuranceDetailController : AppBaseController
{
    private readonly InsuranceDetailService _InsuranceDetailService;
    private IConfiguration _configuration;
    private EmployeeContext _EmployeeContext;
    private readonly EmployeeService _EmployeeService;

    public InsuranceDetailController(EmployeeContext ctx, IConfiguration configuration, InsuranceDetailService Service, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _EmployeeContext = ctx;
        _InsuranceDetailService = Service;
        _configuration = configuration;
        _EmployeeService = EmployeeService;
        _InsuranceDetailService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_InsuranceDetailService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_InsuranceDetailService.Get(id));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] InsuranceDetailDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (body.InsuranceId > 0)
                {
                    var parent = _EmployeeContext.Insurances.FirstOrDefault(i => i.Id == body.InsuranceId);
                    if (parent != null && parent.EmployeeId > 0 && !_EmployeeService.CheckAccess(currentUserId, parent.EmployeeId))
                    {
                        return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
                    }
                }

                return Ok(await _InsuranceDetailService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] InsuranceDetailDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (body.InsuranceId > 0)
                {
                    var parent = _EmployeeContext.Insurances.FirstOrDefault(i => i.Id == body.InsuranceId);
                    if (parent != null && parent.EmployeeId > 0 && !_EmployeeService.CheckAccess(currentUserId, parent.EmployeeId))
                    {
                        return this.AppBadRequest("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
                    }
                }

                var result = await _InsuranceDetailService.UpdateForAsync(body);
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
        if (EmployeeId > 0)
        {
            var Filtered = _EmployeeContext.InsuranceDetails.Include(i => i.Insurance).Where(i => i.Insurance.EmployeeId == EmployeeId);
            return this.AppOk(_InsuranceDetailService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
        }
        else
        {
            return this.AppNotFound();
        }
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_InsuranceDetailService.DeleteRecord(id));
    }
}
