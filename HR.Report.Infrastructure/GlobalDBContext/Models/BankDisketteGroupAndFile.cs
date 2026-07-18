using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Diskette_Group_And_File", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteId", Name = "IX_Bank_Diskette_Group_And_File_BankDisketteId")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteTemplateId", Name = "IX_Bank_Diskette_Group_And_File_BankDisketteTemplateId")]
public partial class BankDisketteGroupAndFile
{
    [Key]
    public long Id { get; set; }

    public long BankDisketteId { get; set; }

    public long BankDisketteTemplateId { get; set; }

    public string? Content { get; set; }

    [StringLength(64)]
    public string? Extension { get; set; }

    [StringLength(64)]
    public string? FileName { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankDisketteId")]
    [InverseProperty("BankDisketteGroupAndFiles")]
    public virtual BankDiskette BankDiskette { get; set; } = null!;

    [InverseProperty("BankDisketteGroupAndFile")]
    public virtual ICollection<BankDisketteItem> BankDisketteItems { get; set; } = new List<BankDisketteItem>();

    [ForeignKey("BankDisketteTemplateId")]
    [InverseProperty("BankDisketteGroupAndFiles")]
    public virtual BankDisketteTemplate BankDisketteTemplate { get; set; } = null!;
}
