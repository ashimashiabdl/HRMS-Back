using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Military_Service", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EducationGradeId", Name = "IX_Military_Service_EducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Military_Service_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "OrganisationChartId", "IsLast", "IsDeleted", "Id", Name = "IX_Military_Service_EmployeeId_OrgChart_IsLast", IsDescending = new[] { false, false, false, false, true })]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Military_Service_OrganisationChartId")]
public partial class MilitaryService
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? MilitaryStatusCodeId { get; set; }

    public long? ImmunityTypeId { get; set; }

    [StringLength(256)]
    public string? NameOfPeriod { get; set; }

    [StringLength(128)]
    public string? MilitaryDuration { get; set; }

    [StringLength(128)]
    public string MilitaryFullDuration { get; set; } = null!;

    [StringLength(128)]
    public string? MilitaryMinDuration { get; set; }

    public DateOnly? ConfirmedLetterDate { get; set; }

    [StringLength(128)]
    public string? ConfirmedLetterNo { get; set; }

    public DateOnly? MilitaryStartDate { get; set; }

    public DateOnly? MilitaryEndDate { get; set; }

    public DateOnly? MilitariIssueDate { get; set; }

    public DateOnly? ImmunityValidDate { get; set; }

    [StringLength(128)]
    public string? MilitariSerialNo { get; set; }

    [StringLength(512)]
    public string? Descriptions { get; set; }

    public bool? IsContinue { get; set; }

    public bool? IsLast { get; set; }

    public long? DueTypeId { get; set; }

    public long? EducationGradeId { get; set; }

    public bool? IsComputable { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

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

    public long? MilitariGradeTypeId { get; set; }

    [Column("ConfirmerOrganID")]
    public long? ConfirmerOrganId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EducationGradeId")]
    [InverseProperty("MilitaryServices")]
    public virtual EducationGrade? EducationGrade { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("MilitaryServices")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("MilitaryServices")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
