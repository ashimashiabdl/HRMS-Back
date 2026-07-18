using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Bank_Diskette_Template_Row", Schema = "Payroll")]
public class BankDisketteTemplateRow : BaseEntity
{
    [ForeignKey("BankDisketteTemplate")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long BankDisketteTemplateId { get; set; }
    public virtual BankDisketteTemplate? BankDisketteTemplate { get; set; }
    [ForeignKey("DisketteItemType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long DisketteItemTypeId { get; set; }
    public virtual BaseTableValue? DisketteItemType { get; set; }
    public int Length { get; set; }
    [StringLength(8)]
    public string? PadLeftCharacter { get; set; }
    /// <summary>
    /// متن ثابت
    /// </summary>
    public string? StaticText { get; set; }
    /// <summary>
    /// شناسه پرداخت
    /// </summary>
    public string? PaymentId { get; set; }
    public int Priority { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
