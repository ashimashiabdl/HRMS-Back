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

[Route("api/BankDisketteTemplate")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("قالب دیسکت بانک")]
public class BankDisketteTemplateController : AppBaseController
{
    private readonly BankDisketteTemplateService _BankDisketteTemplateService;
    public BankDisketteTemplateController(BankDisketteTemplateService Service, ILogger<BankDisketteTemplateController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _BankDisketteTemplateService = Service;
        _BankDisketteTemplateService._currentUserDefaultOrganId = currentUserDefaultOrganId;

    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_BankDisketteTemplateService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_BankDisketteTemplateService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_BankDisketteTemplateService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    private BankDisketteTemplateDTO validatePostedData(BankDisketteTemplateDTO body)
    {
        if (body.HasLineDelimiterCharacter != true)
        {
            body.LineDelimiterCharacter = null;
        }
        if (body.HasLineEndCharacter != true)
        {
            body.LineEndCharacter = null;
        }
        if (body.HasLineStartCharacter != true)
        {
            body.LineStartCharacter = null;
        }
        return body;
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] BankDisketteTemplateDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _BankDisketteTemplateService.CreateForAsync(validatePostedData(body)));
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

    public async Task<IActionResult> Put([FromBody] BankDisketteTemplateDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _BankDisketteTemplateService.UpdateForAsync(validatePostedData(body));
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
        return this.AppOk(_BankDisketteTemplateService.DeleteRecord(id));
    }
}
