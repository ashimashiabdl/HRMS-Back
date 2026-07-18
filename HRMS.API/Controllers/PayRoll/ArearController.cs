using AutoMapper;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/Arear")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("معوقه")]
public class ArearController : AppBaseController
{
    private readonly ArearService _arearService;

    public ArearController(
        ArearService arearService,
        ILogger<ArearController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _arearService = arearService;
        _arearService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        return this.AppOk(_arearService.GetArearById(id));
    }

    [HttpGet, Route("GetByEmployee/{currentPage}/{pageSize}/{employeeId}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetByEmployee(int currentPage, int pageSize, long employeeId)
    {
        return this.AppOk(_arearService.GetArearsByEmployee(employeeId, currentPage, pageSize));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] bool IgnoreExpired = true)
    {
        var filtered = _arearService._unitOfWork.Context.Arears
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.IsDeleted != true);
        if (string.IsNullOrEmpty(activeSortColumn))
        {
            activeSortColumn = "id";
        }
        return this.AppOk(_arearService.GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            CustomDataSource: filtered));
    }
}
