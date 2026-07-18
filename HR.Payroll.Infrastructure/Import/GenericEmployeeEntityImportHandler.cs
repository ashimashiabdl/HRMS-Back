using System.Globalization;
using System.Reflection;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.Employee.Core.Entities;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

public class GenericEmployeeEntityImportHandler(
    IUnitOfWork<EmployeeContext> employeeUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private static readonly HashSet<string> ReservedRowKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "NationalNo", "EmployeeId", "EmployeeFullName", "MemberNationalNo"
    };

    public bool CanHandle(string targetEntityName) => EmployeeImportEntityRegistry.IsSupported(targetEntityName);

    public async Task ValidateAndResolveRowsAsync(BaseInfoContext context, ImportProfile profile, List<ImportTempRow> tempRows)
    {
        var (mode, importCtx, organId) = await EmployeeLinkedImportHelper.LoadBatchImportContextAsync(context, contextService, tempRows);
        if (importCtx == null || organId <= 0)
        {
            EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, "اطلاعات تکمیلی Import نامعتبر است.");
            return;
        }

        if (mode == ImportContextMode.BatchContext)
        {
            var contextError = ImportContextRowHelper.ValidateRequiredBatchContextFields(profile, importCtx);
            if (contextError != null)
            {
                EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, contextError);
                return;
            }
        }

        await EmployeeLinkedImportHelper.ResolveEmployeeLinkedRowsAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var entityType = EmployeeImportEntityRegistry.GetEntityType(batch.ImportProfile?.TargetEntityName ?? "")
            ?? throw new InvalidOperationException("Employee entity type not found for finalize.");

        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null || contextService.ReadOrganisationChartId(importCtx) <= 0)
            throw new InvalidOperationException($"ContextJson برای {entityType.Name} نامعتبر است.");

        var organisationChartId = contextService.ReadOrganisationChartId(importCtx);
        var contextFields = contextService.GetActiveContextFields(batch.ImportProfile!);
        var profileFieldNames = batch.ImportProfile!.Fields
            .Where(f => !f.IsDeleted)
            .Select(f => f.TargetPropertyName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var validRows = await context.ImportTempRows
            .Where(r => r.ImportBatchId == batch.Id
                        && !r.IsDeleted
                        && r.ValidationStatus == ImportValidationStatus.Valid
                        && r.MainRecordId == null)
            .ToListAsync();

        if (validRows.Count == 0)
            return 0;

        var employeeCtx = employeeUnitOfWork.Context;
        var now = DateTime.Now;
        var inserted = 0;

        foreach (var row in validRows)
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            if (!long.TryParse(resolved.GetValueOrDefault("EmployeeId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
                || employeeId <= 0)
            {
                MarkRowError(row, "شناسه کارمند نامعتبر است.");
                continue;
            }

            var entity = Activator.CreateInstance(entityType) as BaseEntity
                ?? throw new InvalidOperationException($"Cannot create entity instance for {entityType.Name}.");

            entity.title = "";
            entity.CreateDate = now;
            entity.StartDate = now;
            entity.IsDeleted = false;
            entity.IPAddress = ipAddress;

            if (!TrySetProperty(entityType, entity, "EmployeeId", employeeId, out var employeeError))
            {
                MarkRowError(row, employeeError);
                continue;
            }

            var rowContext = ImportContextRowHelper.MergeForRow(batch, batch.ImportProfile!, resolved, contextService);

            if (HasWritableProperty(entityType, "OrganisationChartId"))
            {
                if (!TrySetProperty(entityType, entity, "OrganisationChartId", organisationChartId, out var organError))
                {
                    MarkRowError(row, organError);
                    continue;
                }
            }

            foreach (var ctxField in contextFields)
            {
                rowContext.TryGetValue(ctxField.TargetPropertyName, out var raw);
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (!TrySetPropertyFromString(entityType, entity, ctxField.TargetPropertyName, raw, ctxField.DataType, out var ctxApplyError))
                {
                    MarkRowError(row, ctxApplyError);
                    goto NextRow;
                }
            }

            foreach (var (propertyName, rawValue) in resolved)
            {
                if (ReservedRowKeys.Contains(propertyName) || string.IsNullOrWhiteSpace(rawValue))
                    continue;

                if (profileFieldNames.Count > 0 && !profileFieldNames.Contains(propertyName))
                    continue;

                var profileField = batch.ImportProfile!.Fields.FirstOrDefault(f =>
                    !f.IsDeleted && string.Equals(f.TargetPropertyName, propertyName, StringComparison.OrdinalIgnoreCase));

                if (!TrySetPropertyFromString(entityType, entity, propertyName, rawValue, profileField?.DataType, out var rowApplyError))
                {
                    MarkRowError(row, rowApplyError);
                    goto NextRow;
                }
            }

            if (entityType.Name.Equals(nameof(Family), StringComparison.OrdinalIgnoreCase)
                && resolved.TryGetValue("MemberNationalNo", out var memberNationalNo)
                && !string.IsNullOrWhiteSpace(memberNationalNo)
                && !TrySetPropertyFromString(entityType, entity, "NationalNo", memberNationalNo, "Text", out var familyError))
            {
                MarkRowError(row, familyError);
                goto NextRow;
            }

            await ApplyEntitySpecificLogicBeforeSaveAsync(employeeCtx, entityType.Name, entity, resolved);

            ImportEntityReflectionHelper.AddEntity(employeeCtx, entityType, entity);
            await employeeCtx.SaveChangesAsync();

            row.MainRecordId = entity.Id;
            row.FinalizedAt = now;
            inserted++;

            NextRow: ;
        }

        await context.SaveChangesAsync();
        return inserted;
    }

    public async Task<OperationResult?> RollbackFinalizedBatchAsync(BaseInfoContext context, ImportBatch batch)
    {
        if (batch.Status != ImportBatchStatus.Completed)
            return null;

        var entityType = EmployeeImportEntityRegistry.GetEntityType(batch.ImportProfile?.TargetEntityName ?? "");
        if (entityType == null)
            return null;

        var mainIds = await context.ImportTempRows.AsNoTracking()
            .Where(r => r.ImportBatchId == batch.Id && !r.IsDeleted && r.MainRecordId != null)
            .Select(r => r.MainRecordId!.Value)
            .Distinct()
            .ToListAsync();

        if (mainIds.Count == 0)
            return null;

        var employeeCtx = employeeUnitOfWork.Context;
        var entities = await ImportEntityReflectionHelper.LoadEntitiesByIdsAsync(employeeCtx, entityType, mainIds);
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await employeeCtx.SaveChangesAsync();
        return null;
    }

    private static async Task ApplyEntitySpecificLogicBeforeSaveAsync(
        EmployeeContext employeeCtx,
        string entityName,
        BaseEntity entity,
        Dictionary<string, string?> resolved)
    {
        if (!entityName.Equals(nameof(BankAccount), StringComparison.OrdinalIgnoreCase)
            || entity is not BankAccount bankAccount
            || !bankAccount.Status)
            return;

        var others = await employeeCtx.BankAccounts
            .Where(x => x.EmployeeId == bankAccount.EmployeeId && !x.IsDeleted && x.Status)
            .ToListAsync();
        foreach (var item in others)
            item.Status = false;
    }

    private static bool HasWritableProperty(Type entityType, string propertyName)
    {
        var prop = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return prop is { CanWrite: true };
    }

    private static bool TrySetProperty(Type entityType, object entity, string propertyName, object value, out string error)
    {
        error = "";
        var prop = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null || !prop.CanWrite)
            return true;

        try
        {
            var converted = ConvertToPropertyType(prop.PropertyType, value);
            prop.SetValue(entity, converted);
            return true;
        }
        catch
        {
            error = $"مقدار نامعتبر برای '{propertyName}'.";
            return false;
        }
    }

    private static bool TrySetPropertyFromString(
        Type entityType,
        object entity,
        string propertyName,
        string rawValue,
        string? dataType,
        out string error)
    {
        error = "";
        var prop = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null || !prop.CanWrite)
            return true;

        try
        {
            object? converted = prop.PropertyType switch
            {
                _ when prop.PropertyType == typeof(string) => ExcelImportParser.NormalizeText(rawValue),
                _ when prop.PropertyType == typeof(bool) => EmployeeLinkedImportHelper.TryParseBool(rawValue),
                _ when prop.PropertyType == typeof(bool?) => EmployeeLinkedImportHelper.TryParseBool(rawValue),
                _ when prop.PropertyType == typeof(DateTime) => ImportDateParser.ParseRequired(rawValue),
                _ when prop.PropertyType == typeof(DateTime?) => ImportDateParser.ParseNullable(rawValue),
                _ => ParseNumericOrString(prop.PropertyType, rawValue, dataType)
            };

            prop.SetValue(entity, converted);
            return true;
        }
        catch
        {
            error = $"مقدار نامعتبر برای '{propertyName}'.";
            return false;
        }
    }

    private static object? ParseNumericOrString(Type propertyType, string rawValue, string? dataType)
    {
        var normalized = ExcelImportParser.NormalizeDigitsToEnglish(rawValue).Trim();
        var typeHint = (dataType ?? "").Trim();

        if (propertyType == typeof(int) || propertyType == typeof(int?)
            || string.Equals(typeHint, "Int", StringComparison.OrdinalIgnoreCase))
            return int.Parse(normalized, CultureInfo.InvariantCulture);

        if (propertyType == typeof(long) || propertyType == typeof(long?)
            || string.Equals(typeHint, "Long", StringComparison.OrdinalIgnoreCase))
            return long.Parse(normalized, CultureInfo.InvariantCulture);

        if (propertyType == typeof(decimal) || propertyType == typeof(decimal?)
            || string.Equals(typeHint, "Decimal", StringComparison.OrdinalIgnoreCase))
            return decimal.Parse(normalized.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture);

        if (propertyType == typeof(double) || propertyType == typeof(double?)
            || string.Equals(typeHint, "Double", StringComparison.OrdinalIgnoreCase))
            return double.Parse(normalized.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture);

        return Convert.ChangeType(normalized, Nullable.GetUnderlyingType(propertyType) ?? propertyType, CultureInfo.InvariantCulture);
    }

    private static object? ConvertToPropertyType(Type propertyType, object value)
    {
        var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (value.GetType() == targetType)
            return value;
        return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }

    private static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { message });
    }
}
