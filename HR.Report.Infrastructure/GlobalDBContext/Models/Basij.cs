using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Basij", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("BasijPlaceId", Name = "IX_Basij_BasijPlaceId")]
[Microsoft.EntityFrameworkCore.Index("BasijTypeId", Name = "IX_Basij_BasijTypeId")]
[Microsoft.EntityFrameworkCore.Index("ConfirmerOrganId", Name = "IX_Basij_ConfirmerOrganID")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Basij_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Basij_OrganisationChartId")]
public partial class Basij
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? BasijTypeId { get; set; }

    public int? DurationYear { get; set; }

    public int? DurationMonth { get; set; }

    public int? DurationDay { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public DateOnly? LetterDate { get; set; }

    [StringLength(50)]
    public string? LetterNumber { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public bool? IsContinues { get; set; }

    public long? BasijPlaceId { get; set; }

    [Column("ConfirmerOrganID")]
    public long? ConfirmerOrganId { get; set; }

    public bool IsActive { get; set; }

    public bool IsComputeableInHistory { get; set; }

    public int? YearCoefficient { get; set; }

    public int? Year { get; set; }

    public bool IsPercent { get; set; }

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
    [InverseProperty("Basijs")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Basijs")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
