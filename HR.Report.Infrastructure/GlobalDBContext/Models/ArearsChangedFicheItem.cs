using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Arears_Changed_FicheItem", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("ArearFicheId", Name = "IX_Arears_Changed_FicheItem_ArearFicheId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Arears_Changed_FicheItem_WageItemId")]
public partial class ArearsChangedFicheItem
{
    [Key]
    public long Id { get; set; }

    public long WageItemId { get; set; }

    /// <summary>
    /// مبلغ جدید
    /// </summary>
    public long CurrentAmount { get; set; }

    /// <summary>
    /// مبلغ فیش قبلی
    /// </summary>
    public long LastAmount { get; set; }

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

    public long ArearFicheId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("WageItemId")]
    [InverseProperty("ArearsChangedFicheItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
