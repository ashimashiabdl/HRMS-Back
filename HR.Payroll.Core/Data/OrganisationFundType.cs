using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;

using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Organisation_Fund_Type", Schema = "Payroll")]
public class OrganisationFundType : BaseEntity , IOrganisationChartId , IignoreDateRangeValidation
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("FundType")]
    public long FundTypeId { get; set; }
    public virtual FundType? FundType { get; set; }
}
