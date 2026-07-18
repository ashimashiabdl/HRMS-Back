using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Loan_Type", Schema = "Payroll")]
public class LoanType : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [StringLength(128)]
    public string? Code { get; set; }
    [StringLength(512)]
    public string? Comment { get; set; }
    public bool? IsActive { get; set; }
    public long? MaxAmount { get; set; }
    
    [StringLength(128)]
    public string? LoanDocNo { get; set; }
    [StringLength(128)]
    public string? LoanDocDesc { get; set; }
    
    public bool? IsTradeBalance { get; set; }

    [ForeignKey("WageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WageItemId { get; set; }
    public virtual WageItem? WageItem { get; set; }

    [ForeignKey("SettlementItem")]
    public long? SettlementItemId { get; set; }
    public virtual SettlementItem? SettlementItem { get; set; }
}
