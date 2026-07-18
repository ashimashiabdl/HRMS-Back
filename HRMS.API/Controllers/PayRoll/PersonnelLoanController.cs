using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;


[Route("api/PersonnelLoan")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("وام کارکنان")]
public class PersonnelLoanController : AppBaseController
{
    private readonly PersonnelLoanService _PersonnelLoanService;
    private readonly FicheItemService _ficheItemService;
    public PersonnelLoanController(PersonnelLoanService Service, FicheItemService FicheItemService, ILogger<PersonnelLoanController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _ficheItemService = FicheItemService;
        _PersonnelLoanService = Service;
        _PersonnelLoanService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_PersonnelLoanService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_PersonnelLoanService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        return this.AppOk(_PersonnelLoanService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId: null, EmployeeId: EmployeeId));
    }
    [HttpGet, Route("getcartexWithServersidePaging/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult getcartexWithServersidePaging(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] long EmployeeId = 0, [FromQuery] long PersonnelLoanId = 0)
    {
        if (PersonnelLoanId > 0 && EmployeeId > 0)
        {
            var filtered = _ficheItemService.All()
                .Include(i => i.WageItem)
                .Include(i => i.Fiche)
                .Include(i => i.Fiche.PaymentPeriod)
                .Where(i => i.PersonnelLoanId == PersonnelLoanId)
                .OrderByDescending(i => i.Fiche.PaymentPeriod.ShamsiYear)
                .ThenByDescending(i => i.Fiche.PaymentPeriod.ShamsiMonth)
                ;
            return this.AppOk(_ficheItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filtered));
        }
        return this.AppBadRequest("Internal Server Error");
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] PersonnelLoanDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _PersonnelLoanService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] PersonnelLoanDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {




                var result = await _PersonnelLoanService.UpdateForAsync(body);
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
        return this.AppOk(_PersonnelLoanService.DeleteRecord(id));
    }
}
