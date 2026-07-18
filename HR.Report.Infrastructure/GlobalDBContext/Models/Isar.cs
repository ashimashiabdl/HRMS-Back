using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Isar", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Isar_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Isar_OrganisationChartId")]
public partial class Isar
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? IsartypeId { get; set; }

    public long? ConfirmerOrganId { get; set; }

    public DateOnly? IsarStartDate { get; set; }

    public float? Isarpercent { get; set; }

    public long? IsarLocationId { get; set; }

    public long? IsarEquipmentId { get; set; }

    [StringLength(256)]
    public string? IsarInability { get; set; }

    [StringLength(256)]
    public string? IsarInjuerdOrgan { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    public DateOnly? LetterDate { get; set; }

    [StringLength(50)]
    public string? LetterNumber { get; set; }

    public DateOnly? IsarEndDate { get; set; }

    public int? IsarDurationYear { get; set; }

    public int? IsarDurationMonth { get; set; }

    public int? IsarDurationDay { get; set; }

    public bool? IsContinues { get; set; }

    public bool IsActive { get; set; }

    [StringLength(50)]
    public string? TrackingCode { get; set; }

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

    [ForeignKey("EmployeeId")]
    [InverseProperty("Isars")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Isars")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
