using HR.SharedKernel.Attribute;
using HR.SharedKernel.Import;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Import_Profile", Schema = "bas")]
public class ImportProfile : HR.SharedKernel.Data.BaseEntity
{
    [StringLength(256)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string TargetEntityName { get; set; } = "";

    [StringLength(64)]
    public string? TargetSchema { get; set; }

    [StringLength(64)]
    public string? ModuleKey { get; set; }

    public ImportHandlerType HandlerType { get; set; } = ImportHandlerType.Simple;

    [StringLength(128)]
    public string? CustomHandlerKey { get; set; }

    public bool RequiresEmployeeLookup { get; set; }

    [StringLength(128)]
    public string AllowedExtensions { get; set; } = ".xlsx,.csv";

    public int MaxRowCount { get; set; } = 10000;

    public bool HasHeaderRow { get; set; } = true;

    [StringLength(128)]
    public string? PermissionKey { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(512)]
    public string? Description { get; set; }

    public virtual ICollection<ImportProfileField> Fields { get; set; } = new List<ImportProfileField>();

    public virtual ICollection<ImportProfileContextField> ContextFields { get; set; } = new List<ImportProfileContextField>();
}
