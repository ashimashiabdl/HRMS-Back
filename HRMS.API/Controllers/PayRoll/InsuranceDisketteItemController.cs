using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/InsuranceDisketteItem")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("ردیف دیسکت بیمه")]
public class InsuranceDisketteItemController : AppBaseController
{
    private readonly InsuranceDisketteItemService _InsuranceDisketteItemService;
    
    public InsuranceDisketteItemController(
        InsuranceDisketteItemService InsuranceDisketteItemService, 
        ILogger<InsuranceDisketteItemController> logger, 
        IHttpContextAccessor accessor, 
        IMapper mapper, 
        IDapper dapper, 
        UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _InsuranceDisketteItemService = InsuranceDisketteItemService;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        try
        {
            _logger.LogInformation("InsuranceDisketteItem Get: Getting item {ItemId} by user {UserId}", id, currentUserId);
            
            // Test with a simple query first
            var item = _InsuranceDisketteItemService._unitOfWork.Context.Set<InsuranceDisketteItem>()
                .Include(i => i.Employee)
                .FirstOrDefault(i => i.Id == id);
            
            if (item == null)
            {
                _logger.LogWarning("InsuranceDisketteItem Get: Item {ItemId} not found", id);
                return this.AppBadRequest("ردیف دیسکت بیمه یافت نشد");
            }

            var result = _InsuranceDisketteItemService.GetIdAsync(id).Result;
            
            if (result == null)
            {
                _logger.LogWarning("InsuranceDisketteItem Get: Service returned null for item {ItemId}", id);
                return this.AppBadRequest("خطا در دریافت اطلاعات ردیف دیسکت بیمه");
            }

            _logger.LogInformation("InsuranceDisketteItem Get: Successfully retrieved item {ItemId}", id);
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsuranceDisketteItem Get: Error getting item {ItemId}. UserId: {UserId}", id, currentUserId);
            return this.AppBadRequest("خطا در دریافت اطلاعات ردیف دیسکت بیمه");
        }
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long InsuranceDisketteId = 0)
    {
        try
        {
            _logger.LogInformation("InsuranceDisketteItem GetPagedData: Getting paged data for InsuranceDiskette {InsuranceDisketteId} by user {UserId}", InsuranceDisketteId, currentUserId);
            
            var Filtered = _InsuranceDisketteItemService._unitOfWork.Context.Set<InsuranceDisketteItem>()
                .Include(i => i.Employee)
                .Where(i => i.InsuranceDisketteId == InsuranceDisketteId);

            var result = _InsuranceDisketteItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: Filtered);
            
            
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsuranceDisketteItem GetPagedData: Error getting paged data for InsuranceDiskette {InsuranceDisketteId}. UserId: {UserId}", InsuranceDisketteId, currentUserId);
            return this.AppBadRequest("خطا در دریافت فهرست ردیف‌های دیسکت بیمه");
        }
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] InsuranceDisketteItemDTO body)
    {
        try
        {
            if (body == null)
            {
                _logger.LogWarning("InsuranceDisketteItem Post: body is null");
                return this.AppBadRequest("اطلاعات ردیف دیسکت بیمه ارسال نشده است");
            }

            _logger.LogInformation("InsuranceDisketteItem Post: Creating item for InsuranceDiskette {InsuranceDisketteId} by user {UserId}", 
                body.InsuranceDisketteId, currentUserId);

            var result = await _InsuranceDisketteItemService.CreateForAsync(body);

            if (result == null)
            {
                _logger.LogError("InsuranceDisketteItem Post: CreateForAsync returned null for InsuranceDiskette {InsuranceDisketteId}", 
                    body.InsuranceDisketteId);
                return this.AppBadRequest("خطا در ایجاد ردیف دیسکت بیمه");
            }

            if (result.Success)
            {
                _logger.LogInformation("InsuranceDisketteItem Post: Successfully created item for InsuranceDiskette {InsuranceDisketteId}. Result: {@Result}", 
                    body.InsuranceDisketteId, result);
            }
            else
            {
                _logger.LogWarning("InsuranceDisketteItem Post: Failed to create item for InsuranceDiskette {InsuranceDisketteId}. Message: {Message}", 
                    body.InsuranceDisketteId, result.Message);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsuranceDisketteItem Post: Unhandled exception occurred. UserId: {UserId}, InsuranceDisketteId: {InsuranceDisketteId}", 
                currentUserId, body?.InsuranceDisketteId);
            return this.AppBadRequest("خطای غیرمنتظره در ایجاد ردیف دیسکت بیمه. لطفا با پشتیبانی تماس بگیرید");
        }
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] InsuranceDisketteItemDTO body)
    {
        try
        {
            if (body == null)
            {
                _logger.LogWarning("InsuranceDisketteItem Put: body is null");
                return this.AppBadRequest("اطلاعات ردیف دیسکت بیمه ارسال نشده است");
            }

            if (body.Id <= 0)
            {
                _logger.LogWarning("InsuranceDisketteItem Put: Invalid Id {Id}", body.Id);
                return this.AppBadRequest("شناسه ردیف دیسکت بیمه نامعتبر است");
            }

            _logger.LogInformation("InsuranceDisketteItem Put: Updating item {ItemId} for InsuranceDiskette {InsuranceDisketteId} by user {UserId}", 
                body.Id, body.InsuranceDisketteId, currentUserId);

            var result = await _InsuranceDisketteItemService.UpdateForAsync(body);

            if (result == null)
            {
                _logger.LogError("InsuranceDisketteItem Put: UpdateForAsync returned null for item {ItemId}", body.Id);
                return this.AppBadRequest("خطا در بروزرسانی ردیف دیسکت بیمه");
            }

            if (result.Success)
            {
                _logger.LogInformation("InsuranceDisketteItem Put: Successfully updated item {ItemId}. Result: {@Result}", body.Id, result);
            }
            else
            {
                _logger.LogWarning("InsuranceDisketteItem Put: Failed to update item {ItemId}. Message: {Message}", body.Id, result.Message);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsuranceDisketteItem Put: Unhandled exception occurred. UserId: {UserId}, ItemId: {ItemId}", 
                currentUserId, body?.Id);
            return this.AppBadRequest("خطای غیرمنتظره در بروزرسانی ردیف دیسکت بیمه. لطفا با پشتیبانی تماس بگیرید");
        }
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public IActionResult Delete(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("InsuranceDisketteItem Delete: Invalid Id {Id}", id);
                return this.AppBadRequest("شناسه ردیف دیسکت بیمه نامعتبر است");
            }

            _logger.LogInformation("InsuranceDisketteItem Delete: Deleting item {ItemId} by user {UserId}", id, currentUserId);

            var result = _InsuranceDisketteItemService.DeleteRecord(id);

            if (result == null)
            {
                _logger.LogError("InsuranceDisketteItem Delete: DeleteRecord returned null for item {ItemId}", id);
                return this.AppBadRequest("خطا در حذف ردیف دیسکت بیمه");
            }

            if (result.Success)
            {
                _logger.LogInformation("InsuranceDisketteItem Delete: Successfully deleted item {ItemId}. Result: {@Result}", id, result);
            }
            else
            {
                _logger.LogWarning("InsuranceDisketteItem Delete: Failed to delete item {ItemId}. Message: {Message}", id, result.Message);
            }

            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsuranceDisketteItem Delete: Unhandled exception occurred. UserId: {UserId}, ItemId: {ItemId}", 
                currentUserId, id);
            return this.AppBadRequest("خطای غیرمنتظره در حذف ردیف دیسکت بیمه. لطفا با پشتیبانی تماس بگیرید");
        }
    }
}
