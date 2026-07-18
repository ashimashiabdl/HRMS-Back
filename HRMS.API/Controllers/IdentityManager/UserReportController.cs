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

[Route("api/UserReport")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("دسترسی گزارش ها")]
public class UserReportController : AppBaseController
{
    private readonly UserReportService _UserReportService;
    public UserReportController(UserReportService UserReportService, ILogger<UserReportController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, null, dapper)
    {
        _UserReportService = UserReportService;
        _UserReportService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetCurrentUserAvailableReports")]
    [CustomAccessKey(AccessKey: "GetCurrentUserAvailableReports")]
    public IActionResult GetCurrentUserAvailableReports()
    {
        return this.AppOk(_UserReportService.GetSelectedUserAvailableReports(currentUserId));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_UserReportService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null, [FromQuery] long? DynamicReportId = null)
    {
        if (UserId == 0)
        {
            UserId = null;
        }
        if (DynamicReportId == 0)
        {
            DynamicReportId = null;
        }


        var Filtered = ((IdentityContext)_UserReportService._db).UserReports
           .Include(i => i.User)

           .Where(DateValidityExtension<UserReport>.GetDateValidationPredicate(IgnoreExpired).And(i => (i.UserId == UserId || UserId == null) && (i.DynamicReportId == DynamicReportId || DynamicReportId == null)))
           ;

        var paged = _UserReportService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered);

        var results = _UserReportService._unitOfWork.Context.Database.SqlQuery<HR.SharedKernel.Data.KeyValuePair>($"SELECT \r\n[Id] as [key],\r\n[Id] as [id]\r\n     \r\n      ,[title] as [value]\r\n    \r\n  FROM [rpt].[Dynamic_Report]\r\n");
        var list = paged.Payload;
        foreach (var row in (List<UserReportDTO>)list)
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

    public async Task<IActionResult> Post([FromBody] UserReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _UserReportService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] UserReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _UserReportService.UpdateForAsync(body);
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
        var result = _UserReportService.DeleteRecord(id);
        return this.AppOk(result);
    }
}



