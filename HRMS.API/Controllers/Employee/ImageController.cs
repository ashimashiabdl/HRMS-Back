using HR.SharedKernel.Attribute;
using AutoMapper;
using Hr.Employee.infrastructure.Services;
using HR.Employee.Core.DTOs;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using HRMS.API.Controllers.Employee;
using HRMS.API.Controllers.SystemSetting;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.Employee;

[Route("api/Image")]
[ControllerGroup("Employee", "اطلاعات کارکنان ")]
[DisplayName("تصویر کارکنان")]
//[EmployeeAccessCheck]
public class ImageController : AppBaseController
{
    private readonly EmployeeImageService _EmployeeImageService;
    private IConfiguration _configuration;
    private readonly EmployeeService _EmployeeService;

    public ImageController(IConfiguration configuration, EmployeeImageService Service, EmployeeService EmployeeService, ILogger<EmployeeController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _EmployeeImageService = Service;
        _configuration = configuration;
        _EmployeeService = EmployeeService;
        _EmployeeImageService._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    [HttpPost("UploadUserImage")]
    [CustomAccessKey(AccessKey: "create")]

    public async Task<IActionResult> UploadUserImage([FromBody] ImageDTO body)
    {
        return this.AppOk(await _EmployeeImageService.UploadUserImage(body));
    }
    [HttpGet("UserImage/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult UserImage(long id)
    {
        var image = _EmployeeImageService.UserImage(id);
        if (image == null)
        {
            var content = System.IO.File.ReadAllBytes(_configuration.GetValue<string>(WebHostDefaults.ContentRootKey) + "/Assets/Unknown_person.webp");
            return File(content, "image/jpeg");
        }
        else
        {
            return File(image, "image/jpeg");
        }
    }
}
