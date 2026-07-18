using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Education", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EducationFieldId", Name = "IX_Education_EducationFieldId")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Education_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EducationOrientationId", Name = "IX_Education_EducationOrientationId")]
[Microsoft.EntityFrameworkCore.Index("EducationPlacesId", Name = "IX_Education_EducationPlacesId")]
[Microsoft.EntityFrameworkCore.Index("EducationStateId", Name = "IX_Education_EducationStateID")]
[Microsoft.EntityFrameworkCore.Index("EffectiveEducationGradeId", Name = "IX_Education_EffectiveEducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Education_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "OrganisationChartId", "IsDefaultEducation", "IsDeleted", "Id", Name = "IX_Education_EmployeeId_OrgChart_Default", IsDescending = new[] { false, false, false, false, true })]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Education_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("UniversityId", Name = "IX_Education_UniversityId")]
[Microsoft.EntityFrameworkCore.Index("UniversityLevelId", Name = "IX_Education_UniversityLevelId")]
[Microsoft.EntityFrameworkCore.Index("UniversityTypeId", Name = "IX_Education_UniversityTypeID")]
public partial class Education
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? EducationGradeId { get; set; }

    public long? EducationGroupId { get; set; }

    public long? EffectiveEducationGradeId { get; set; }

    public long? EducationFieldId { get; set; }

    public long? EducationOrientationId { get; set; }

    [Column("EducationStateID")]
    public long? EducationStateId { get; set; }

    public DateOnly? EducationFromDate { get; set; }

    public DateOnly? EducationToDate { get; set; }

    [StringLength(8)]
    public string? EducationAverage { get; set; }

    public DateOnly? EducationLicensePresentDate { get; set; }

    public DateOnly? EducationLicenseImplDate { get; set; }

    public DateOnly? EducationLicenseExpireDate { get; set; }

    public bool? IsInDutyTime { get; set; }

    public long? EducationPlacesId { get; set; }

    [StringLength(500)]
    public string? Descriptions { get; set; }

    public bool? IsBoursie { get; set; }

    [StringLength(256)]
    public string? ThesisTitle { get; set; }

    public long? UniversityId { get; set; }

    public bool? IsDefaultEducation { get; set; }

    public bool? IsUsedInOrder { get; set; }

    public bool? SetByEmployee { get; set; }

    public long? LicenceTypeId { get; set; }

    [StringLength(128)]
    public string? LicenceNumber { get; set; }

    [StringLength(128)]
    public string? OtherUniversityName { get; set; }

    public long? KindOfGraduationId { get; set; }

    public long? ThesisGradeTypeId { get; set; }

    public long? ThesisGradeValueId { get; set; }

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

    [Column("UniversityTypeID")]
    public long? UniversityTypeId { get; set; }

    public long? UniversityLevelId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EducationFieldId")]
    [InverseProperty("Educations")]
    public virtual EducationField? EducationField { get; set; }

    [ForeignKey("EducationGroupId")]
    public virtual EducationGroup? EducationGroup { get; set; }

    [ForeignKey("EducationGradeId")]
    [InverseProperty("EducationEducationGrades")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EducationOrientationId")]
    [InverseProperty("Educations")]
    public virtual EducationOrientation? EducationOrientation { get; set; }

    [ForeignKey("EducationPlacesId")]
    [InverseProperty("Educations")]
    public virtual Place? EducationPlaces { get; set; }

    [ForeignKey("EffectiveEducationGradeId")]
    [InverseProperty("EducationEffectiveEducationGrades")]
    public virtual EducationGrade? EffectiveEducationGrade { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Educations")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("EmpEdu")]
    public virtual ICollection<InterdictOrder> InterdictOrders { get; set; } = new List<InterdictOrder>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Educations")]
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("UniversityId")]
    [InverseProperty("Educations")]
    public virtual University? University { get; set; }

    [ForeignKey("UniversityLevelId")]
    [InverseProperty("EducationUniversityLevels")]
    public virtual BaseTableValue? UniversityLevel { get; set; }

    [ForeignKey("UniversityTypeId")]
    [InverseProperty("EducationUniversityTypes")]
    public virtual BaseTableValue? UniversityType { get; set; }
}
