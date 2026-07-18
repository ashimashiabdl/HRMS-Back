using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.FormulaEngine.Core.Data;

[Table("Formula_Database_Function_Definition", Schema = "For")]
public class FormulaDatabaseFunctionDefinition : BaseEntity , IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? FuctionType { get; set; }
    public long? FuctionTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(256)]
    public string? EnglishName { get; set; }
    [StringLength(32)]
    public string? Schema { get; set; }
    [StringLength(255)]
    public string? FunctionName { get; set; }
    [StringLength(1024)]
    public string? Help { get; set; }
    public string? ParamsJson { get; set; }
    public string? Body { get; set; }
    public int NumberOfParameters { get; set; }
    public bool IsPublic { get; set; }
}
