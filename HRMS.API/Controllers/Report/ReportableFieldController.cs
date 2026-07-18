using AutoMapper;
using HR.Report.Core.DTOs;
using HR.Report.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace HRMS.API.Controllers.Report;

[Route("api/ReportableField")]
[ControllerGroup("Report", "گزارش گیری")]
[DisplayName("فیلدهای قابل گزارش")]
public class ReportableFieldController : AppBaseController
{
    private readonly ReportableFieldService _service;

    public ReportableFieldController(ReportableFieldService service, ILogger<ReportableFieldController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_service.GetAsKeyValuePair());
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? entityId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, entityId));
    }

    [HttpGet, Route("GetAllWithPaging/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAllWithPaging(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? entityId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, entityId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] ReportableFieldDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                return Ok(await _service.CreateForAsync(body));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }

        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in allErrors)
        {
            _logger.LogInformation(error.ErrorMessage);
        }

        return this.AppBadRequest(ModelState);
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] ReportableFieldDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _service.UpdateForAsync(body);
                return this.AppOk(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.AppBadRequest("Internal Server Error");
            }
        }

        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in allErrors)
        {
            _logger.LogInformation(error.ErrorMessage);
        }

        return this.AppBadRequest(ModelState);
    }

    /// <summary>
    /// حذف فیزیکی فیلد قابل گزارش
    /// </summary>
    /// <param name="id">شناسه فیلد</param>
    /// <returns>نتیجه عملیات حذف</returns>
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        // حذف فیزیکی (Physical Delete) - رکورد به طور کامل از دیتابیس حذف می‌شود
        return this.AppOk(_service.DeleteRecord(id));
    }
}

