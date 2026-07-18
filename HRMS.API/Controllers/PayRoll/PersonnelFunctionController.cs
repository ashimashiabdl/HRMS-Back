using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;


namespace HRMS.API.Controllers.PayRoll;

[Route("api/PersonnelFunction")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("کارکرد کارکنان")]
public class PersonnelFunctionController : AppBaseController
{
    private readonly PersonnelFunctionService _personnelFunctionService;
    private readonly TempPersonnelFunctionservice _tempPersonnelFunctionservice;
    public PersonnelFunctionController(PersonnelFunctionService Service, TempPersonnelFunctionservice TempPersonnelFunctionservice, ILogger<PersonnelFunctionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _tempPersonnelFunctionservice = TempPersonnelFunctionservice;
        _personnelFunctionService = Service;
        _personnelFunctionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _tempPersonnelFunctionservice._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _personnelFunctionService._currentUserDefaultPaymentPeriod = currentUserDefaultPaymentPeiodId;
    }
    [HttpGet, Route("GetTop5Year")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetTop5Year()
    {
        return this.AppOk(_personnelFunctionService.GetTop5Year());
    }
    [HttpGet, Route("GetYearList")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetYearList()
    {
        return this.AppOk(_personnelFunctionService.GetYearList());
    }
    [HttpGet, Route("CopyFunctionsFromLastPeriod")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult CopyFunctionsFromLastPeriod()
    {
        return this.AppOk(_personnelFunctionService.CopyFunctionsFromLastPeriod());
    }
    [HttpGet, Route("GetMonths")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetMonths()
    {
        return this.AppOk(_personnelFunctionService.GetMonths());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_personnelFunctionService.Get(id));
    }

    [HttpGet, Route("TransferExcelTempFunctionstoMain/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult TransferExcelTempFunctionstoMain(int id)
    {
        var affected = _tempPersonnelFunctionservice.TransferExcelTempFunctions_toMain(id);
        if (affected > 0)
        {
            return this.AppOk(OperationResult.Succeeded(payload: affected));
        }
        else
        {
            return this.AppBadRequest(OperationResult.Failed("ردیفی منتقل نشد"));
        }
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (EmployeeId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        return this.AppOk(_personnelFunctionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: EmployeeId));
    }        /// <summary>
             /// گرفتن فهرست کارکرد های ثبت شده موقت جهت پیش نمایش  تایید کارکرد ها
             /// </summary>
             /// <param name="currentPage"></param>
             /// <param name="pageSize"></param>
             /// <param name="filter"></param>
             /// <param name="activeSortColumn"></param>
             /// <param name="Sortdirection"></param>
             /// <param name="PersonnelFunctionExcelFileId"></param>
             /// <returns></returns>
    [HttpGet, Route("GetPagedTempData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{PersonnelFunctionExcelFileId?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedTempData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] long? PersonnelFunctionExcelFileId = null)
    {
        if (PersonnelFunctionExcelFileId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        var filterede = _tempPersonnelFunctionservice.All().Include(i => i.Employee).Where(i => i.PersonnelFunctionExcelFileId == PersonnelFunctionExcelFileId && i.OrganisationChartId == currentUserDefaultOrganId);
        var paged = _tempPersonnelFunctionservice.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: filterede);
        return this.AppOk(paged);
    }

    [HttpPost("getCurrentOrganPayRollPendigFunctions")]
    [CustomAccessKey(AccessKey: "create")]
    public IActionResult getCurrentOrganPayRollPendigFunctions([FromBody] PayRollOrderPagerDTO id)
    {
        id.currentUserId = currentUserId;
        return this.AppOk(_personnelFunctionService.getCurrentOrganPayRollPendigFunctions(id));
    }

    [HttpPost("GetFilteredPersonnelFunctions")]
    [CustomAccessKey(AccessKey: "GetFilteredPersonnelFunctions")]
    public IActionResult GetFilteredPersonnelFunctions([FromBody] PersonnelFunctionFilterDTO filterDto)
    {
        return this.AppOk(_personnelFunctionService.GetFilteredPersonnelFunctions(filterDto));
    }

    [HttpPut("BulkUpdate")]
    [CustomAccessKey(AccessKey: "BulkUpdate")]
    public async Task<IActionResult> BulkUpdate([FromBody] List<PersonnelFunctionDTO> items)
    {
        if (ModelState.IsValid)
        {
            if (items == null || !items.Any())
            {
                return this.AppBadRequest(OperationResult.Failed("لیست کارکردها خالی است"));
            }
            return this.AppOk(await _personnelFunctionService.BulkUpdateAsync(items));
        }
        return this.AppBadRequest(ModelState);
    }

    [HttpPost("GetFunctionBulkCartable")]
    [CustomAccessKey(AccessKey: "GetFunctionBulkCartable")]
    public IActionResult GetFunctionBulkCartable([FromBody] FunctionBulkCartableFilterDTO filterDto)
    {
        return this.AppOk(_personnelFunctionService.GetFunctionBulkCartable(filterDto));
    }
    [HttpPut("PayRollApproveAll")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult PayRollApproveAll([FromBody] FunctionApproveDTO req)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var all = _personnelFunctionService.All(IgnoreExpired: false).Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.IsConfirmed != true).ToList();
                foreach (var item in all)
                {
                    try
                    {
                        req.Id = item.Id;
                        _personnelFunctionService.PayRollApprove(req);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
                return this.AppOk(OperationResult.Succeeded());
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


    [HttpPut("PayRollApprove")]
    [CustomAccessKey(AccessKey: "update")]

    public IActionResult PayRollApprove([FromBody] FunctionApproveDTO req)
    {
        if (ModelState.IsValid)
        {
            if (req.Id > 0)
            {
                return this.AppOk(_personnelFunctionService.PayRollApprove(req));
            }
            else
            {
                if (req.FunctionIdList == null)
                {
                    return this.AppBadRequest(OperationResult.Failed("شناسه کارکرد ارسال نشده است"));
                }
                else
                {
                    if (req.FunctionIdList.Any())
                    {
                        foreach (var item in req.FunctionIdList)
                        {
                            try
                            {
                                req.Id = item;
                                _personnelFunctionService.PayRollApprove(req);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message, ex.Message);
                            }
                        }
                        return this.AppOk(OperationResult.Succeeded());
                    }
                    else
                    {
                        return this.AppBadRequest(OperationResult.Failed("شناسه حکم ارسال نشده است"));
                    }
                }
            }

        }
        else
        {
            return this.AppBadRequest(ModelState);
        }
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] PersonnelFunctionDTO body)
    {
        if (ModelState.IsValid)
        {
            body.title = "z";
            return Ok(await _personnelFunctionService.CreateForAsync(body));
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] PersonnelFunctionDTO body)
    {
        var result = await _personnelFunctionService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public async Task<IActionResult> Delete(int id)
    {
        return this.AppOk(await Task.Run(() => _personnelFunctionService.DeleteRecord(id)));
    }
    [HttpDelete("DeleteTemp/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> DeleteTemp(int id)
    {
        return this.AppOk(await Task.Run(() => _tempPersonnelFunctionservice.DeleteRecord(id)));
    }
}
