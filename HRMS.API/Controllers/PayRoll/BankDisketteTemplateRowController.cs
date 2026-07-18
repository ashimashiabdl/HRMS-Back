using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Organisation.Infrastructure.Services;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/BankDisketteTemplateRow")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("تنظیمات ردیف الگو دیسکت بانک")]
public class BankDisketteTemplateRowController : AppBaseController
{
    private readonly BankDisketteTemplateRowService _BankDisketteTemplateRowService;
    public BankDisketteTemplateRowController(BankDisketteTemplateRowService Service, ILogger<BankDisketteTemplateRowController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base( UserResolverService, logger, accessor, mapper, dapper)
    {
        _BankDisketteTemplateRowService = Service;
        _BankDisketteTemplateRowService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
[CustomAccessKey(AccessKey: "view")]
    
    public IActionResult GetAsKeyValuePair()
{                
            return this.AppOk(_BankDisketteTemplateRowService.GetAsKeyValuePair());
}        [HttpGet, Route("Get/{id}")]
[CustomAccessKey(AccessKey: "view")]
    
    public IActionResult Get(int id)
{                
            return this.AppOk(_BankDisketteTemplateRowService.Get(id));
}
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
[CustomAccessKey(AccessKey: "view")]
    
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? BankDisketteTemplateId = null)
{                if (BankDisketteTemplateId == 0)
            {
                BankDisketteTemplateId = null;
            }
            
            var Filtered = _BankDisketteTemplateRowService._db.Set<HR.Payroll.Core.Data.BankDisketteTemplateRow>()
           .Include(i => i.BankDisketteTemplate.Bank)
           .Where(DateValidityExtension<HR.Payroll.Core.Data.BankDisketteTemplateRow>.GetDateValidationPredicate(IgnoreExpired)
           //.And(i => i.OrganizationJob.OrganisationChartId == currentUserDefaultOrganId)
           .And(i => BankDisketteTemplateId == null || BankDisketteTemplateId == i.BankDisketteTemplateId)
           );
            return this.AppOk(_BankDisketteTemplateRowService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered));
}
    [HttpPost("Post")]
[CustomAccessKey(AccessKey: "create")]
    
    public async Task<IActionResult> Post([FromBody] BankDisketteTemplateRowDTO body)
{            if (ModelState.IsValid)
        {
            try
            {
                
                return Ok(await _BankDisketteTemplateRowService.CreateForAsync(body));
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
}        [HttpPut("Put")]
[CustomAccessKey(AccessKey: "update")]
    
    public async Task<IActionResult> Put([FromBody] BankDisketteTemplateRowDTO body)
{            if (ModelState.IsValid)
        {
            try
            {
                
                var result = await _BankDisketteTemplateRowService.UpdateForAsync(body);
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
            return this.AppOk(_BankDisketteTemplateRowService.DeleteRecord(id));
}    }
