using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;

namespace HRMS.API.Controllers.BaseInfo;


[Route("api/Places")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("مکان ها")]
public class PlacesController(PlacesService Service, ILogger<PlacesController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly PlacesService _placesService = Service;

    [HttpGet, Route("GetAsKeyValuePairLazy/{filter}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePairLazy(string filter)
    {
        var ret = _placesService.GetAsKeyValuePair(filter);
        return Ok(ret);
    }
    [HttpGet, Route("GetAsKeyValuePairLazy")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePairLazy()
    {

        var ret = _placesService.GetAsKeyValuePair("");

        return Ok(ret);
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(_placesService.GetAsKeyValuePair());
    }

    [HttpGet, Route("getFullPath/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult getFullPath(long id)
    {
        return this.AppOk(_placesService.getFullPath(id));
    }
    [HttpGet, Route("GetAsTree/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetAsTree(long id)
    {
        return this.AppOk(_placesService.GetAsTree(id));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        return this.AppOk(_placesService.Get(id));
    }    /// <summary>
         /// Gets all Base tables.
         /// </summary>
         /// <returns></returns>
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        return this.AppOk(_placesService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> Post([FromBody] PlacesDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                return Ok(await _placesService.CreateForAsync(body));
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

    public async Task<IActionResult> Put([FromBody] PlacesDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var result = await _placesService.UpdateForAsync(body);
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
    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public IActionResult Delete(int id)
    {
        return this.AppOk(OperationResult.Failed("امکان حذف جدول پایه سیستم وجود ندارد، لطفا با مدیر تماس بگیرید", payload: id));
    }
}
