using HR.SharedKernel.Attribute;
using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Data.Entity;
using System.Net.Http.Headers;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;
namespace HRMS.API.Controllers.SystemSetting;

[Route("api/OrganisationMRT")]
[ControllerGroup("SystemSetting", "تنظیمات سیستم")]
[DisplayName("فایل های گزارش سازمان")]
public class OrganisationMRTController : AppBaseController
{
    private readonly OrganisationMRTService _organisationMRTService;
    public OrganisationMRTController(OrganisationMRTService Service, ILogger<OrganisationMRTController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _organisationMRTService = Service;
        _organisationMRTService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_organisationMRTService.GetAsKeyValuePair());
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_organisationMRTService.Get(id));
    }
    [HttpGet, Route("downloadMRT/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadMRT(int id)
    {
        return this.AppOk(_organisationMRTService.downloadMRT(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = ((SystemSettingContext)_organisationMRTService._db).OrganisationMRTs.Include(i => i.OrganisationChart)
        .Where(DateValidityExtension<OrganisationMRT>.GetDateValidationPredicate(IgnoreExpired).And(i => i.OrganisationChartId == currentUserDefaultOrganId))
        ;
        var ret = _organisationMRTService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered);
        foreach (var item in ret.Payload)
        {
            item.Content = null;
        }
        return this.AppOk(ret);
    }
    [HttpPost("uploadNewMRT")]
    [CustomAccessKey(AccessKey: "create")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> uploadNewMRT()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                long maxlength = 0;
                var maxSizeSetting = _organisationMRTService.GetSettingById(10008);
                if (string.IsNullOrEmpty(maxSizeSetting))
                {
                    maxlength = HR.SharedKernel.Share.Constants.defaultMaxUploadSize;
                }
                else
                {
                    maxlength = Convert.ToInt64(maxSizeSetting);
                }
                if (maxlength > (file.Length / 1024))
                {

                }
                else
                {
                    return this.AppBadRequest(OperationResult.Failed("حجم فایل ارسالی بیش از مقدار مجاز تعیین شده می باشد " + maxlength + " KB "));
                }

                if (file.Length > 0)
                {
                    string FileExtn = System.IO.Path.GetExtension(file.FileName);
                    OrganisationTempFile toAdd = new OrganisationTempFile()
                    {
                        CreateDate = DateTime.Now,
                        IPAddress = "FileUpload",
                        title = HR.SharedKernel.Share.Helper.SanitizeFileName(file.FileName),
                        IsDeleted = false,
                        OrganisationChartId = currentUserDefaultOrganId,
                        Size = file.Length,
                        MimeType = HR.SharedKernel.Share.Helper.GetMimeType(FileExtn),
                    };
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        toAdd.Content = fileBytes;
                        //        string s = Convert.ToBase64String(fileBytes);
                    }

                    _organisationMRTService._db.Set<OrganisationTempFile>().Add(toAdd);
                    await _organisationMRTService._db.SaveChangesAsync();

                    return Ok(toAdd.Id);

                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] OrganisationMRTDTO body)
    {
        if (ModelState.IsValid)
        {
            if (body.TempFileId > 0)
            {
                try
                {

                    var tempFile = _organisationMRTService._db.Set<OrganisationTempFile>().Find(body.TempFileId);
                    body.Content = tempFile.Content;
                    body.Size = tempFile.Size;
                    body.MimeType = tempFile.MimeType;
                    body.title = tempFile.title;
                    return Ok(await _organisationMRTService.CreateForAsync(body));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return this.AppBadRequest("Internal Server Error");
                }
            }
            else
            {
                return this.AppBadRequest(OperationResult.Failed("فایل گزارش به درستی بارگزاری نشده است"));
            }

        }
        return this.AppBadRequest(ModelState);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] OrganisationMRTDTO body)
    {
        if (ModelState.IsValid)
        {
            if (body.TempFileId > 0)
            {
                try
                {

                    var tempFile = _organisationMRTService._db.Set<OrganisationTempFile>().Find(body.TempFileId);
                    body.Content = tempFile.Content;
                    body.Size = tempFile.Size;
                    body.MimeType = tempFile.MimeType;
                    body.title = tempFile.title;
                    return Ok(await _organisationMRTService.UpdateForAsync(body));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return this.AppBadRequest("Internal Server Error");
                }

            }
            else
            {
                var existingFile = _organisationMRTService._db.Set<OrganisationMRT>().Find(body.Id);
                if (existingFile.Content != null)
                {
                    body.Content = existingFile.Content;
                    body.Size = existingFile.Size;
                    body.MimeType = existingFile.MimeType;
                    body.title = existingFile.title;
                    return Ok(await _organisationMRTService.UpdateForAsync(body));
                }
                else
                {
                    return this.AppBadRequest(OperationResult.Failed("فایل گزارش به درستی بارگزاری نشده است"));
                }
            }
        }
        return this.AppBadRequest(ModelState);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_organisationMRTService.DeleteRecord(id));
    }
}
