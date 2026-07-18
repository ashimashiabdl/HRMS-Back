using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Formula_Table_Value", Schema = "For")]
[Microsoft.EntityFrameworkCore.Index("FormulaTableId", Name = "IX_Formula_Table_Value_FormulaTableId")]
[Microsoft.EntityFrameworkCore.Index("FormulaTableId", "Year", "DiscreteValue", Name = "IX_Formula_Table_Value_FormulaTableId_Year_DiscreteValue")]
public partial class FormulaTableValue
{
    [Key]
    public long Id { get; set; }

    public long FormulaTableId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? FromValue1 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ToValue1 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? FromValue2 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ToValue2 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? FromValue3 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ToValue3 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscreteValue { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Resultvalue { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    public int? Year { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("FormulaTableId")]
    [InverseProperty("FormulaTableValues")]
    public virtual FormulaTable FormulaTable { get; set; } = null!;
}
