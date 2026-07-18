using HR.SharedKernel.Attribute;
using HR.Identity.Core.DTOs;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel;
using AutoMapper;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/AspNetRoles")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("مدیریت نقش ها")]
public class AspNetRolesController(RoleManager<AspNetRoles> roleManager, IdentityContext IdentityContext, IMapper mapper, ILogger<AspNetRolesController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, null, dapper)
{
    private readonly RoleManager<AspNetRoles> _roleManager = roleManager;
    IdentityContext _identityContext = IdentityContext;
    IMapper _mapper = mapper;

    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(OperationResult.Succeeded(payload: _roleManager.Roles.Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            id = i.Id,
            key = i.Id,
            value = i.PersianName
        })));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public async Task<IActionResult> Get(long id)
    {


        return this.AppOk(OperationResult.Succeeded(payload: _mapper.Map<AspNetRolesDTO>(await _identityContext.AspNetRoles.FindAsync(id))));

    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        var paged = _identityContext.Roles;
        int rowCount = 0;
        return this.AppOk(OperationResult.Succeeded(payload: _mapper.Map<List<AspNetRolesDTO>>(PagerUtility<AspNetRoles>.GetPagedData(paged, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection))));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] AspNetRolesDTO body)
    {
        var resp = await _roleManager.CreateAsync(new AspNetRoles()
        {
            CreateDate = DateTime.Now,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            Name = body.Name,

            PersianName = body.PersianName,
            NormalizedName = body.Name.ToUpper(),
            //OrganisationChartId = 5
        });


        if (resp.Succeeded)
        {
            return this.AppOk(resp);
        }
        else
        {
            return this.AppBadRequest(resp.Errors.First().Description);
        }


    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] AspNetRolesDTO body)
    {

        var selected = await _roleManager.FindByIdAsync(body.Id.ToString());
        selected.PersianName = body.PersianName;
        selected.Name = body.Name;

        var resp = await _roleManager.UpdateAsync(selected);

        return this.AppOk(resp);
    }
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(1);
    }
}



