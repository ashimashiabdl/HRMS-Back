using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_OrderType", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_OrderType_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrderLevelTypeId", Name = "IX_Organisation_EmployeeType_OrderType_OrderLevelTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_EmployeeType_OrderType_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_OrderType_OrganisationChartId")]
public partial class OrganisationEmployeeTypeOrderType
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long OrderTypeId { get; set; }

    public bool OrderCopyToMyCurrentWorkplace { get; set; }

    public bool OrderCopyToMyNewWorkplace { get; set; }

    public bool OrderCopyToMyCurrentCostCenter { get; set; }

    public bool OrderCopyToMyNewCostCenter { get; set; }

    public bool OrderCopyToMyCurrentOrganizationUnit { get; set; }

    public bool OrderCopyToMyNewOrganizationUnit { get; set; }

    public bool ForceSetNullForPost { get; set; }

    public bool ForceSetNullForJob { get; set; }

    /// <summary>
    /// True: پست از چارت سازمانی انتخاب شود؛ False: عنوان از جدول پست‌ها انتخاب شود
    /// </summary>
    public bool SelectPostFromChart { get; set; }

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

    public int? ExperienceCoefficient { get; set; }

    public bool? NeedSettlement { get; set; }

    public int? RetiredCoefficient { get; set; }

    public int? YearCoefficient { get; set; }

    public long OrderLevelTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypes")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypes")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
