using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Attribute;

public class LogActionCallsAttribute(ILogger<LogActionCallsAttribute> logger) : IActionFilter
{
    protected readonly ILogger<LogActionCallsAttribute> _logger = logger;

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var timestamp = DateTime.Now;

        _logger.LogDebug($"[Action End] {timestamp:u} - {actionName}");

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var timestamp = DateTime.Now;

        var parameters = string.Join(", ", context.ActionArguments.Select(p => $"{p.Key}={p.Value}"));

        _logger.LogDebug($"[Action Start] {timestamp:u} - {actionName} | Params: {parameters}");

    }
}
