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

[Route("api/BankBranch")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("شعبه بانک")]
public class BankBranchController : AppBaseController
{
    private readonly BankBranchService _BankBranchService;
    public BankBranchController(BankBranchService Service, ILogger<BankBranchController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _BankBranchService = Service;
        _BankBranchService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair(long id)
    {
        return this.AppOk(_BankBranchService.GetAsKeyValuePair(id));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_BankBranchService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_BankBranchService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] BankBranchDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _BankBranchService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] BankBranchDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _BankBranchService.UpdateForAsync(body);
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
        return this.AppOk(_BankBranchService.DeleteRecord(id));
    }
}
