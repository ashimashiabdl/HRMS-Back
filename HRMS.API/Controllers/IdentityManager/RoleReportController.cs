using HR.SharedKernel.Attribute;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/RoleReport")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("دسترسی گزارش های نقش")]
public class RoleReportController : AppBaseController
{
    private readonly RoleReportService _RoleReportService;
    public RoleReportController(RoleReportService RoleReportService, ILogger<RoleReportController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _RoleReportService = RoleReportService;
        _RoleReportService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetCurrentRoleAvailableReports")]
    [CustomAccessKey(AccessKey: "GetCurrentRoleAvailableReports")]
    public IActionResult GetCurrentRoleAvailableReports()
    {
        return this.AppOk(_RoleReportService.GetSelectedRoleAvailableReports(currentUserId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_RoleReportService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? RoleId = null, [FromQuery] long? DynamicReportId = null)
    {
        if (RoleId == 0)
        {
            RoleId = null;
        }
        if (DynamicReportId == 0)
        {
            DynamicReportId = null;
        }


        var Filtered = ((IdentityContext)_RoleReportService._db).RoleReports
           .Include(i => i.Role)

           .Where(DateValidityExtension<RoleReport>.GetDateValidationPredicate(IgnoreExpired).And(i => (i.RoleId == RoleId || RoleId == null) && (i.DynamicReportId == DynamicReportId || DynamicReportId == null)))
           ;

        var paged = _RoleReportService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered);

        var results = _RoleReportService._unitOfWork.Context.Database.SqlQuery<HR.SharedKernel.Data.KeyValuePair>($"SELECT \r\n[Id] as [key],\r\n[Id] as [id]\r\n     \r\n      ,[title] as [value]\r\n    \r\n  FROM [rpt].[Dynamic_Report]\r\n");
        var list = paged.Payload;
        foreach (var row in (List<RoleReportDTO>)list)
        {
            if (results.Any(i => i.id == row.DynamicReportId))
            {
                row.DynamicReport = results.Single(i => i.id == row.DynamicReportId).value;
            }
        }


        return this.AppOk(paged);
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] RoleReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _RoleReportService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] RoleReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _RoleReportService.UpdateForAsync(body);
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
        var result = _RoleReportService.DeleteRecord(id);
        return this.AppOk(result);
    }
}

