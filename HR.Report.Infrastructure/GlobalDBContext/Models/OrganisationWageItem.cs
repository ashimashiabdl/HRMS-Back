using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_WageItem", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("MappedExcelColumnId", Name = "IX_Organisation_WageItem_MappedExcelColumnId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_WageItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "WageItemId", "IsDeleted", Name = "IX_Organisation_WageItem_PayLoc_WageItem_Active")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Organisation_WageItem_WageItemId")]
public partial class OrganisationWageItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long WageItemId { get; set; }

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
    [InverseProperty("OrganisationWageItems")]
    public virtual BaseTableValue? MappedExcelColumn { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationWageItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("OrganisationWageItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
