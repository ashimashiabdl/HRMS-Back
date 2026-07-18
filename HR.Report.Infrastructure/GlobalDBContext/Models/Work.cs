using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Work", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Work_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Work_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", Name = "IX_Work_EmployeeStatusId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Work_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrgChartWorkPlaceId", Name = "IX_Work_OrgChartWorkPlaceId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Work_OrganisationChartId")]
public partial class Work
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long? OrgChartWorkPlaceId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long EmployeeStatusId { get; set; }

    public long EmployeeId { get; set; }

    public long? IndustryTypeId { get; set; }

    public long? ActivityTypeId { get; set; }

    public long? InsuranceTypeId { get; set; }

    public long? RelatedTypeId { get; set; }

    public long? EducationGradeId { get; set; }

    public long? StatusId { get; set; }

    [StringLength(256)]
    public string? WorkPlaceDesc { get; set; }

    [StringLength(128)]
    public string? LetterNumber { get; set; }

    [StringLength(256)]
    public string? LastTitle { get; set; }

    public DateOnly? WorkingFrom { get; set; }

    public DateOnly? LetterDate { get; set; }

    public DateOnly? WorkingTo { get; set; }

    public DateOnly? HistoryDate { get; set; }

    public byte InsHsyYear { get; set; }

    public byte InsHsyMonth { get; set; }

    public byte InsHsyDay { get; set; }

    public byte AcptInsHsyYear { get; set; }

    public byte AcptInsHsyMonth { get; set; }

    public byte AcptInsHsyDay { get; set; }

    public long? LastSalary { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool? IsComputeable { get; set; }

    public int? ExperienceMult { get; set; }

    public int? RetiredMult { get; set; }

    public int? YearMult { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

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

    public long? LeaveDueToWorkTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EducationGradeId")]
    [InverseProperty("Works")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Works")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("EmployeeStatusId")]
    [InverseProperty("Works")]
    public virtual EmployeeStatus EmployeeStatus { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("Works")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrgChartWorkPlaceId")]
    [InverseProperty("WorkOrgChartWorkPlaces")]
    public virtual OrganisationChart? OrgChartWorkPlace { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("WorkOrganisationCharts")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
