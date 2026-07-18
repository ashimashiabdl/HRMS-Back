using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Initial_Course_Qualification", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("CourseLevelId", Name = "IX_OrganizationJob_Initial_Course_Qualification_CourseLevelId")]
[Microsoft.EntityFrameworkCore.Index("CourseTypeId", Name = "IX_OrganizationJob_Initial_Course_Qualification_CourseTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Initial_Course_Qualification_OrganizationJobId")]
public partial class OrganizationJobInitialCourseQualification
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long CourseTypeId { get; set; }

    public long CourseLevelId { get; set; }

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

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("OrganizationJobInitialCourseQualifications")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
