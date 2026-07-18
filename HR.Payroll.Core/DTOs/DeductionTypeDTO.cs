using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs;

public class DeductionTypeDTO : BaseDTO
{
    public string? TaxCode { get; set; }
    public bool IsActive { get; set; }
    [StringLength(50)]
    public string? EnglishName { get; set; }
    [StringLength(128)]
    public string? Code { get; set; }
    [StringLength(512)]
    public string? Comment { get; set; }
    public long OrganisationChartId { get; set; }
    public virtual string? OrganisationChart { get; set; }

    public long WageItemId { get; set; }
    public virtual string? WageItem { get; set; }

    public long? SettlementItemId { get; set; }
    public virtual string? SettlementItem { get; set; }
}
