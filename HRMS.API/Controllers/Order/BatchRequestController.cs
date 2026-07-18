using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.Core.DTOs;
using HRMS.API.Controllers.Order;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Data;
using HR.Order.Infrastructure.Services;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Infrastructure.Upload;
using System.ComponentModel;

namespace HRMS.API.Controllers.Order;

[Route("api/BatchRequest")]
[ControllerGroup("OrderNameSpace", " احکام")]
[DisplayName("صدور گروهی احکام")]
public class BatchRequestController : AppBaseController
{
    private readonly BatchRequestService _batchRequestService;
    private OrderContext _context;
    public BatchRequestController(BatchRequestService BatchRequestService, OrderContext OrderContext, ILogger<OrderController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = OrderContext;
        _batchRequestService = BatchRequestService;

        _batchRequestService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_batchRequestService.Get(id));
    }
    [HttpGet, Route("GetBatchRequestArchiveStatus/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetBatchRequestArchiveStatus(int id)
    {
        return this.AppOk(_batchRequestService.GetBatchRequestArchiveStatus(id));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var Filtered = _context.BatchRequests
            .Include(i => i.OrderType)
            .Include(i => i.OrganisationChart)
            .Include(i => i.RequestState)
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId);
        var paged = _batchRequestService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: Filtered);
        return this.AppOk(paged);
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public IActionResult Post([FromBody] BatchRequestDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                body.IssuerUser = CurrentUserName;
                if (currentUserEmployeeId > 0)
                {
                    //body.RequesterEmployeeId = CurrentUserName;
                }
                return Ok(_batchRequestService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] BatchRequestDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _batchRequestService.UpdateForAsync(body);
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
    [HttpPut("UpdateState")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> UpdateState([FromBody] BatchRequestStateUpdateDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _batchRequestService.UpdateState(body);
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

    [HttpPut("UpdateArchiveState")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> UpdateArchiveState([FromBody] BatchRequestStateUpdateDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _batchRequestService.UpdateArchiveState(body);
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
    [HttpDelete("DeleteAllDrafts/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult DeleteAllDrafts(long id)
    {
        var result = _batchRequestService.DeleteAllDrafts(id);
        return this.AppOk(result);
    }
    [HttpDelete("DeleteAllArchives/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult DeleteAllArchives(long id)
    {
        var result = _batchRequestService.DeleteAllArchives(id);
        return this.AppOk(result);
    }

    [HttpPut("SendAllDraftsToCartable/{id}")]
    [CustomAccessKey(AccessKey: "update")]
    public IActionResult SendAllDraftsToCartable(long id)
    {
        var result = _batchRequestService.SendAllDraftsToCartable(id);
        return this.AppOk(result);
    }
    [HttpGet("GetBatchRequestFiles/{id}")]

    public IActionResult GetBatchRequestFiles(long id)
    {
        var result = _batchRequestService.GetBatchRequestFiles(id);
        return this.AppOk(result);
    }

    [HttpGet, Route("downloadFile/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult downloadFile(int id)
    {
        return this.AppOk(_batchRequestService.downloadFile(id));
    }

    [HttpDelete, Route("deleteFile/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult deleteFile(int id)
    {
        return this.AppOk(_batchRequestService.deleteFile(id));
    }

    [HttpPost("uploadTempFile")]
    [CustomAccessKey(AccessKey: "create")]
    [AllowUploadExtensions(".xlsx", ".xls")]
    [RequestSizeLimit(100_000_00)]
    public async Task<IActionResult> uploadTempFile()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                long maxlength = 0;
                var maxSizeSetting = _batchRequestService.GetSettingById(10008);

                if (string.IsNullOrEmpty(maxSizeSetting))
                {
                    maxlength = HR.SharedKernel.Share.Constants.defaultMaxUploadSize;
                }
                else
                {
                    maxlength = Convert.ToInt64(maxSizeSetting);
                }
                if (maxlength > file.Length / 1000)
                {

                }
                else
                {
                    return this.AppBadRequest("حجم فایل ارسالی بیش از مقدار مجاز تعیین شده می باشد " + maxlength + " KB ");
                }
                if (file.Length > 0)
                {
                    string FileExtn = Path.GetExtension(file.FileName);
                    OrderTempFile toAdd = new OrderTempFile()
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

                    _batchRequestService._db.Set<OrderTempFile>().Add(toAdd);
                    await _batchRequestService._db.SaveChangesAsync();

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


    [HttpGet, Route("getWageExcelPreview/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult getWageExcelPreview(int id, [FromQuery] bool useMappedColumns = true)
    {
        return this.AppOk(_batchRequestService.getWageExcelPreview(id, true, useMappedColumns));
    }

    [HttpGet, Route("getCoefExcelPreview/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult getCoefExcelPreview(int id, [FromQuery] bool useMappedColumns = true)
    {
        return this.AppOk(_batchRequestService.getCoefExcelPreview(id, true, useMappedColumns));
    }

    [HttpGet, Route("getDatesExcelPreview/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult getDatesExcelPreview(int id)
    {
        return this.AppOk(_batchRequestService.getDatesExcelPreview(id, true));
    }

    [HttpGet, Route("getDatesFromWageExcelPreview/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult getDatesFromWageExcelPreview(int id)
    {
        return this.AppOk(_batchRequestService.getDatesFromWageExcelPreview(id, true));
    }
}
