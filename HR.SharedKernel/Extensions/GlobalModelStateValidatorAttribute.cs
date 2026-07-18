using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HR.SharedKernel.Extensions
{
    public class GlobalModelStateValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                Controller controller = context.Controller as Controller;
                object model = context.ActionArguments.Any()
                   ? context.ActionArguments.First().Value
                   : null;

                IEnumerable<ModelError> allErrors = context.ModelState.Values.SelectMany(v => v.Errors);
                var errorMessages = allErrors.Select(e => e.ErrorMessage).ToList();

                // Check if any error is a JSON conversion error
                var jsonError = errorMessages.FirstOrDefault(e => 
                    e?.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) == true ||
                    e?.Contains("JSON value could not be converted", StringComparison.OrdinalIgnoreCase) == true ||
                    e?.Contains("body", StringComparison.OrdinalIgnoreCase) == true ||
                    e?.Contains("required", StringComparison.OrdinalIgnoreCase) == true);

                string detail;
                if (jsonError != null)
                {
                    // Extract field name from error message if possible
                    var pathMatch = Regex.Match(jsonError, @"Path:\s*\$\\.([^\s|]+)");
                    var fieldName = pathMatch.Success ? pathMatch.Groups[1].Value.Trim() : null;
                    
                    if (!string.IsNullOrEmpty(fieldName))
                        detail = $"داده‌های ارسالی نامعتبر است. مقدار فیلد «{fieldName}» باید عدد صحیح باشد. لطفاً فیلدهای فرم را بررسی کنید.";
                    else if (jsonError.Contains("body", StringComparison.OrdinalIgnoreCase) || jsonError.Contains("required", StringComparison.OrdinalIgnoreCase))
                        detail = "بدنه درخواست خالی یا نامعتبر است. لطفاً اطلاعات را ارسال کنید.";
                    else
                        detail = "داده‌های ارسالی نامعتبر است. لطفاً فیلدهای فرم (از جمله فیلدهای عددی مانند وزن، قد، سایز) را به درستی پر کنید.";
                }
                else
                {
                    // Generic validation error - join all error messages
                    detail = string.Join(" ", errorMessages);
                }

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "درخواست نامعتبر",
                    Detail = detail
                };

                context.Result = new BadRequestObjectResult(problemDetails);
            }

            base.OnActionExecuting(context);
        }
    }
}
