using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs;

public class LoanTypeDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    [StringLength(128)]
    public string? Name { get; set; }
    [StringLength(128)]
    public string? Code { get; set; }
    [StringLength(512)]
    public string? Comment { get; set; }
    public bool? IsActive { get; set; }
    public long? MaxAmount { get; set; }
    public string MaxAmountSep
    {
        get
        {
            if (MaxAmount == null)
            {
                return "0";
            }
            return MaxAmount.Value.ToString("#,##0") + " ريال ";
        }
    }
    [StringLength(128)]
    public string? LoanDocNo { get; set; }
    [StringLength(128)]
    public string? LoanDocDesc { get; set; }
    public bool? IsTradeBalance { get; set; }
    
    public long WageItemId { get; set; }
    public string? WageItem { get; set; }

    public long? SettlementItemId { get; set; }
    public string? SettlementItem { get; set; }
}
