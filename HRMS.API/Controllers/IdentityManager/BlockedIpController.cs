using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/BlockedIp")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("آدرس‌های IP مسدود شده")]
public class BlockedIpController : AppBaseController
{
    private readonly BlockedIpService _blockedIpService;
    private readonly BlockedIpSecurityService _blockedIpSecurityService;
    
    public BlockedIpController(BlockedIpService blockedIpService, BlockedIpSecurityService blockedIpSecurityService, ILogger<BlockedIpController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService userResolverService) 
        : base(userResolverService, logger, accessor, null, dapper)
    {
        _blockedIpService = blockedIpService;
        _blockedIpService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _blockedIpSecurityService = blockedIpSecurityService;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_blockedIpService.Get(id));
    }

    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_blockedIpService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: null));
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] BlockedIpDTO body)
    {
        var result = await _blockedIpService.CreateForAsync(body);
        
        // به‌روزرسانی کش بعد از ایجاد
        if (result.Success && result.Payload != null)
        {
            await _blockedIpSecurityService.UpdateCacheForIpAsync(body.IpAddress, body.IsActive);
        }
        
        return Ok(result);
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] BlockedIpDTO body)
    {
        // دریافت اطلاعات قبلی برای به‌روزرسانی کش
        string? oldIpAddress = null;
        if (body.Id.HasValue && body.Id.Value > 0)
        {
            var existing = _blockedIpService.Get(body.Id.Value);
            if (existing != null && existing.Payload != null)
            {
                var existingDto = existing.Payload as BlockedIpDTO;
                if (existingDto != null)
                {
                    oldIpAddress = existingDto.IpAddress;
                }
            }
        }

        var result = await _blockedIpService.UpdateForAsync(body);
        
        // به‌روزرسانی کش بعد از بروزرسانی
        if (result.Success)
        {
            // اگر IP تغییر کرده، کش IP قبلی را حذف کن
            if (!string.IsNullOrWhiteSpace(oldIpAddress) && oldIpAddress != body.IpAddress)
            {
                await _blockedIpSecurityService.UpdateCacheForIpAsync(oldIpAddress, false);
            }
            
            // به‌روزرسانی کش IP فعلی (با IsActive جدید)
            if (!string.IsNullOrWhiteSpace(body.IpAddress))
            {
                await _blockedIpSecurityService.UpdateCacheForIpAsync(body.IpAddress, body.IsActive);
            }
        }
        
        return this.AppOk(result);
    }

    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> Delete(int id)
    {
        // دریافت IP قبل از حذف برای به‌روزرسانی کش
        string? ipAddress = null;
        var existing = _blockedIpService.Get(id);
        if (existing != null && existing.Payload != null)
        {
            var existingDto = existing.Payload as BlockedIpDTO;
            if (existingDto != null)
            {
                ipAddress = existingDto.IpAddress;
            }
        }

        var result = _blockedIpService.DeleteRecord(id);
        
        // حذف از کش بعد از حذف رکورد
        if (result.Success && !string.IsNullOrWhiteSpace(ipAddress))
        {
            await _blockedIpSecurityService.UpdateCacheForIpAsync(ipAddress, false);
        }
        
        return this.AppOk(result);
    }
}

