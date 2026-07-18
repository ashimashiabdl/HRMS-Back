using HR.SharedKernel.Attribute;
using Microsoft.AspNetCore.Mvc;
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
using HR.SharedKernel.DTOs;

namespace HRMS.API.Controllers.FormulaEngine;

[Route("api/FormulaDefinition")]
[ControllerGroup("FormulaEngine", " فرمول ها ")]
[DisplayName("طراحی فرمول ها")]
public class FormulaDefinitionController : AppBaseController
{
    private readonly FormulaService _formulaDefinitionService;
    private FormulaEngineContext _context;
    public FormulaDefinitionController(FormulaEngineContext context, FormulaService FormulaDefinitionService, ILogger<FormulaDefinitionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _formulaDefinitionService = FormulaDefinitionService;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_formulaDefinitionService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_formulaDefinitionService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _context.FormulaDefinitions
        .Include(i => i.OrganisationFormula.Formula)
        .Where(DateValidityExtension<FormulaDefinition>.GetDateValidationPredicate(IgnoreExpired).And(i => i.OrganisationFormula.OrganisationChartId == currentUserDefaultOrganId));
        return this.AppOk(_formulaDefinitionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered));
    }

    [HttpGet, Route("GetFormulaCountsByOrganisation")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetFormulaCountsByOrganisation()
    {
        return this.AppOk(_formulaDefinitionService.GetFormulaCountsByOrganisation());
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] FormulaDefinitionDTO body)
    {
        return Ok(await _formulaDefinitionService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] FormulaDefinitionDTO body)
    {
        if (_formulaDefinitionService.All().Any(i => i.Id == body.Id))
        {
            string? previousFormulaText = null;
            try
            {
                var currentEntity = _context.FormulaDefinitions.AsNoTracking().SingleOrDefault(i => i.Id == body.Id);
                previousFormulaText = currentEntity?.FormulaText;
            }
            catch { }
            if (body.Version == null)
            {
                body.Version = 0;
            }
            body.Version = body.Version + 1;
            var result = await _formulaDefinitionService.UpdateForAsync(body);
            try
            {
                await _formulaDefinitionService.LogFormulaHistoryAsync(Convert.ToInt64(body.Id), previousFormulaText);
            }
            catch { }
            return this.AppOk(result);
        }
        else
        {
            body.Version = 1;
            return Ok(await _formulaDefinitionService.CreateForAsync(body));
        }
    }
    [HttpGet, Route("GetHistory/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetHistory(long id)
    {
        var data = _formulaDefinitionService
            .GetHistory(id)
            .Select(h => new
            {
                previousFormulaText = h.PreviousFormulaText,
                changeDateTime = h.ChangeDateTime,
                ipAddress = h.IPAddress,
                userId = h.UserId,
                userFullName = h.UserFullName
            })
            .ToList();
        return this.AppOk(OperationResult.Succeeded(payload: data));
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        return this.AppOk(_formulaDefinitionService.DeleteRecord(id));
    }
}
