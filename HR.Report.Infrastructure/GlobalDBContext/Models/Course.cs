using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Course", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("CourseId", Name = "IX_Course_CourseId")]
[Microsoft.EntityFrameworkCore.Index("CourseLicenseId", Name = "IX_Course_CourseLicenseId")]
[Microsoft.EntityFrameworkCore.Index("CourseRegTypeId", Name = "IX_Course_CourseRegTypeId")]
[Microsoft.EntityFrameworkCore.Index("CourseStatusId", Name = "IX_Course_CourseStatusId")]
[Microsoft.EntityFrameworkCore.Index("CourseTypeId", Name = "IX_Course_CourseTypeId")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Course_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Course_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Course_OrganisationChartId")]
public partial class Course
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? CourseStatusId { get; set; }

    public long? CourseLicenseId { get; set; }

    [StringLength(8)]
    public string? CourseMark { get; set; }

    public int? CourseTime { get; set; }

    public long? CourseRegTypeId { get; set; }

    public long? EducationGradeId { get; set; }

    [StringLength(128)]
    public string? CoursepPlace { get; set; }

    public int? CourseSession { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(16)]
    public string? CourseSerial { get; set; }

    public long? CourseTypeId { get; set; }

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

    public long? CourseId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EducationGradeId")]
    [InverseProperty("Courses")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Courses")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Courses")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
