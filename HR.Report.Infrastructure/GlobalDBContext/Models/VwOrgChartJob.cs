using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Keyless]
public partial class VwOrgChartJob
{
    public long Id { get; set; }

    public long? JobId { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(30)]
    public string? TotalJobCode { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(50)]
    public string? SystemCode { get; set; }

    [StringLength(255)]
    public string? Code { get; set; }

    [StringLength(512)]
    public string? JobDescriptions { get; set; }

    [StringLength(256)]
    public string JobTitle { get; set; } = null!;

    public int JobDegree { get; set; }

    public long? StaffingRuleId { get; set; }

    public long? JobNatureId { get; set; }

    [StringLength(256)]
    public string? StfRul { get; set; }

    [StringLength(256)]
    public string? JobNatureName { get; set; }

    public long? InsuranceDesketid { get; set; }

    [StringLength(50)]
    public string? InsuranceJobCode { get; set; }

    [StringLength(50)]
    public string? InsuranceJobName { get; set; }

    [Column("EntOrgChartJobCategoryID")]
    public long? EntOrgChartJobCategoryId { get; set; }

    [StringLength(10)]
    public string? EntOrgChartJobCategoryCode { get; set; }

    [StringLength(256)]
    public string EntOrgChartJobCategoryName { get; set; } = null!;

    [Column("EntOrgChartJobGroupID")]
    public long? EntOrgChartJobGroupId { get; set; }

    [StringLength(50)]
    public string? EntOrgChartJobGroupCode { get; set; }

    [StringLength(256)]
    public string? EntOrgChartJobGroupName { get; set; }

    [Column("OrgChartJobSeriesID")]
    public long? OrgChartJobSeriesId { get; set; }

    [StringLength(50)]
    public string? EntOrgChartJobSeriesCode { get; set; }

    [StringLength(256)]
    public string? EntOrgChartJobSeriesName { get; set; }

    public bool IsDifficultJob { get; set; }
}
