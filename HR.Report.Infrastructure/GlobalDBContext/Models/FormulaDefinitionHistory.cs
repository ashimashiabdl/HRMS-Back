using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Formula_Definition_History", Schema = "For")]
[Microsoft.EntityFrameworkCore.Index("FormulaDefinitionId", Name = "IX_Formula_Definition_History_FormulaDefinitionId")]
public partial class FormulaDefinitionHistory
{
    [Key]
    public long Id { get; set; }

    public long FormulaDefinitionId { get; set; }

    public string? PreviousFormulaText { get; set; }

    [Column("IPAddress")]
    public string? Ipaddress { get; set; }

    public long? UserId { get; set; }

    public string? UserFullName { get; set; }

    public DateTime ChangeDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("FormulaDefinitionId")]
    [InverseProperty("FormulaDefinitionHistories")]
    public virtual FormulaDefinition FormulaDefinition { get; set; } = null!;
}
