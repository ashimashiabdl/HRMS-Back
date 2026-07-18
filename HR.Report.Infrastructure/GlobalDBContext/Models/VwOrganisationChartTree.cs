using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Keyless]
public partial class VwOrganisationChartTree
{
    public Guid? RowNumber { get; set; }

    [Column("OrganisationChartID")]
    public long? OrganisationChartId { get; set; }

    [StringLength(128)]
    public string? Code { get; set; }

    [StringLength(256)]
    public string? Name { get; set; }

    [Column("OrganParentID")]
    public long? OrganParentId { get; set; }

    public long? OrgType { get; set; }

    [StringLength(128)]
    public string? SystemCode { get; set; }

    [Column("ParentOrganisationChartID")]
    public long? ParentOrganisationChartId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public int? Descriptions { get; set; }

    public bool? IsCostCenter { get; set; }

    public bool? IsPayLocation { get; set; }

    [StringLength(128)]
    public string? LetterCode { get; set; }

    [StringLength(128)]
    public string? Rank { get; set; }

    public int? Order { get; set; }

    public bool? IsRegister { get; set; }

    [StringLength(500)]
    public string? NameFull { get; set; }

    [Column("IDFull")]
    [StringLength(500)]
    public string? Idfull { get; set; }

    [Column("OrganizationUnitID")]
    public long OrganizationUnitId { get; set; }

    public int? State { get; set; }

    public int? NodeLevel { get; set; }

    public bool? IsOrg { get; set; }

    [Column("OrganizationTypeID")]
    public long? OrganizationTypeId { get; set; }

    [StringLength(256)]
    public string? OrganizationTypeName { get; set; }

    public bool? IsApproved { get; set; }

    public int? Grade { get; set; }

    public int? ApprovedPositionCapacity { get; set; }

    public int? StaffingCapacity { get; set; }
}
