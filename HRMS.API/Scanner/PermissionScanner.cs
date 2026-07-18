using Castle.Components.DictionaryAdapter.Xml;
using HR.Identity.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;
using System.Security;
using HR.Identity.infrastructure.Data;
using HRMS.API.Cache;

namespace HRMS.API.Scanner;

public class PermissionScanner(IdentityContext dbContext, DefaultPermissionRouteSeeder permissionRouteSeeder)
{
    public async Task<bool> InitializeDefaultPermissions()
    {
        await DeletePermissions();
        var permission = GetPermissions();
        var res = dbContext.Permissions.Add(permission);


        // Save changes to the database.
        var rr = dbContext.SaveChanges();
        await permissionRouteSeeder.EnsureAttendanceRoutesAsync();
        return true;
    }


    private Permission GetPermissions()
    {
        var rootNode = new Permission { Name = "Actions", DisplayName = "", Permissions = new List<Permission>() };
        var controllers = Assembly.GetExecutingAssembly()
        .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(AppBaseController).IsAssignableFrom(t))
                .Where(t => t.GetCustomAttributes(typeof(ControllerGroup), inherit: true).Any())
                .ToList();


        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "اطلاعات پایه", Name = "baseInfo" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "اطلاعات کارکنان", Name = "Employee" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "فرمول های سیستم", Name = "FormulaEngine" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "احراز هویت", Name = "IdentityManager" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "احکام", Name = "OrderNameSpace" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "ساختار سازمان", Name = "Organisation" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "حقوق و دستمزد", Name = "PayRoll" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "گزارش ها", Name = "Report" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "تنظیمات سیستم", Name = "SystemSetting" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "گردش کار", Name = "WorkFlow" });
        rootNode.Permissions.Add(new Permission() { Id = Guid.NewGuid(), DisplayName = "حضور و غیاب", Name = "Attendance" });




        foreach (var controller in controllers)
        {

            if (controller.Name.ToLower().StartsWith("order"))
            {

            }


            var ControllerGroup = controller.GetCustomAttribute<ControllerGroup>()?.EnglishKey ?? controller.Name.Replace("Controller", "");

            if (rootNode.Permissions.Any(i => i.Name.ToLower() == ControllerGroup.ToLower()))
            {
                var relatedParent = rootNode.Permissions.Single(i => i.Name.ToLower() == ControllerGroup.ToLower());


                var controllerDisplayName = controller.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? controller.Name.Replace("Controller", "");
                if (controllerDisplayName == "Account")
                { continue; }
                var controllerPermission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = ControllerGroup + "." + controller.Name.Replace("Controller", ""),
                    DisplayName = controllerDisplayName,
                    IsCustomPermission = false,
                    Permissions = new List<Permission>()
                };
                relatedParent.Permissions.Add(controllerPermission);
                var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => m.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true).Length == 0) // Exclude actions with AllowAnonymous
                    .ToList();

                foreach (var action in actions)
                {
                    var CustomKey = action.GetCustomAttribute<CustomAccessKey>();

                    if (CustomKey == null)
                    {
                        continue;
                    }

                    var AccessKey = PermissionKeyDescription.CustomAccessKeyDesc.Where(i => i.Key.ToLower() == $"{controllerPermission.Name}.{CustomKey.AccessKey}".ToLower());

                    if (AccessKey == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (AccessKey.Count() == 1)
                        {
                            var actionPermission = new Permission
                            {
                                Name = $"{controllerPermission.Name}.{CustomKey.AccessKey}",
                                DisplayName = AccessKey.Single().Value,
                                Id = Guid.NewGuid(),
                                IsCustomPermission = false,
                            };
                            if (controllerPermission.Permissions.Any(i => i.Name.ToLower() == $"{controllerPermission.Name}.{CustomKey.AccessKey}".ToLower()))
                            {
                                continue;
                            }
                            controllerPermission.Permissions.Add(actionPermission);
                        }
                        else
                        {

                          //  throw new Exception();
                        }


                    }


                }

            }
            else
            {
                  throw new SecurityException($"ControllerGroup '{ControllerGroup}' not found in root permissions.");
            }
        }

        return rootNode;

    }
    private async Task DeletePermissions()
    {

        var permissionsToBeDeleted = dbContext.Permissions
                                                    .Where(p => !p.IsCustomPermission)
                                                    .IgnoreQueryFilters()
                                                    .ToList();


        if (permissionsToBeDeleted.Count != 0)

            dbContext.Permissions.RemoveRange(entities: permissionsToBeDeleted);
    }

}
