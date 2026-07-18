using AutoMapper;
using HR.Identity.infrastructure.Services;
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

[Route("api/PayLocationProgressReport")]
[ControllerGroup("Report", "گزارش گیری")]
[DisplayName("گزارش پیشرفت محل پرداخت")]
public class PayLocationProgressReportController : AppBaseController
{
    private readonly PayLocationProgressReportService _service;
    private readonly UserPayLocationService _userPayLocationService;

    public PayLocationProgressReportController(
        PayLocationProgressReportService service,
        UserPayLocationService userPayLocationService,
        ILogger<PayLocationProgressReportController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _userPayLocationService = userPayLocationService;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _userPayLocationService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_userPayLocationService.GetAsKeyValuePair(currentUserId));
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_service.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? OrganisationChartId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, OrganisationChartId: OrganisationChartId));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] PayLocationProgressReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Set UploadedByUserId to current user
                body.UploadedByUserId = currentUserId;
                
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
    public async Task<IActionResult> Put([FromBody] PayLocationProgressReportDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Set UploadedByUserId to current user if not set
                if (body.UploadedByUserId == null)
                {
                    body.UploadedByUserId = currentUserId;
                }
                
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

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}

