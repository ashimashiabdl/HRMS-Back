using HR.SharedKernel.Attribute;


using AutoMapper;
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

namespace HRMS.API.Controllers.PayRoll;

[Route("api/PersonnelFicheItem")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("عوامل حقوقی اختصاصی فرد")]
public class PersonnelFicheItemController : AppBaseController
{
    private readonly PersonnelFicheItemService _PersonnelFicheItemService;
    public PersonnelFicheItemController(PersonnelFicheItemService Service, ILogger<PersonnelFicheItemController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _PersonnelFicheItemService = Service;
        _PersonnelFicheItemService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_PersonnelFicheItemService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_PersonnelFicheItemService.Get(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_PersonnelFicheItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] PersonnelFicheItemDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _PersonnelFicheItemService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] PersonnelFicheItemDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _PersonnelFicheItemService.UpdateForAsync(body);
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
        if (_PersonnelFicheItemService._unitOfWork.Context.PersonnelPayments.Any(i => i.PersonnelFicheItemId == id))
        {
            return this.AppBadRequest("رکورد مورد نظر ملاک محاسبه در فیش بوده است و امکان حذف وجود ندارد");
        }

        return this.AppOk(_PersonnelFicheItemService.DeleteRecord(id));
    }

    [HttpGet("GetPaymentDetailsByFicheItemId/{id}")]
    [CustomAccessKey(AccessKey: "GetPaymentDetailsByFicheItemId")]
    public IActionResult GetPaymentDetailsByFicheItemId(long id)
    {
        return this.AppOk(_PersonnelFicheItemService.GetPaymentDetailsByFicheItemId(id));
    }
}
