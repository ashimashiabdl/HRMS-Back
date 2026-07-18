using HR.SharedKernel.Attribute;
using AutoMapper;

using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Configuration;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;


namespace HRMS.API.Controllers.Organisation;

[Microsoft.AspNetCore.Mvc.Route("api/OrganisationChartImage")]
[ControllerGroup("Organisation", " ساختار سازمان")]
[DisplayName("تصویر چارت سازمانی")]
public class OrganisationChartImageController : AppBaseController
{
    private readonly OrganisationChartImageService _OrganisationChartImageService;
    private IConfiguration _configuration;
    public OrganisationChartImageController(IConfiguration configuration, OrganisationChartImageService Service, ILogger<OrganisationChartImageController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _configuration = configuration;
        _OrganisationChartImageService = Service;
        _OrganisationChartImageService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("Get/{id}")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_OrganisationChartImageService.Get(id));
    }
    [HttpPost("uploadNewChartImage")]
    [CustomAccessKey(AccessKey: "create")]

    [RequestSizeLimit(100_000_00)]
    public async Task<IActionResult> uploadNewChartImage()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                long maxlength = 0;
                var maxSizeSetting = _OrganisationChartImageService.GetSettingById(10008);

                if (string.IsNullOrEmpty(maxSizeSetting))
                {
                    maxlength = HR.SharedKernel.Share.Constants.defaultMaxUploadSize;
                }
                else
                {
                    maxlength = Convert.ToInt64(maxSizeSetting);
                }
                if (maxlength > (file.Length / 1000))
                {

                }
                else
                {
                    return this.AppBadRequest("حجم فایل ارسالی بیش از مقدار مجاز تعیین شده می باشد " + maxlength + " KB ");
                }
                if (file.Length > 0)
                {
                    string FileExtn = Path.GetExtension(file.FileName);
                    TashkilatTempFile toAdd = new TashkilatTempFile()
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
                    }

                    _OrganisationChartImageService._db.Set<TashkilatTempFile>().Add(toAdd);
                    await _OrganisationChartImageService._db.SaveChangesAsync();
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
    [Microsoft.AspNetCore.Mvc.HttpGet("OrganisationImage/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult OrganisationImage(long id)
    {
        var image = _OrganisationChartImageService.All().Where(i => i.OrganisationChartId == id);
        if (image == null)
        {
            var content = System.IO.File.ReadAllBytes(_configuration.GetValue<string>(WebHostDefaults.ContentRootKey) + "/Assets/Chart.webp");
            return File(content, "image/jpeg");
        }
        else
        {
            if (image.Any())
            {
                return File(image.Single().Content, "image/png");
            }
            else
            {
                var content = System.IO.File.ReadAllBytes(_configuration.GetValue<string>(WebHostDefaults.ContentRootKey) + "/Assets/Chart.webp");
                return File(content, "image/jpeg");
            }
        }
    }

    [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_OrganisationChartImageService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [Microsoft.AspNetCore.Mvc.HttpPost("Post")]

    public async Task<IActionResult> Post([Microsoft.AspNetCore.Mvc.FromBody] OrganisationChartImageDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _OrganisationChartImageService.CreateForAsync(body));
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
    [Microsoft.AspNetCore.Mvc.HttpPut("Put")]

    public async Task<IActionResult> Put([Microsoft.AspNetCore.Mvc.FromBody] OrganisationChartImageDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _OrganisationChartImageService.UpdateForAsync(body);
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
    [Microsoft.AspNetCore.Mvc.HttpDelete("Delete/{id}")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_OrganisationChartImageService.DeleteRecord(id));
    }
}
