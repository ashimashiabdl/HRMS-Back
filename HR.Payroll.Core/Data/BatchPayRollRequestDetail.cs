using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;

using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Batch_PayRoll_Request_Detail", Schema = "Payroll")]
public class BatchPayRollRequestDetail : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("BatchPayRollRequest")]
    public long BatchPayRollRequestId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long? FicheId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? FinalMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DoDatetime { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? LastTryDateTime { get; set; }

    public double RunTimeinMilliseconds { get; set; }


    // [ForeignKey("BankDisketteItem")]
    public long? BankDisketteItemId { get; set; }
    // public virtual BankDisketteItem? BankDisketteItem { get; set; }


    // [ForeignKey("InsuranceDisketteItem")]
    public long? InsuranceDisketteItemId { get; set; }
    // public virtual InsuranceDisketteItem? InsuranceDisketteItem { get; set; }
    public double Value { get; set; }
    public string? ISMainTax { get; set; }

    [NotMapped]
    private new string title { get; set; }
}
