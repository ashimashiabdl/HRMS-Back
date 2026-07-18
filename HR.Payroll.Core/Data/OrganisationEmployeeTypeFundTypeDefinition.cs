using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Organisation_Employee_Type_FundType_Definition", Schema = "Payroll")]
public class OrganisationEmployeeTypeFundTypeDefinition : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }
    /// <summary>
    /// درصد کارمند
    /// </summary>
    public int EmployeePercent { get; set; }

    [ForeignKey("EmployeeWageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeWageItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual WageItem? EmployeeWageItem { get; set; }


    [ForeignKey("EmployerWageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployerWageItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual WageItem? EmployerWageItem { get; set; }


    [ForeignKey("EmployeeFormula")]
    public long? EmployeeFormulaId { get; set; }
    public virtual OrganisationFormula? EmployeeFormula { get; set; }


    [ForeignKey("EmployerFormula")]
    public long? EmployerFormulaId { get; set; }
    public virtual OrganisationFormula? EmployerFormula { get; set; }

    [ForeignKey("FundType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long FundTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual FundType? FundType { get; set; }
}
