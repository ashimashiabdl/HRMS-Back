using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Organisation_EmployeeType_Coefficient_Bonus_WageItem", Schema = "Payroll")]
public class OrganisationEmployeeTypeCoefficientBonusWageItem : BaseEntity , IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }
    [ForeignKey("WageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WageItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual WageItem? WageItem { get; set; }
    [ForeignKey("Coefficient")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long CoefficientId { get; set; }
    public virtual Coefficient? Coefficient { get; set; }
}
