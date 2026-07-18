using HR.BaseInfo.Core.Entities;

using HR.SharedKernel.Import;

using HR.SharedKernel.Service;



namespace HR.BaseInfo.infrastructure.Import;



public class ImportHandlerRegistry : IScopedServices

{

    private readonly IReadOnlyList<IImportTargetHandler> _handlers;



    public ImportHandlerRegistry(IEnumerable<IImportTargetHandler> handlers)

    {

        _handlers = handlers.ToList();

    }



    public IImportTargetHandler? GetHandler(ImportProfile profile)

    {

        if (profile.HandlerType == ImportHandlerType.Custom)

        {

            if (!string.IsNullOrWhiteSpace(profile.CustomHandlerKey))

            {

                var byKey = _handlers.FirstOrDefault(h => h.CanHandle(profile.CustomHandlerKey!));

                if (byKey != null)

                    return byKey;

            }



            return _handlers.FirstOrDefault(h => h.CanHandle(profile.TargetEntityName));

        }



        return _handlers.FirstOrDefault(h => h.CanHandle(profile.TargetEntityName));

    }



    public string GetMissingHandlerMessage(ImportProfile profile)

    {

        if (profile.HandlerType == ImportHandlerType.Custom)

        {

            if (string.IsNullOrWhiteSpace(profile.CustomHandlerKey))

                return "پروفایل Custom فاقد CustomHandlerKey است.";



            return $"Handler اختصاصی با کلید '{profile.CustomHandlerKey}' برای موجودیت '{profile.TargetEntityName}' ثبت نشده است.";

        }



        return $"Handler برای موجودیت '{profile.TargetEntityName}' تعریف نشده است.";

    }

}

