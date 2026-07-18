using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Formula_Definition", Schema = "For")]
public partial class FormulaDefinition
{
    [Key]
    public long Id { get; set; }

    public string FormulaText { get; set; } = null!;

    public int Version { get; set; }

    [StringLength(256)]
    public string? LastChangeReason { get; set; }

    [StringLength(256)]
    public string ErrorMessage { get; set; } = null!;

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

    public int SuccessRunTimeInmilliseconds { get; set; }

    [StringLength(2048)]
    public string Description { get; set; } = null!;

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("FormulaDefinition")]
    public virtual ICollection<FormulaDefinitionHistory> FormulaDefinitionHistories { get; set; } = new List<FormulaDefinitionHistory>();

    [ForeignKey("Id")]
    [InverseProperty("FormulaDefinition")]
    public virtual OrganisationFormula IdNavigation { get; set; } = null!;
}
