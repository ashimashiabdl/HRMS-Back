using Hr.Employee.infrastructure.Services;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Service;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace HRMS.API.Cache;

public sealed class EmployeeAccessCheckAttribute : TypeFilterAttribute
{
    public EmployeeAccessCheckAttribute() : base(typeof(EmployeeAccessCheckFilter))
    {
    }

    private sealed class EmployeeAccessCheckFilter(EmployeeService employeeService, EmployeeContext employeeContext, UserResolverService userResolverService) : IAsyncActionFilter
    {
        private readonly EmployeeService _employeeService = employeeService;
        private readonly EmployeeContext _employeeContext = employeeContext;
        private readonly UserResolverService _userResolverService = userResolverService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpMethod = context.HttpContext.Request?.Method?.ToUpperInvariant();
            
            // Get controller name to resolve correct entity
            var controllerName = string.Empty;
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                controllerName = descriptor.ControllerName;
            }

            // 1) Pre-execution checks
            if (httpMethod == "POST" || httpMethod == "PUT" || httpMethod == "GET")
            {
                // For POST/PUT and for GET endpoints that accept EmployeeId (e.g., GetPagedData), check upfront
                long preEmployeeId = TryResolveEmployeeId(context.ActionArguments, _employeeContext, controllerName);
                if (preEmployeeId > 0)
                {
                    var currentUserId = _userResolverService.GetUserId();
                    var hasAccess = _employeeService.CheckAccess(currentUserId, preEmployeeId);
                    if (!hasAccess)
                    {
                        context.Result = new BadRequestObjectResult(new { success = false, data = "کاربر جاری به کارمند مورد نظر دسترسی ندارد" });
                        return;
                    }
                }
            }

            // Proceed to action
            var executedContext = await next();

