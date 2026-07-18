using System.Globalization;
using System.Reflection;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.Employee.Core.Constants;
using EmployeeEntity = HR.Employee.Core.Entities.Employee;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

/// <summary>
/// Import new rows into emp.Employee (root entity — no existing employee lookup by NationalNo).
/// </summary>
public class EmployeeImportHandler(
    IUnitOfWork<EmployeeContext> employeeUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = "Employee";

    public bool CanHandle(string targetEntityName) =>
        string.Equals(targetEntityName, TargetEntity, StringComparison.OrdinalIgnoreCase);

    public async Task ValidateAndResolveRowsAsync(BaseInfoContext context, ImportProfile profile, List<ImportTempRow> tempRows)
    {
        var (mode, importCtx, organId) = await EmployeeLinkedImportHelper.LoadBatchImportContextAsync(context, contextService, tempRows);
        if (importCtx == null || organId <= 0)
        {
            EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, "سازمان پیش‌فرض مشخص نشده است.");
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

        var fields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, mode);
        var profileFieldNames = profile.Fields.Where(f => !f.IsDeleted).Select(f => f.TargetPropertyName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var nationalNoField = fields.FirstOrDefault(f =>
            string.Equals(f.TargetPropertyName, "NationalNo", StringComparison.OrdinalIgnoreCase));

        var fileNationalCounts = BuildNationalNoCounts(tempRows);
        var employeeCtx = employeeUnitOfWork.Context;
        var existingNationalNos = await LoadExistingNationalNosAsync(employeeCtx, tempRows);
        var existingPersonelCodes = await LoadExistingPersonelCodesAsync(employeeCtx, tempRows);

        foreach (var row in tempRows)
        {
            var parsed = EmployeeLinkedImportHelper.DeserializeDict(row.ParsedDataJson);
            var resolved = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var messages = new List<string>();
            var status = ImportValidationStatus.Valid;

            foreach (var field in fields)
            {
                parsed.TryGetValue(field.TargetPropertyName, out var raw);
                var (value, fieldError) = this.NormalizeField(raw, field);
                resolved[field.TargetPropertyName] = value;
                if (!string.IsNullOrEmpty(fieldError))
                {
                    status = ImportValidationStatus.Error;
                    messages.Add(fieldError);
                }
            }

            if (status != ImportValidationStatus.Error && nationalNoField != null)
            {
                resolved.TryGetValue("NationalNo", out var nationalNo);
                var normalizedNational = ImportEmployeeLookupService.NormalizeNationalNo(nationalNo);
                resolved["NationalNo"] = normalizedNational;

                if (string.IsNullOrWhiteSpace(normalizedNational))
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کد ملی الزامی است.");
                }
                else if (normalizedNational.Length != 10)
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کد ملی باید ۱۰ رقم باشد.");
                }
                else if (fileNationalCounts.GetValueOrDefault(normalizedNational) > 1)
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کد ملی در فایل تکراری است.");
                }
                else if (existingNationalNos.Contains(normalizedNational))
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کارمندی با این کد ملی از قبل در سیستم وجود دارد.");
                }
                else if (fieldResolver.ResolveEmployeeId(normalizedNational) > 0)
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کارمندی با این کد ملی از قبل در سیستم وجود دارد.");
                }
            }

            if (status != ImportValidationStatus.Error)
            {
                resolved.TryGetValue("PersonelCode", out var personelCode);
                if (!string.IsNullOrWhiteSpace(personelCode))
                {
                    var normalizedCode = personelCode.Trim();
                    if (existingPersonelCodes.Contains(normalizedCode))
                    {
                        status = ImportValidationStatus.Error;
                        messages.Add("کد پرسنلی در سیستم از قبل وجود دارد.");
                    }
                }
            }

            if (status != ImportValidationStatus.Error
                && mode == ImportContextMode.RowExcelKeys
                && contextService.GetActiveContextFields(profile).Count > 0)
            {
                var contextErrors = await contextService.ValidateRowContextFieldsAsync(profile, resolved, organId);
                if (contextErrors.Count > 0)
                {
                    status = ImportValidationStatus.Error;
                    messages.AddRange(contextErrors);
                }
            }

            row.ResolvedDataJson = JsonSerializer.Serialize(resolved);
            row.ValidationStatus = status;
            row.ValidationMessagesJson = messages.Count > 0 ? JsonSerializer.Serialize(messages) : null;

            resolved.TryGetValue("FirstName", out var firstName);
            resolved.TryGetValue("LastName", out var lastName);
            var fullName = $"{firstName} {lastName}".Trim();
            row.title = !string.IsNullOrWhiteSpace(fullName) ? fullName : resolved.GetValueOrDefault("NationalNo") ?? $"ردیف {row.RowNumber}";
        }
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null || contextService.ReadOrganisationChartId(importCtx) <= 0)
            throw new InvalidOperationException("ContextJson برای Employee نامعتبر است.");

        var defaultOrganId = contextService.ReadOrganisationChartId(importCtx);
        var profile = batch.ImportProfile ?? throw new InvalidOperationException("Import profile not loaded.");
        var contextFields = contextService.GetActiveContextFields(profile);
        var profileFieldNames = profile.Fields.Where(f => !f.IsDeleted).Select(f => f.TargetPropertyName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var validRows = await context.ImportTempRows
            .Where(r => r.ImportBatchId == batch.Id
                        && !r.IsDeleted
                        && r.ValidationStatus == ImportValidationStatus.Valid
                        && r.MainRecordId == null)
            .ToListAsync();

        if (validRows.Count == 0)
            return 0;

        var employeeCtx = employeeUnitOfWork.Context;
        var entityType = typeof(EmployeeEntity);
        var now = DateTime.Now;
        var inserted = 0;

        foreach (var row in validRows)
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            var rowContext = ImportContextRowHelper.MergeForRow(batch, profile, resolved, contextService);

            var baseOrganisationId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "BaseOrganisationId");
            if (baseOrganisationId <= 0)
                baseOrganisationId = defaultOrganId;

            var entity = new EmployeeEntity
            {
                title = "",
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                IPAddress = ipAddress ?? string.Empty,
                BaseOrganisationId = baseOrganisationId,
                IsActive = ImportContextRowHelper.TryParseBool(resolved.GetValueOrDefault("IsActive"), defaultValue: true),
            };

            foreach (var ctxField in contextFields)
            {
                rowContext.TryGetValue(ctxField.TargetPropertyName, out var raw);
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (!TrySetPropertyFromString(entityType, entity, ctxField.TargetPropertyName, raw, ctxField.DataType, out var ctxError))
                {
                    MarkRowError(row, ctxError);
                    goto NextRow;
                }
            }

            foreach (var (propertyName, rawValue) in resolved)
            {
                if (string.IsNullOrWhiteSpace(rawValue))
                    continue;
                // When profile fields are loaded, restrict to them; otherwise map any matching entity property.
                if (profileFieldNames.Count > 0 && !profileFieldNames.Contains(propertyName))
                    continue;

                var profileField = profile.Fields.FirstOrDefault(f =>
                    !f.IsDeleted && string.Equals(f.TargetPropertyName, propertyName, StringComparison.OrdinalIgnoreCase));

                if (!TrySetPropertyFromString(entityType, entity, propertyName, rawValue, profileField?.DataType, out var rowError))
                {
                    MarkRowError(row, rowError);
                    goto NextRow;
                }
            }

            EmployeeMartyrRelationRules.ApplyMartyrChildTrackingCodeRule(entity);
            EmployeeAuthorizedForeignerRules.ApplyAuthorizedForeignerRule(entity);

            employeeCtx.Employees.Add(entity);
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

        var mainIds = await context.ImportTempRows.AsNoTracking()
            .Where(r => r.ImportBatchId == batch.Id && !r.IsDeleted && r.MainRecordId != null)
            .Select(r => r.MainRecordId!.Value)
            .Distinct()
            .ToListAsync();

        if (mainIds.Count == 0)
            return null;

        var employeeCtx = employeeUnitOfWork.Context;
        var entities = await employeeCtx.Employees.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await employeeCtx.SaveChangesAsync();
        return null;
    }

    private (string? Value, string? Error) NormalizeField(string? raw, ImportProfileField field) =>
        fieldResolver.NormalizeProfileField(raw, field);

    private static Dictionary<string, int> BuildNationalNoCounts(List<ImportTempRow> tempRows)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in tempRows)
        {
            var parsed = EmployeeLinkedImportHelper.DeserializeDict(row.ParsedDataJson);
            parsed.TryGetValue("NationalNo", out var raw);
            var nationalNo = ImportEmployeeLookupService.NormalizeNationalNo(raw);
            if (string.IsNullOrWhiteSpace(nationalNo))
                continue;
            counts[nationalNo] = counts.GetValueOrDefault(nationalNo) + 1;
        }

        return counts;
    }

    private static async Task<HashSet<string>> LoadExistingNationalNosAsync(EmployeeContext ctx, List<ImportTempRow> tempRows)
    {
        var candidates = tempRows
            .Select(r => EmployeeLinkedImportHelper.DeserializeDict(r.ParsedDataJson).GetValueOrDefault("NationalNo"))
            .Select(ImportEmployeeLookupService.NormalizeNationalNo)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (candidates.Count == 0)
            return [];

        var existing = await ctx.Employees.AsNoTracking()
            .Where(e => !e.IsDeleted && e.NationalNo != null && candidates.Contains(e.NationalNo))
            .Select(e => e.NationalNo!)
            .ToListAsync();

        return existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<HashSet<string>> LoadExistingPersonelCodesAsync(EmployeeContext ctx, List<ImportTempRow> tempRows)
    {
        var candidates = tempRows
            .Select(r => EmployeeLinkedImportHelper.DeserializeDict(r.ParsedDataJson).GetValueOrDefault("PersonelCode")?.Trim())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (candidates.Count == 0)
            return [];

        var existing = await ctx.Employees.AsNoTracking()
            .Where(e => !e.IsDeleted && e.PersonelCode != null && candidates.Contains(e.PersonelCode))
            .Select(e => e.PersonelCode!)
            .ToListAsync();

        return existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
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
                _ when prop.PropertyType == typeof(bool) => ImportContextRowHelper.TryParseBool(rawValue),
                _ when prop.PropertyType == typeof(bool?) => ImportContextRowHelper.TryParseBool(rawValue),
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

        return Convert.ChangeType(normalized, Nullable.GetUnderlyingType(propertyType) ?? propertyType, CultureInfo.InvariantCulture);
    }

    private static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { message });
    }
}
