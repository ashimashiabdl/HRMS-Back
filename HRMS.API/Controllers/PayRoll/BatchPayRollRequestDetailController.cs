using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Payroll.Core.Data;
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
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/BatchPayRollRequestDetail")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("جزئیات درخواست گروهی گروهی")]
public class BatchPayRollRequestDetailController : AppBaseController
{
    private readonly BatchPayRollRequestDetailService _BatchPayRollRequestDetailService;
    public BatchPayRollRequestDetailController(BatchPayRollRequestDetailService Service, ILogger<BatchPayRollRequestDetailController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _BatchPayRollRequestDetailService = Service;
        _BatchPayRollRequestDetailService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_BatchPayRollRequestDetailService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_BatchPayRollRequestDetailService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{BatchPayRollRequestId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? BatchPayRollRequestId = null)
    {
        if (BatchPayRollRequestId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        var filtered = _BatchPayRollRequestDetailService._db.Set<BatchPayRollRequestDetail>()
            .Include(x => x.Employee)
            .Where(i => i.IsDeleted != true && i.BatchPayRollRequestId == BatchPayRollRequestId);
        return this.AppOk(_BatchPayRollRequestDetailService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: filtered));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] BatchPayRollRequestDetailDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _BatchPayRollRequestDetailService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] BatchPayRollRequestDetailDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _BatchPayRollRequestDetailService.UpdateForAsync(body);
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
        return this.AppOk(_BatchPayRollRequestDetailService.DeleteRecord(id));
    }
}
