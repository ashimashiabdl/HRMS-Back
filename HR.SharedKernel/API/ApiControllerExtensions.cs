using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace HR.SharedKernel.API;

public static class ApiControllerExtensions
{
    public static IActionResult AppOk(this ControllerBase controller, OperationResult result)
    {
        return controller.Ok(result);
    }
    public static IActionResult AppOk(this ControllerBase controller, dynamic data)
    {
        return controller.Ok(new { success = true, data = data });
    }
    public static IActionResult AppBadRequest(this ControllerBase controller, dynamic data)
    {
        return controller.BadRequest(new { success = false, data = data });
    }

    public static IActionResult AppNotFound(this ControllerBase controller, string msg = null)
    {
        return controller.NotFound(new { success = false, message = msg });
    }

    public static IActionResult AppBadRequest(this ControllerBase controller, ModelStateDictionary modelState)
    {
        var msg = string.Join(',', modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
        return controller.BadRequest(new { success = false, message = msg });
    }
}
