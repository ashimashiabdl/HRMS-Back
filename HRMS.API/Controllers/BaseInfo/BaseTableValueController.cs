using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.Core.Interfaces;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.Data.SqlClient;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/BaseTableValue")]
[DisplayName("مقادیر اطلاعات پایه")]
[ControllerGroup("baseInfo", "اطلاعات پایه")]
public class BaseTableValueController : AppBaseController
{
    private readonly BaseTableValueService _baseTableValueService;
    public BaseTableValueController(BaseTableValueService BaseTableValueService, ILogger<BaseTableValueController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _baseTableValueService = BaseTableValueService;
        _baseTableValueService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("GetAsKeyValuePairValue/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePairValue(int id)
    {
        return this.AppOk(_baseTableValueService.GetAsKeyValuePairValue(id));
    }
    [HttpGet, Route("GetAsKeyValuePairValueIncluded/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePairValueIncluded(int id)
    {
        return this.AppOk(_baseTableValueService.GetAsKeyValuePairValueIncluded(id));
    }
    [HttpPost("GetAsKeyValuePairValueBatch")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePairValueBatch([FromBody] BatchTableValueRequest body)
    {
        return Ok(_baseTableValueService.GetAsKeyValuePairValueBatch(body));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_baseTableValueService.Get(id));
    }    /// <summary>
         /// .
         /// </summary>
         /// <returns></returns>
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? BasetableId = null)
    {
        return this.AppOk(_baseTableValueService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, BasetableId: BasetableId));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] BaseTableValueDTO body)
    {
        try
        {
            var result = await _baseTableValueService.CreateForAsync(body);
            return this.AppOk(result);
        }
        catch (Exception ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning(ex, "BaseTableValue Post: duplicate BaseTableId+title.");
            return this.AppOk(OperationResult.Failed(DuplicateBaseTableValueMessage));
        }
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] BaseTableValueDTO body)
    {
        try
        {
            var result = await _baseTableValueService.UpdateForAsync(body);
            return this.AppOk(result);
        }
        catch (Exception ex) when (IsUniqueConstraintViolation(ex))
        {
            _logger.LogWarning(ex, "BaseTableValue Put: duplicate BaseTableId+title.");
            return this.AppOk(OperationResult.Failed(DuplicateBaseTableValueMessage));
        }
    }

    private const string DuplicateBaseTableValueMessage = "عنوان تکراری است. در هر جدول پایه، ترکیب «جدول» و «عنوان» باید یکتا باشد.";

    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        for (var e = ex; e != null; e = e.InnerException)
        {
            if (e is SqlException sql && (sql.Number == 2627 || sql.Number == 2601))
                return true;
        }
        return ex.Message == "امکان ثبت رکورد تکراری وجود ندارد";
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(OperationResult.Failed("امکان حذف مقادیر جداول پایه سیستم وجود ندارد، لطفا با مدیر تماس بگیرید", payload: id));
    }
}
