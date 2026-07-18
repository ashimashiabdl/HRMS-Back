using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Education_Grade", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Education_Grade_title", IsUnique = true)]
public partial class EducationGrade
{
    [Key]
    public long Id { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    public int Order { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column("OldID")]
    public int OldId { get; set; }

    [StringLength(1)]
    public string? TaxCode { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("EducationGrade")]
    public virtual ICollection<Captivity> Captivities { get; set; } = new List<Captivity>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<Education> EducationEducationGrades { get; set; } = new List<Education>();

    [InverseProperty("EffectiveEducationGrade")]
    public virtual ICollection<Education> EducationEffectiveEducationGrades { get; set; } = new List<Education>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<Family> Families { get; set; } = new List<Family>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<InterdictOrder> InterdictOrderEducationGrades { get; set; } = new List<InterdictOrder>();

    [InverseProperty("EffectiveEducationGrade")]
    public virtual ICollection<InterdictOrder> InterdictOrderEffectiveEducationGrades { get; set; } = new List<InterdictOrder>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<MilitaryService> MilitaryServices { get; set; } = new List<MilitaryService>();

    [InverseProperty("MaxEducationGrade")]
    public virtual ICollection<OrganisationJob> OrganisationJobs { get; set; } = new List<OrganisationJob>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<OrganizationJobEducationGradeQualification> OrganizationJobEducationGradeQualifications { get; set; } = new List<OrganizationJobEducationGradeQualification>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<War> Wars { get; set; } = new List<War>();

    [InverseProperty("EducationGrade")]
    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
