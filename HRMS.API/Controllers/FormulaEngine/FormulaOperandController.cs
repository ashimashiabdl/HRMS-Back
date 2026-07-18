using HR.SharedKernel.Attribute;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.Core.Interfaces;
using HR.SharedKernel;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HR.SharedKernel.API;

using AutoMapper;

using HR.SharedKernel.Dapper;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Extensions;
using HR.FormulaEngine.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.FormulaEngine.Infrastructure.Services;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.FormulaEngine;

[Route("api/FormulaOperand")]
[ControllerGroup("FormulaEngine", " فرمول ها ")]
[DisplayName("عملوند های فرمول")]
public class FormulaOperandController : AppBaseController
{
    private readonly FormulaOperandService _formulaOperandService;

    public FormulaOperandController(FormulaEngineContext context, FormulaOperandService FormulaOperandService, ILogger<FormulaOperandController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _formulaOperandService = FormulaOperandService;
        _formulaOperandService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_formulaOperandService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_formulaOperandService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_formulaOperandService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] FormulaOperandDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _formulaOperandService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] FormulaOperandDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _formulaOperandService.UpdateForAsync(body);
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
        return this.AppOk(_formulaOperandService.DeleteRecord(id));
    }
}
