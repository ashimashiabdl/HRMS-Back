using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Captivity", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("CaptivityLocationId", Name = "IX_Captivity_CaptivityLocationId")]
[Microsoft.EntityFrameworkCore.Index("ConfirmerOrganId", Name = "IX_Captivity_ConfirmerOrganID")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Captivity_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Captivity_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Captivity_OrganisationChartId")]
public partial class Captivity
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public long? CaptivityLocationId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateOnly? LetterDate { get; set; }

    [StringLength(50)]
    public string? LetterNumber { get; set; }

    [Column("ConfirmerOrganID")]
    public long? ConfirmerOrganId { get; set; }

    public bool? IsContinues { get; set; }

    public double? SacrificePercent { get; set; }

    public bool IsActive { get; set; }

    [StringLength(50)]
    public string? TrackingCode { get; set; }

    public long? EducationGradeId { get; set; }

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
    [InverseProperty("Captivities")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Captivities")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Captivities")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
