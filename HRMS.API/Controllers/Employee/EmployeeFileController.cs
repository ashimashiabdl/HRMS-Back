using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.Employee.infrastructure.Data;
using Hr.Employee.infrastructure.Services;

using HRMS.API.Controllers.SystemSetting;
using HR.BaseInfo.Core.DTOs;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HRMS.API.Controllers.Employee;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/EmployeeFile")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("مستندات")]
[EmployeeAccessCheck]
public class EmployeeFileController : AppBaseController
{
    private readonly EmployeeFileService _EmployeeFileService;
    private IConfiguration _configuration;
    private readonly EmployeeService _EmployeeService;
    private readonly UserResolverService _userResolverService;

    public EmployeeFileController(IConfiguration configuration, EmployeeFileService Service, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _userResolverService = UserResolverService;
        _EmployeeFileService = Service;
        _configuration = configuration;
        _EmployeeService = EmployeeService;
        _EmployeeFileService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_EmployeeFileService.Get(id));
    }
    [HttpGet, Route("downloadFile/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadFile(int id)
    {
        return this.AppOk(_EmployeeFileService.downloadFile(id));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] EmployeeFileDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (body.TempFileId > 0)
                {

                    //var tempFile = _EmployeeFileService._db.Set<EmployeeFile>().Find(body.TempFileId);
                    //body.Content = tempFile.Content;
                    //body.Size = tempFile.Size;
                    //body.MimeType = tempFile.MimeType;
                    //body.title = tempFile.title;
                    //body.TempFileId = 
                    body.FileId = body.TempFileId;
                    return Ok(await _EmployeeFileService.CreateForAsync(body));
                }
                else
                {
                    return this.AppBadRequest(OperationResult.Failed("فایل گزارش به درستی بارگزاری نشده است"));
                }

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
    public async Task<IActionResult> Put([FromBody] EmployeeFileDTO body)
    {
        return this.AppOk(await _EmployeeFileService.UpdateForAsync(body));
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
        var Filtered = ((EmployeeContext)_EmployeeFileService._db).EmployeeFiles
        .Include(i => i.File)
        .Include(i => i.FileGroup)
        .Include(i => i.OrganisationChart)

        .Where(DateValidityExtension<EmployeeFile>.GetDateValidationPredicate(IgnoreExpired).And(i => i.OrganisationChartId == currentUserDefaultOrganId && i.EmployeeId == EmployeeId))
;
        var ret = _EmployeeFileService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered);
        return this.AppOk(ret);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_EmployeeFileService.DeleteRecord(id));
    }
    [HttpPost("uploadNewFile")]
    [CustomAccessKey(AccessKey: "create")]
    [DisableRequestSizeLimit]
    [HRMS.API.Infrastructure.Upload.SkipFileSignatureValidation]
    public async Task<IActionResult> UploadNewFile()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                long maxlength = _configuration.GetValue<long>("FileUploadValidation:MaxFileSizeBytes", 200L * 1024 * 1024);
                if (maxlength < (file.Length))
                {
                    return this.AppBadRequest(OperationResult.Failed("حجم فایل ارسالی بیش از مقدار مجاز تعیین شده می باشد " + (maxlength / 1024 / 1024) + " MB "));
                }


                if (file.Length > 0)
                {
                    string FileExtn = System.IO.Path.GetExtension(file.FileName);
                    HR.Employee.Core.Entities.File toAdd = new HR.Employee.Core.Entities.File()
                    {
                        CreateDate = DateTime.Now,
                        IPAddress = _userResolverService.GetIP(),
                        title = HR.SharedKernel.Share.Helper.SanitizeFileName(file.FileName),
                        IsDeleted = false,
                        OrganisationChartId = currentUserDefaultOrganId,
                        Size = file.Length,
                        UniqueId = Guid.NewGuid(),
                        MimeType = HR.SharedKernel.Share.Helper.GetMimeType(FileExtn),
                        Extension = FileExtn
                    };
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        toAdd.Content = fileBytes;
                    }

                    _EmployeeFileService._db.Set<HR.Employee.Core.Entities.File>().Add(toAdd);
                    await _EmployeeFileService._db.SaveChangesAsync();
                    return Ok(toAdd.Id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        return this.AppBadRequest(ModelState);
    }
}
