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
using HRMS.API.Cache;
using System.ComponentModel;

namespace HRMS.API.Controllers.BaseInfo;

[Route("api/TempGlobalFile")]
[ControllerGroup("baseInfo", "اطلاعات پایه ")]
[DisplayName("فایل ها")]
public class TempGlobalFileController(TempGlobalFileService Service, ILogger<TempGlobalFileController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, mapper, dapper)
{
    private readonly TempGlobalFileService _TempGlobalFileService = Service;

    [HttpPost("UploadNewFile")]
    [CustomAccessKey(AccessKey: "create")]

    [RequestSizeLimit(100_000_00)]
    public async Task<IActionResult> UploadNewFile()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    string FileExtn = System.IO.Path.GetExtension(file.FileName);
                    TempGlobalFile toAdd = new TempGlobalFile()
                    {
                        CreateDate = DateTime.Now,
                        IPAddress = "_Ip",
                        title = HR.SharedKernel.Share.Helper.SanitizeFileName(file.FileName),
                        IsDeleted = false,
                        Size = file.Length,
                        UniqueId = Guid.NewGuid(),
                        Extension = FileExtn,
                        MimeType = HR.SharedKernel.Share.Helper.GetMimeType(FileExtn),
                    };
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        toAdd.Content = fileBytes;
                    }

                    _TempGlobalFileService._db.Set<TempGlobalFile>().Add(toAdd);
                    await _TempGlobalFileService._db.SaveChangesAsync();
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