            // 2) Post-execution checks for GET single item responses (access based on returned DTO.EmployeeId)
            if (httpMethod == "GET" && executedContext.Result is ObjectResult objectResult)
            {
                var payload = objectResult.Value;
                if (payload == null) return;

                // AppOk wraps as { success = true, data = <dto> }
                var dataProp = payload.GetType().GetProperty("data");
                var data = dataProp != null ? dataProp.GetValue(payload) : payload;
                if (data == null) return;

                // Skip collections (only enforce for single-record GET)
                if (data is System.Collections.IEnumerable && !(data is string))
                {
                    return;
                }

                // The intention is to check access on the EmployeeId of the returned record,
                // not on the id used to fetch it.
                long dtoEmployeeId = ResolveEmployeeIdFromObject(data, _employeeContext);
                if (dtoEmployeeId > 0)
                {
                    var currentUserId = _userResolverService.GetUserId();
                    var hasAccess = _employeeService.CheckAccess(currentUserId, dtoEmployeeId);
                    if (!hasAccess)
                    {
                        executedContext.Result = new BadRequestObjectResult(new { success = false, data = "کاربر جاری به کارمند مورد نظر دسترسی ندارد" });
                    }
                }
            }
        }

        private static long TryResolveEmployeeId(IDictionary<string, object> actionArguments, EmployeeContext employeeContext, string controllerName)
        {
            if (actionArguments == null || actionArguments.Count == 0)
            {
                return 0;
            }

            // 1) Prefer explicit parameter named "EmployeeId" (case-insensitive)
            foreach (var kvp in actionArguments)
            {
                if (kvp.Value == null) continue;
                if (string.Equals(kvp.Key, "EmployeeId", StringComparison.OrdinalIgnoreCase))
                {
                    var empId = CoerceToLong(kvp.Value);
                    if (empId > 0) return empId;
                }
            }

            // 2) Consider other parameter names that end with EmployeeId (e.g., some binding variants)
            foreach (var kvp in actionArguments)
            {
                if (kvp.Value == null) continue;
                if (kvp.Key != null && kvp.Key.EndsWith("EmployeeId", StringComparison.OrdinalIgnoreCase))
                {
                    var empId = CoerceToLong(kvp.Value);
                    if (empId > 0) return empId;
                }
            }

            // 3) Complex types with EmployeeId property
            foreach (var kvp in actionArguments)
            {
                var argument = kvp.Value;
                if (argument == null) continue;

                var type = argument.GetType();
                var prop = type.GetProperty("EmployeeId");
                if (prop != null)
                {
                    var value = prop.GetValue(argument);
                    var empId = CoerceToLong(value);
                    if (empId > 0) return empId;
                }

                // Special case: InsuranceDetailDTO via InsuranceId -> parent.EmployeeId
                var insuranceIdProp = type.GetProperty("InsuranceId");
                if (insuranceIdProp != null)
                {
                    var iidVal = insuranceIdProp.GetValue(argument);
                    long insuranceId = CoerceToLong(iidVal);

                    if (insuranceId > 0)
                    {
                        var parent = employeeContext.Insurances.FirstOrDefault(i => i.Id == insuranceId);
                        if (parent != null && parent.EmployeeId > 0)
                        {
                            return parent.EmployeeId;
                        }
                    }
                }
            }

            // 4) Try to resolve EmployeeId from related entities when 'id' parameter is present
            // This handles cases like Get(int id) where id is the entity's primary key, not EmployeeId
            foreach (var kvp in actionArguments)
            {
                if (kvp.Value == null) continue;
                if (string.Equals(kvp.Key, "id", StringComparison.OrdinalIgnoreCase))
                {
                    long recordId = CoerceToLong(kvp.Value);
                    if (recordId > 0)
                    {
                        // Try to find EmployeeId from related entities based on controller name
                        long resolvedEmployeeId = TryResolveEmployeeIdFromRelatedEntity(recordId, employeeContext, controllerName);
                        if (resolvedEmployeeId > 0)
                        {
                            return resolvedEmployeeId;
                        }
                    }
                }
            }

            return 0;
        }

        private static long TryResolveEmployeeIdFromRelatedEntity(long recordId, EmployeeContext employeeContext, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                return 0;
            }

            // Map controller name to the corresponding DbSet and query for EmployeeId
            switch (controllerName)
            {
                case "Education":
                    var education = employeeContext.Educations.FirstOrDefault(e => e.Id == recordId);
                    return education?.EmployeeId ?? 0;

                case "Insurance":
                    var insurance = employeeContext.Insurances.FirstOrDefault(i => i.Id == recordId);
                    return insurance?.EmployeeId ?? 0;

                case "Course":
                    var course = employeeContext.Courses.FirstOrDefault(c => c.Id == recordId);
                    return course?.EmployeeId ?? 0;

                case "Family":
                    var family = employeeContext.Families.FirstOrDefault(f => f.Id == recordId);
                    return family?.EmployeeId ?? 0;

                case "ContactInfo":
                    var contactInfo = employeeContext.ContactInfos.FirstOrDefault(c => c.Id == recordId);
                    return contactInfo?.EmployeeId ?? 0;

                case "BankAccount":
                    var bankAccount = employeeContext.BankAccounts.FirstOrDefault(b => b.Id == recordId);
                    return bankAccount?.EmployeeId ?? 0;

                case "Basij":
                    var basij = employeeContext.Basijs.FirstOrDefault(b => b.Id == recordId);
                    return basij?.EmployeeId ?? 0;

                case "BasijGrade":
                    var basijGrade = employeeContext.BasijGrades.FirstOrDefault(b => b.Id == recordId);
                    return basijGrade?.EmployeeId ?? 0;

                case "Captivity":
                    var captivity = employeeContext.Captivities.FirstOrDefault(c => c.Id == recordId);
                    return captivity?.EmployeeId ?? 0;

                case "Coefficient":
                    var coefficient = employeeContext.Coefficients.FirstOrDefault(c => c.Id == recordId);
                    return coefficient?.EmployeeId ?? 0;

                case "Competency":
                    var competency = employeeContext.Competencies.FirstOrDefault(c => c.Id == recordId);
                    return competency?.EmployeeId ?? 0;

                case "Disability":
                    var disability = employeeContext.Disabilities.FirstOrDefault(d => d.Id == recordId);
                    return disability?.EmployeeId ?? 0;

                case "DrivingLicense":
                    var drivingLicense = employeeContext.DrivingLicenses.FirstOrDefault(d => d.Id == recordId);
                    return drivingLicense?.EmployeeId ?? 0;

                case "EvaluationResult":
                    var evaluationResult = employeeContext.EvaluationResults.FirstOrDefault(e => e.Id == recordId);
                    return evaluationResult?.EmployeeId ?? 0;

                case "ForeignTravel":
                    var foreignTravel = employeeContext.ForeignTravels.FirstOrDefault(f => f.Id == recordId);
                    return foreignTravel?.EmployeeId ?? 0;

                case "ForeignLanguage":
                
                    var ForeignLanguage = employeeContext.ForeignLanguages.FirstOrDefault(l => l.Id == recordId);
                    return ForeignLanguage?.EmployeeId ?? 0;

                case "Image":
                    var image = employeeContext.Images.FirstOrDefault(i => i.Id == recordId);
                    return image?.EmployeeId ?? 0;

                case "Isar":
                    var isar = employeeContext.Isars.FirstOrDefault(i => i.Id == recordId);
                    return isar?.EmployeeId ?? 0;

                case "MilitaryService":
                    var militaryService = employeeContext.MilitaryServices.FirstOrDefault(m => m.Id == recordId);
                    return militaryService?.EmployeeId ?? 0;

                case "OtherVeteran":
                    var otherVeteran = employeeContext.OtherVeterans.FirstOrDefault(o => o.Id == recordId);
                    return otherVeteran?.EmployeeId ?? 0;

                case "War":
                    var war = employeeContext.Wars.FirstOrDefault(w => w.Id == recordId);
                    return war?.EmployeeId ?? 0;

                case "HistoryStop":
                    var historyStop = employeeContext.HistoryStops.FirstOrDefault(h => h.Id == recordId);
                    return historyStop?.EmployeeId ?? 0;

                case "EmployeeFile":
                    var employeeFile = employeeContext.EmployeeFiles.FirstOrDefault(e => e.Id == recordId);
                    return employeeFile?.EmployeeId ?? 0;

                case "Work":
                    var work = employeeContext.Works.FirstOrDefault(w => w.Id == recordId);
                    return work?.EmployeeId ?? 0;

                case "EmployeeSoftware":
                    var employeeSoftware = employeeContext.EmployeeSoftwares.FirstOrDefault(s => s.Id == recordId);
                    return employeeSoftware?.EmployeeId ?? 0;

                case "Ability":
                    var ability = employeeContext.Abilities.FirstOrDefault(a => a.Id == recordId);
                    return ability?.EmployeeId ?? 0;

                case "Character":
                    var character = employeeContext.Characters.FirstOrDefault(c => c.Id == recordId);
                    return character?.EmployeeId ?? 0;

                case "PunishmentEncourage":
                    var punishmentEncourage = employeeContext.PunishmentEncourages.FirstOrDefault(p => p.Id == recordId);
                    return punishmentEncourage?.EmployeeId ?? 0;

                case "AbsenceRecord":
                    var absenceRecord = employeeContext.AbsenceRecords.FirstOrDefault(a => a.Id == recordId);
                    return absenceRecord?.EmployeeId ?? 0;

                case "Attendance":
                    var attendance = employeeContext.Attendances.FirstOrDefault(a => a.Id == recordId);
                    return attendance?.EmployeeId ?? 0;

                case "Experience":
                    var experience = employeeContext.Experiences.FirstOrDefault(e => e.Id == recordId);
                    return experience?.EmployeeId ?? 0;

                case "Appearance":
                
                    var Appearance = employeeContext.Appearances.FirstOrDefault(a => a.Id == recordId);
                    return Appearance?.EmployeeId ?? 0;

                case "EmployeeRequest":
                    var employeeRequest = employeeContext.EmployeeRequests.FirstOrDefault(r => r.Id == recordId);
                    return employeeRequest?.EmployeeId ?? 0;

                default:
                    // Controller not recognized - return 0
                    return 0;
            }
        }

        private static long ResolveEmployeeIdFromObject(object instance, EmployeeContext employeeContext)
        {
            if (instance == null) return 0;

            var type = instance.GetType();

            // Try EmployeeId on DTO
            var empProp = type.GetProperty("EmployeeId");
            if (empProp != null)
            {
                var value = empProp.GetValue(instance);
                var empId = CoerceToLong(value);
                if (empId > 0) return empId;
            }

            // Try InsuranceId indirection
            var insuranceIdProp = type.GetProperty("InsuranceId");
            if (insuranceIdProp != null)
            {
                var iidVal = insuranceIdProp.GetValue(instance);
                long insuranceId = CoerceToLong(iidVal);

                if (insuranceId > 0)
                {
                    var parent = employeeContext.Insurances.FirstOrDefault(i => i.Id == insuranceId);
                    if (parent != null && parent.EmployeeId > 0)
                    {
                        return parent.EmployeeId;
                    }
                }
            }

            return 0;
        }

        private static long CoerceToLong(object value)
        {
            if (value == null) return 0;
            if (value is long) return (long)value;
            if (value is int) return (int)value;
            if (value is short) return (short)value;
            if (value is byte) return (byte)value;
            if (value is long?) return ((long?)value) ?? 0;
            if (value is int?) return ((int?)value) ?? 0;
            if (value is short?) return ((short?)value) ?? 0;
            if (value is byte?) return ((byte?)value) ?? 0;
            if (value is string s && long.TryParse(s, out var p)) return p;
            return 0;
        }
    }
}


