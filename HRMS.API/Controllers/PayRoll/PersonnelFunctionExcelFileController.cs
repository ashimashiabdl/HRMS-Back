using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HR.Payroll.Core.Data;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/PersonnelFunctionExcelFile")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("فایل‌های اکسل کارکرد")]
public class PersonnelFunctionExcelFileController : AppBaseController
{
    private readonly PersonnelFunctionExcelFileService _service;
    private readonly IConfiguration _configuration;

    public PersonnelFunctionExcelFileController(
        PersonnelFunctionExcelFileService service,
        IConfiguration configuration,
        ILogger<PersonnelFunctionExcelFileController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService) : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _configuration = configuration;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    /// <summary>
    /// دریافت فهرست فایل‌های اکسل کارکرد (بدون محتوا برای بهینه‌سازی)
    /// </summary>
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        [FromQuery] string filter = "",
        [FromQuery] string activeSortColumn = "",
        [FromQuery] string Sortdirection = "",
        [FromQuery] long? paymentPeriodId = null,
        [FromQuery] long? employeeTypeId = null)
    {
        return this.AppOk(_service.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, true, paymentPeriodId, employeeTypeId));
    }

    /// <summary>
    /// دریافت اطلاعات یک فایل (بدون محتوا)
    /// </summary>
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(long id)
    {
        var result = _service.Get(id);
        if (result != null)
        {
            // حذف Content برای جلوگیری از ارسال حجم زیاد داده
            result.Payload.Content = null;
        }
        return this.AppOk(result);
    }

    /// <summary>
    /// دانلود فایل
    /// </summary>
    [HttpGet, Route("DownloadFile/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult DownloadFile(long id)
    {
        var result = _service.DownloadFile(id);
        if (result.Success && result.Payload != null)
        {
            var fileDto = result.Payload as PersonnelFunctionExcelFileDTO;
            if (fileDto != null && fileDto.Content != null)
            {
                return File(fileDto.Content, fileDto.MimeType ?? "application/octet-stream", fileDto.FileName);
            }
        }
        return this.AppBadRequest(result);
    }

    /// <summary>
    /// حذف فایل اکسل کارکرد
    /// </summary>
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(long id)
    {
        return this.AppOk(_service.DeleteRecord(id));
    }
}

