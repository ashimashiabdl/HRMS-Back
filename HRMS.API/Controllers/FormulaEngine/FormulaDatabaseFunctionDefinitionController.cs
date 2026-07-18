using HR.SharedKernel.Attribute;
using HR.SharedKernel.DTOs;

using Microsoft.AspNetCore.Mvc;
using HR.SharedKernel.API;

using AutoMapper;

using HR.SharedKernel.Dapper;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;

using HR.SharedKernel.Extensions;
using HR.FormulaEngine.Infrastructure.Data;
using LinqKit;

using HR.FormulaEngine.Infrastructure.Services;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.FormulaEngine;


[Route("api/FormulaDatabaseFunctionDefinition")]
[ControllerGroup("FormulaEngine", " فرمول ها ")]
[DisplayName("توابع دیتابیسی فرمول ها")]
public class FormulaDatabaseFunctionDefinitionController : AppBaseController
{
    private readonly FormulaDatabaseFunctionDefinitionService _formulaDatabaseFunctionDefinitionService;
    private FormulaEngineContext _context;
    public FormulaDatabaseFunctionDefinitionController(FormulaEngineContext context, FormulaDatabaseFunctionDefinitionService FormulaDatabaseFunctionDefinitionService, ILogger<FormulaDatabaseFunctionDefinitionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _formulaDatabaseFunctionDefinitionService = FormulaDatabaseFunctionDefinitionService;
        _formulaDatabaseFunctionDefinitionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_formulaDatabaseFunctionDefinitionService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_formulaDatabaseFunctionDefinitionService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        // رکوردهای عمومی (IsPublic) همیشه نمایش داده می‌شوند؛ در غیر این صورت فقط رکوردهای سازمان جاری
        var Filtered = _context.FormulaDatabaseFunctionDefinitions
            .Where(DateValidityExtension<FormulaDatabaseFunctionDefinition>.GetDateValidationPredicate(IgnoreExpired)
                .And(i => i.IsPublic == true || i.OrganisationChartId == currentUserDefaultOrganId));

        foreach (var item in Filtered)
        {
            item.Body = "";
            item.Help = "";
            item.ParamsJson = "";
        }

        return this.AppOk(_formulaDatabaseFunctionDefinitionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId: null, EmployeeId: null, CustomDataSource: Filtered, IgnoreDefaultOrganId: true));
    }
    private bool CheckForUnAuthorizedParts(FormulaDatabaseFunctionDefinitionDTO body)
    {
        if (string.IsNullOrEmpty(body.Body))
        {
            return false;
        }
        else
        {
            if (body.Body.Contains("update", StringComparison.CurrentCultureIgnoreCase) || body.Body.Contains("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] FormulaDatabaseFunctionDefinitionDTO body)
    {
        if (CheckForUnAuthorizedParts(body))
        {
            return this.AppBadRequest(OperationResult.Failed("بدنه تابع شامل عبارات غیر مجاز می باشد"));
        }

        if (ModelState.IsValid)
        {

            return Ok(await _formulaDatabaseFunctionDefinitionService.CreateForAsync(body));
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] FormulaDatabaseFunctionDefinitionDTO body)
    {
        if (CheckForUnAuthorizedParts(body))
        {
            return this.AppBadRequest(OperationResult.Failed("بدنه تابع شامل عبارات غیر مجاز می باشد"));
        }
        if (ModelState.IsValid)
        {
            var result = await _formulaDatabaseFunctionDefinitionService.UpdateForAsync(body);
            return this.AppOk(result);
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_formulaDatabaseFunctionDefinitionService.DeleteRecord(id));
    }
}
