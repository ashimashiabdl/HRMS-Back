using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Formula_Table", Schema = "For")]
[Microsoft.EntityFrameworkCore.Index("Description", "OrganisationChartId", Name = "IX_Formula_Table_Description_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Formula_Table_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("TableTypeId", Name = "IX_Formula_Table_TableTypeId")]
public partial class FormulaTable
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long TableTypeId { get; set; }

    public int Rank { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

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
    public string? RelatedContextField { get; set; }

    public bool SetZeroIfNotFound { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("FormulaTable")]
    public virtual ICollection<FormulaTableValue> FormulaTableValues { get; set; } = new List<FormulaTableValue>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("FormulaTables")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
