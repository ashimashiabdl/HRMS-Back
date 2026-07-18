using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("War", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_War_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_War_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_War_OrganisationChartId")]
public partial class War
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? WarTypeId { get; set; }

    public long? ConfirmerOrganId { get; set; }

    [StringLength(256)]
    public string? JebheOperations { get; set; }

    public DateOnly? LetterDate { get; set; }

    [StringLength(50)]
    public string? LetterNumber { get; set; }

    public double? PercentAnnualIncrease { get; set; }

    public int? DurationYear { get; set; }

    public int? DurationMonth { get; set; }

    public int? DurationDay { get; set; }

    public DateOnly? WarDateFrom { get; set; }

    public DateOnly? WarDateTo { get; set; }

    public bool? IsContinues { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public bool IsActive { get; set; }

    public long? EducationGradeId { get; set; }

    [StringLength(50)]
    public string? TrackingCode { get; set; }

    [StringLength(256)]
    public string? AcceptableDurationForTaxExemption { get; set; }

    public long? WarLocationId { get; set; }

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

    [ForeignKey("EducationGradeId")]
    [InverseProperty("Wars")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Wars")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Wars")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
