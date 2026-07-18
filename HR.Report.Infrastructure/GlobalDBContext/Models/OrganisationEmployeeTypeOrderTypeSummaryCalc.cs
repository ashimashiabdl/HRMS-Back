using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_OrderType_SummaryCalc", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("CalExperienceRecordedEntertypeId", Name = "IX_Organisation_EmployeeType_OrderType_SummaryCalc_CalExperienceRecordedEntertypeId")]
[Microsoft.EntityFrameworkCore.Index("CalRetiredRecordedEntertypeId", Name = "IX_Organisation_EmployeeType_OrderType_SummaryCalc_CalRetiredRecordedEntertypeId")]
[Microsoft.EntityFrameworkCore.Index("CalYearRecordedEntertypeId", Name = "IX_Organisation_EmployeeType_OrderType_SummaryCalc_CalYearRecordedEntertypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_OrderType_SummaryCalc_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_EmployeeType_OrderType_SummaryCalc_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_OrderType_SummaryCalc_OrganisationChartId")]
public partial class OrganisationEmployeeTypeOrderTypeSummaryCalc
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long OrderTypeId { get; set; }

    public bool CalExperienceRecorded { get; set; }

    public bool CalRetiredRecorded { get; set; }

    public bool CalYearRecorded { get; set; }

    public bool CalHistoryOut { get; set; }

    public bool CalHistoryStop { get; set; }

    [Column("CalretiredFlagOK")]
    public bool CalretiredFlagOk { get; set; }

    public bool CalMarriageStatus { get; set; }

    public bool CalSponsorshipCount { get; set; }

    public bool CalEducationInfo { get; set; }

    public bool CalCapivityInfo { get; set; }

    public bool CalIsarInfo { get; set; }

    public bool CalBasijInfo { get; set; }

    public bool CalWarInfo { get; set; }

    public bool CalPersonnelInfo { get; set; }

    public bool CalIsWomenHeadInfo { get; set; }

    public bool CalOtherVeteransInfo { get; set; }

    public bool CalInsuranceTypeInfo { get; set; }

    public bool CalDrivingLicenseNumberInfo { get; set; }

    public bool CalEmployeeDate { get; set; }

    public bool CalMartyrStatus { get; set; }

    public bool CalFamilyInfo { get; set; }

    public bool CalChildCount { get; set; }

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

    public long? CalExperienceRecordedEntertypeId { get; set; }

    public long? CalRetiredRecordedEntertypeId { get; set; }

    public long? CalYearRecordedEntertypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeSummaryCalcs")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeSummaryCalcs")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeSummaryCalcs")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
