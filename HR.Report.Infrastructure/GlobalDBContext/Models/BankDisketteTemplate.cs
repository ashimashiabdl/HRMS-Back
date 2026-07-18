using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Diskette_Template", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankId", Name = "IX_Bank_Diskette_Template_BankId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Bank_Diskette_Template_OrganisationChartId")]
public partial class BankDisketteTemplate
{
    [Key]
    public long Id { get; set; }

    [StringLength(1024)]
    public string? FileName { get; set; }

    [StringLength(64)]
    public string? FileExtension { get; set; }

    public string? FileHeader { get; set; }

    public string? FileEnd { get; set; }

    public bool HasLineStartCharacter { get; set; }

    [StringLength(256)]
    public string? LineStartCharacter { get; set; }

    public bool HasLineEndCharacter { get; set; }

    [StringLength(256)]
    public string? LineEndCharacter { get; set; }

    public bool HasLineDelimiterCharacter { get; set; }

    [StringLength(256)]
    public string? LineDelimiterCharacter { get; set; }

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

    public long OrganisationChartId { get; set; }

    public long BankId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankId")]
    [InverseProperty("BankDisketteTemplates")]
    public virtual Bank Bank { get; set; } = null!;

    [InverseProperty("BankDisketteTemplate")]
    public virtual ICollection<BankDisketteGroupAndFile> BankDisketteGroupAndFiles { get; set; } = new List<BankDisketteGroupAndFile>();

    [InverseProperty("BankDisketteTemplate")]
    public virtual ICollection<BankDisketteTemplateRow> BankDisketteTemplateRows { get; set; } = new List<BankDisketteTemplateRow>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("BankDisketteTemplates")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
