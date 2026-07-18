using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Coefficient", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("CoefficientId", Name = "IX_Organisation_Coefficient_CoefficientId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Coefficient_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("MappedExcelColumnId", Name = "IX_Organisation_Coefficient_MappedExcelColumnId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "CoefficientId", "IsDeleted", Name = "IX_Organisation_Coefficient_PayLoc_Coefficient_Active")]
public partial class OrganisationCoefficient
{
    [Key]
    public long Id { get; set; }

    public long CoefficientId { get; set; }

    public long OrganisationChartId { get; set; }

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

    public long? MappedExcelColumnId { get; set; }

    [ForeignKey("MappedExcelColumnId")]
    [InverseProperty("OrganisationCoefficients")]
    public virtual BaseTableValue? MappedExcelColumn { get; set; }

    [ForeignKey("CoefficientId")]
    [InverseProperty("OrganisationCoefficients")]
    public virtual Coefficient Coefficient { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationCoefficients")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
