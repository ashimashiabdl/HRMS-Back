using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/CommonPassword")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("کلمه عبورهای متداول")]
public class CommonPasswordController : AppBaseController
{
    private readonly CommonPasswordService _commonPasswordService;
    
    public CommonPasswordController(CommonPasswordService commonPasswordService, ILogger<CommonPasswordController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService userResolverService, ILogger<CommonPasswordService> serviceLogger) 
        : base(userResolverService, logger, accessor, null, dapper)
    {
        _commonPasswordService = commonPasswordService;
        _commonPasswordService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_commonPasswordService.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_commonPasswordService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: null));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] CommonPasswordDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                return Ok(await _commonPasswordService.CreateForAsync(body));
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
    public async Task<IActionResult> Put([FromBody] CommonPasswordDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _commonPasswordService.UpdateForAsync(body);
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
        var result = _commonPasswordService.DeleteRecord(id);
        return this.AppOk(result);
    }
}

