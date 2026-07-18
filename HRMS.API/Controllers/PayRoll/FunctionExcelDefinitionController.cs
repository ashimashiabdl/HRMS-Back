using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Order.Core.Data;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using HR.SharedKernel.DTOs;
using HRMS.API.Infrastructure.Upload;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/FunctionExcelDefinition")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("تعریف اکسل کارکرد")]
public class FunctionExcelDefinitionController : AppBaseController
{
    private readonly FunctionExcelDefinitionService _FunctionExcelDefinitionService;
    private readonly UserResolverService _userResolverService;
    public FunctionExcelDefinitionController(FunctionExcelDefinitionService Service, ILogger<FunctionExcelDefinitionController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _userResolverService = UserResolverService;
        _FunctionExcelDefinitionService = Service;
        _FunctionExcelDefinitionService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpPost("uploadTempFile")]
    [CustomAccessKey(AccessKey: "create")]
    [DisableRequestSizeLimit]
    [AllowUploadExtensions(".xlsx")]
    public async Task<IActionResult> uploadTempFile()
    {
        var formCollection = await Request.ReadFormAsync();
        var file = formCollection.Files.FirstOrDefault();
        if (file == null || file.Length <= 0)
        {
            _logger.LogWarning("Function excel upload rejected: no file in request. UserId={UserId}", _userResolverService.GetUserId());
            return this.AppBadRequest(OperationResult.Failed("فایل یافت نشد یا خالی است"));
        }

        if (!formCollection.ContainsKey("paymentPeriodId") || string.IsNullOrWhiteSpace(formCollection["paymentPeriodId"]))
        {
            return this.AppBadRequest(OperationResult.Failed("دوره پرداخت انتخاب نشده است"));
        }

        if (!formCollection.ContainsKey("employeeTypeId") || string.IsNullOrWhiteSpace(formCollection["employeeTypeId"]))
        {
            return this.AppBadRequest(OperationResult.Failed("نوع استخدام انتخاب نشده است"));
        }

        if (!long.TryParse(formCollection["paymentPeriodId"], out long paymentPeriodId) || paymentPeriodId <= 0)
        {
            return this.AppBadRequest(OperationResult.Failed("دوره پرداخت نامعتبر است"));
        }

        if (!long.TryParse(formCollection["employeeTypeId"], out long employeeTypeId) || employeeTypeId <= 0)
        {
            return this.AppBadRequest(OperationResult.Failed("نوع استخدام نامعتبر است"));
        }

        await using var stream = file.OpenReadStream();
        var result = await _FunctionExcelDefinitionService.UploadTempExcelFileAsync(
            stream,
            file.FileName,
            file.Length,
            paymentPeriodId,
            employeeTypeId,
            _userResolverService.GetUserId(),
            _userResolverService.GetIP());

        if (!result.Success)
        {
            return this.AppBadRequest(result);
        }

        return Ok(result.Payload);
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_FunctionExcelDefinitionService.GetAsKeyValuePair());
    }


    [HttpGet, Route("FunctionExcelStartingRow")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult FunctionExcelStartingRow()
    {
        long FunctionExcelStartingRow = 0;
        var ExpiryDurationSetting = _FunctionExcelDefinitionService.GetSettingById(10018);
        if (string.IsNullOrEmpty(ExpiryDurationSetting) || ExpiryDurationSetting == "-1")
        {
            FunctionExcelStartingRow = HR.SharedKernel.Share.Constants.FunctionExcelStartingRow;
        }
        else
        {
            FunctionExcelStartingRow = Convert.ToInt64(ExpiryDurationSetting);
        }
        return this.AppOk(FunctionExcelStartingRow);
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_FunctionExcelDefinitionService.Get(id));
    }
    [HttpGet, Route("GetExcelPreview/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetExcelPreview(int id)
    {
        try
        {
            return this.AppOk(_FunctionExcelDefinitionService.GetExcelPreview(id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetExcelPreview for id: {Id}. Message: {Message}", id, ex.Message);
            return this.AppBadRequest("Internal Server Error");
        }
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? ExcelDefinitionTypeId = null)
    {
        IQueryable<HR.Payroll.Core.Data.FunctionExcelDefinition>? custom = null;
        if (ExcelDefinitionTypeId.HasValue && ExcelDefinitionTypeId.Value > 0)
        {
            custom = _FunctionExcelDefinitionService.All(IgnoreExpired)
                .Where(i => i.ExcelDefinitionTypeId == ExcelDefinitionTypeId.Value);
        }
        return this.AppOk(_FunctionExcelDefinitionService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: custom));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] FunctionExcelDefinitionDTO body)
    {
        // Custom validation for conditional required fields
        if (!body.IsLeave && (!body.PersonnelFunctionColumnId.HasValue || body.PersonnelFunctionColumnId.Value <= 0))
        {
            return this.AppBadRequest(OperationResult.Failed("ستون متناظر جدول کارکرد الزامی است"));
        }
        
        if (body.IsLeave && (!body.LeaveTypeId.HasValue || body.LeaveTypeId.Value <= 0))
        {
            return this.AppBadRequest(OperationResult.Failed("نوع مرخصی الزامی است"));
        }
        
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _FunctionExcelDefinitionService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] FunctionExcelDefinitionDTO body)
    {
        // Custom validation for conditional required fields
        if (!body.IsLeave && (!body.PersonnelFunctionColumnId.HasValue || body.PersonnelFunctionColumnId.Value <= 0))
        {
            return this.AppBadRequest(OperationResult.Failed("ستون متناظر جدول کارکرد الزامی است"));
        }
        
        if (body.IsLeave && (!body.LeaveTypeId.HasValue || body.LeaveTypeId.Value <= 0))
        {
            return this.AppBadRequest(OperationResult.Failed("نوع مرخصی الزامی است"));
        }
        
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _FunctionExcelDefinitionService.UpdateForAsync(body);
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
        return this.AppOk(_FunctionExcelDefinitionService.PhysicalDelete(id));
    }
}
