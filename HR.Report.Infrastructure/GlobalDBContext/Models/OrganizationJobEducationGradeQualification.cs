using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Education_Grade_Qualification", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_OrganizationJob_Education_Grade_Qualification_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Education_Grade_Qualification_OrganizationJobId")]
public partial class OrganizationJobEducationGradeQualification
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long EducationGradeId { get; set; }

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

    [ForeignKey("EducationGradeId")]
    [InverseProperty("OrganizationJobEducationGradeQualifications")]
    public virtual EducationGrade EducationGrade { get; set; } = null!;

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("OrganizationJobEducationGradeQualifications")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
