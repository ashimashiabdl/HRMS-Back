using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Diskette_Template_Row", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteTemplateId", Name = "IX_Bank_Diskette_Template_Row_BankDisketteTemplateId")]
[Microsoft.EntityFrameworkCore.Index("DisketteItemTypeId", Name = "IX_Bank_Diskette_Template_Row_DisketteItemTypeId")]
public partial class BankDisketteTemplateRow
{
    [Key]
    public long Id { get; set; }

    public int Length { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long BankDisketteTemplateId { get; set; }

    public long DisketteItemTypeId { get; set; }

    [StringLength(8)]
    public string? PadLeftCharacter { get; set; }

    public int Priority { get; set; }

    public string? PaymentId { get; set; }

    public string? StaticText { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankDisketteTemplateId")]
    [InverseProperty("BankDisketteTemplateRows")]
    public virtual BankDisketteTemplate BankDisketteTemplate { get; set; } = null!;
}
