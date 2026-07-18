using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;

using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Services;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using System.Linq;
using HR.BaseInfo.Core.Entities;

namespace HRMS.API.Controllers.WorkFlow;

[Route("api/UserSignature")]
[ControllerGroup("WorkFlow", "گردش کار")]
[DisplayName("امضا کاربران")]
public class UserSignatureController : AppBaseController
{
    private readonly UserSignatureService _userSignatureService;
    private readonly WorkFlowContext _context;
    public UserSignatureController(WorkFlowContext context, UserSignatureService Service, ILogger<UserSignatureController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _context = context;
        _userSignatureService = Service;
        _userSignatureService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }
    //[HttpGet, Route("GetAsKeyValuePair")]
    //
    //public IActionResult GetAsKeyValuePair()
    //{
    //    
    //    return this.AppOk(_userSignatureService.GetAsKeyValuePair());
    //}
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(_userSignatureService.Get(id));
    }
    [HttpGet, Route("GetUserSignaturesForCurrentUser")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetUserSignaturesForCurrentUser()
    {
        return this.AppOk(_userSignatureService.GetUserSignaturesForCurrentUser(currentUserId, currentUserDefaultOrganId));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null)
    {
        var filtered = _context.Set<HR.WorkFlow.Core.Data.UserSignature>()
            .Include(i => i.AspNetUsers)
            .Where(HR.SharedKernel.Extensions.DateValidityExtension<HR.WorkFlow.Core.Data.UserSignature>.GetDateValidationPredicate(IgnoreExpired)
                .And(i => (UserId == null || i.AspNetUsersId == UserId)));
        var result = _userSignatureService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: null, CustomDataSource: filtered);
        
        // Map عنوان کاربر از navigation properties به DTO
        if (result != null && result.Success && result.Payload is System.Collections.IEnumerable payloadEnumerable)
        {
            var payloadList = payloadEnumerable.Cast<HR.WorkFlow.Core.DTOs.UserSignatureDTO>().ToList();
            
            // Materialize filtered query فقط برای entities که در نتیجه هستند
            var dtoIds = payloadList.Where(d => d.Id.HasValue).Select(d => d.Id.Value).ToList();
            var entitiesDict = filtered
                .Where(e => dtoIds.Contains(e.Id))
                .ToDictionary(e => e.Id);
            
            foreach (var dto in payloadList)
            {
                if (dto.Id.HasValue && entitiesDict.TryGetValue(dto.Id.Value, out var entity))
                {
                    if (entity.AspNetUsers != null)
                    {
                        dto.AspNetUsers = $"{entity.AspNetUsers.FirstName} {entity.AspNetUsers.LastName}".Trim();
                    }
                }
            }
            result.Payload = payloadList;
        }
        
        return this.AppOk(result);
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] UserSignatureDTO body)
    {
        return Ok(await _userSignatureService.CreateForAsync(body));
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] UserSignatureDTO body)
    {
        var result = await _userSignatureService.UpdateForAsync(body);
        return this.AppOk(result);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(_userSignatureService.DeleteRecord(id));
    }

    [HttpGet("GetSignatureImage/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetSignatureImage(long id)
    {
        var result = _userSignatureService.Get(id);
        if (result.Success && result.Payload is UserSignatureDTO dto && dto.SignatureImageId > 0)
        {
            var signatureResult = _userSignatureService.GetSignatureImageFile(dto.SignatureImageId);
            if (signatureResult.Success && signatureResult.Payload is HR.BaseInfo.Core.Entities.File file && file.Content != null)
            {
                return File(file.Content, file.MimeType ?? "image/png");
            }
        }
        return this.AppNotFound("تصویر نقش امضا یافت نشد");
    }
}
