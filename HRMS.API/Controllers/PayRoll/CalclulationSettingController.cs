using HR.SharedKernel.Attribute;

using AutoMapper;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/CalclulationSetting")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("تنظیمات محاسبات")]
public class CalclulationSettingController : AppBaseController
{
    private readonly CalclulationSettingService _CalclulationSettingService;
    public CalclulationSettingController(CalclulationSettingService Service, ILogger<CalclulationSettingController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _CalclulationSettingService = Service;
        _CalclulationSettingService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]

    public IActionResult Get(int id)
    {
        var existing = _CalclulationSettingService.All().Where(i => i.OrganisationChartId == currentUserDefaultOrganId);
        if (existing == null)
        {
            return this.AppOk(OperationResult.Succeeded(payload: new CalclulationSettingDTO()
            {
                OrganisationChartId = currentUserDefaultOrganId
            }));
        }
        else
        {
            if (existing.Any())
            {
                return this.AppOk(OperationResult.Succeeded(payload: _mapper.Map<CalclulationSettingDTO>(existing.Single())));
            }
            else
            {
                return this.AppOk(OperationResult.Succeeded(payload: new CalclulationSettingDTO()
                {
                    OrganisationChartId = currentUserDefaultOrganId
                }));
            }
        }
    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]

    public async Task<IActionResult> Put([FromBody] CalclulationSettingDTO body)
    {
        if (ModelState.IsValid)
        {
            try
            {

                var existing = _CalclulationSettingService.All().Where(i => i.OrganisationChartId == currentUserDefaultOrganId);
                if (existing == null)
                {
                    var result = await _CalclulationSettingService.UpdateForAsync(body);
                    return this.AppOk(result);
                }
                else
                {
                    if (existing.Any())
                    {
                        body.Id = existing.Single().Id;
                        var result = await _CalclulationSettingService.UpdateForAsync(body);
                        return this.AppOk(result);
                    }
                    else
                    {
                        var result = await _CalclulationSettingService.CreateForAsync(body);
                        return this.AppOk(result);
                    }
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
}
