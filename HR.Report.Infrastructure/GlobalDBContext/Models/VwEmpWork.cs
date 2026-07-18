using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Keyless]
public partial class VwEmpWork
{
    public int WorkHistoryTypeCode { get; set; }

    [StringLength(20)]
    public string WorkHistoryTypeName { get; set; } = null!;

    public long Id { get; set; }

    public long EmployeeId { get; set; }

    [StringLength(515)]
    public string? WorkPlac { get; set; }

    [StringLength(256)]
    public string? LastTitle { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? WorkingFrom { get; set; }

    public DateOnly? WorkingTo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(10)]
    public string? TotalCalcExperienceDurationStr { get; set; }

    [StringLength(10)]
    public string? TotalCalcRetiredDurationStr { get; set; }

    [StringLength(10)]
    public string? TotalCalcYearDurationStr { get; set; }

    public long? IndustryTypeId { get; set; }

    public byte? InsHsyYear { get; set; }

    public byte? InsHsyMonth { get; set; }

    public byte? InsHsyDay { get; set; }

    public byte? AcptInsHsyYear { get; set; }

    public byte? AcptInsHsyMonth { get; set; }

    public byte? AcptInsHsyDay { get; set; }

    public long? ActivityTypeId { get; set; }

    public long? InsuranceTypeId { get; set; }

    public long? LastSalary { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public int? ExperienceMult { get; set; }

    public int? RetiredMult { get; set; }

    public int? YearMult { get; set; }

    public long? EduGradeId { get; set; }

    [StringLength(256)]
    public string? EduGrade { get; set; }

    public int? IsComputeable { get; set; }

    public long? RelatedTypeId { get; set; }

    [StringLength(256)]
    public string? IndustryTypeName { get; set; }

    [StringLength(256)]
    public string? ActivityTypeName { get; set; }

    [StringLength(256)]
    public string? InsuranceTypeName { get; set; }

    [StringLength(256)]
    public string? RelatedTypeName { get; set; }

    [StringLength(256)]
    public string? LeaveDueToWork { get; set; }

    [StringLength(128)]
    public string? LetterNumber { get; set; }

    public DateOnly? LetterDate { get; set; }

    [Column("PayLocationID")]
    public long? PayLocationId { get; set; }

    public long? EmployeeTypeId { get; set; }

    public long? EmployeeStatusId { get; set; }

    public int? CalcExperienceDurationYears { get; set; }

    public int? CalcExperienceDurationMonth { get; set; }

    public int? CalcExperienceDurationDays { get; set; }

    public int? CalcRetiredDurationYears { get; set; }

    public int? CalcRetiredDurationMonth { get; set; }

    public int? CalcRetiredDurationDays { get; set; }

    public int? CalcYearDurationYears { get; set; }

    public int? CalcYearDurationMonth { get; set; }

    public int? CalcYearDurationDays { get; set; }
}
