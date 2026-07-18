using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Family", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("BirthPlaceId", Name = "IX_Family_BirthPlaceId")]
[Microsoft.EntityFrameworkCore.Index("EducationFieldId", Name = "IX_Family_EducationFieldId")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Family_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EducationOrientationId", Name = "IX_Family_EducationOrientationId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Family_OrganisationChartId")]
public partial class Family
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    public long? GenderTypeId { get; set; }

    public long? RelationshipTypeId { get; set; }

    public long? DependentTypeId { get; set; }

    public long? SpecialStatusId { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(10)]
    public string? NationalNo { get; set; }

    [StringLength(100)]
    public string? FatherName { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? EffectivePercent { get; set; }

    public float? MaintenanceCost { get; set; }

    public DateOnly? MaintenanceEndDate { get; set; }

    public long? UseCompeleteInsuranceTypeId { get; set; }

    [StringLength(30)]
    public string? AccountNumber { get; set; }

    public long? InsuranceTypeId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public long? LifeAccidentInsuranceTypeId { get; set; }

    [StringLength(10)]
    public string? IdentityNo { get; set; }

    public long? MaritalStatusId { get; set; }

    public bool? IsImperfective { get; set; }

    public long? BirthPlaceId { get; set; }

    public bool? IsLast { get; set; }

    public bool IsVerify { get; set; }

    public bool? UsedinOrder { get; set; }

    public bool? IsPremierStudent { get; set; }

    public bool? IsDependent { get; set; }

    public DateOnly? ImperfectiveStartDate { get; set; }

    public bool? IsCoveredInsurance { get; set; }

    public DateOnly? MarriageDate { get; set; }

    public int? DisabilityPercent { get; set; }

    public DateOnly? DisabilityEndDate { get; set; }

    public long? DisabilityTypeId { get; set; }

    public bool? HasCertification { get; set; }

    public DateOnly? MarriageEndDate { get; set; }

    [StringLength(128)]
    public string? InsuranceNumber { get; set; }

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

    public long? CompeleteInsuranceTypeId { get; set; }

    public long? EducationFieldId { get; set; }

    public long? EducationGradeId { get; set; }

    public long? EducationOrientationId { get; set; }

    public long? RemedialInsuranceTypeId { get; set; }

    public long? SpecialDiseaseLevelTypeId { get; set; }

    public long? JobId { get; set; }

    public long? SponsorshipTypeId { get; set; }

    public long? UseLifeAccidentInsuranceTypeId { get; set; }

    public bool? IsHekmat { get; set; }

    public bool? IsCashBenefits { get; set; }

    public bool? IsWelfareServices { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BirthPlaceId")]
    [InverseProperty("Families")]
    public virtual Place? BirthPlace { get; set; }

    [ForeignKey("EducationFieldId")]
    [InverseProperty("Families")]
    public virtual EducationField? EducationField { get; set; }

    [ForeignKey("EducationGradeId")]
    [InverseProperty("Families")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EducationOrientationId")]
    [InverseProperty("Families")]
    public virtual EducationOrientation? EducationOrientation { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Families")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Families")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
