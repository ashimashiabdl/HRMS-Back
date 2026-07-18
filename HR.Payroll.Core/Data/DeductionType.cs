using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Deduction_Type", Schema = "Payroll")]
public class DeductionType : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("WageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WageItemId { get; set; }
    public virtual WageItem? WageItem { get; set; }

    [ForeignKey("SettlementItem")]
    public long? SettlementItemId { get; set; }
    public virtual SettlementItem? SettlementItem { get; set; }
    [StringLength(2)]
    public string? TaxCode { get; set; }
    public bool IsActive { get; set; }
    [StringLength(50)]
    public string? EnglishName { get; set; }
    [StringLength(128)]
    public string? Code { get; set; }
    [StringLength(512)]
    public string? Comment { get; set; }
}
