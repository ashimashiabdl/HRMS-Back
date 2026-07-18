using HR.SharedKernel.Service;

namespace HR.SharedKernel.Import;

public enum ImportHandlerType
{
    Simple = 1,
    EmployeeLinked = 2,
    Custom = 3
}

public enum ImportBatchStatus
{
    Draft = 0,
    Parsed = 1,
    PreviewReady = 2,
    Finalizing = 3,
    Completed = 4,
    Failed = 5,
    Cancelled = 6
}

public enum ImportValidationStatus
{
    Valid = 1,
    Warning = 2,
    Error = 3
}

public enum FkLookupType
{
    None = 0,
    ContextForm = 1,
    NaturalKey = 2,
    NationalNo = 3,
    ComboKeyValue = 4,
    OrganScoped = 5
}

public enum ImportContextControlType
{
    Text = 1,
    Number = 2,
    Date = 3,
    Combo = 4
}

/// <summary>
/// How Import supplementary (context) values are supplied.
/// BatchContext: wizard form applies same values to all rows.
/// RowExcelKeys: FK ids are columns in Excel (reference guide shown in UI).
/// </summary>
public enum ImportContextMode
{
    BatchContext = 1,
    RowExcelKeys = 2
}

/// <summary>
/// Validates organ-scoped FK values for Import context/field metadata (module-specific implementations).
/// </summary>
public interface IImportOrganScopedFkValidator : IScopedServices
{
    bool CanValidate(string? fkReferenceEntity, string? fkReferenceSchema = null);

    Task<string?> ValidateAsync(
        string fkReferenceEntity,
        long id,
        long organisationChartId,
        string? fkReferenceSchema = null);
}

/// <summary>Batch context for EmployeeDeduction employee-linked import (Phase 2).</summary>
public class EmployeeDeductionImportContext
{
    public long OrganisationChartId { get; set; }
    public long DeductionTypeId { get; set; }
    public long StartDeductPaymentPeriodId { get; set; }
    public DateTime PaymentDate { get; set; }
}
