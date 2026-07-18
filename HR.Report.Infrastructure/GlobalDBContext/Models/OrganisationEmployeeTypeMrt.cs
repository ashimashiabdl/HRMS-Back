using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_MRT", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_MRT_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_MRT_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationMrtid", Name = "IX_Organisation_EmployeeType_MRT_OrganisationMRTId")]
[Microsoft.EntityFrameworkCore.Index("SettingTypeId", Name = "IX_Organisation_EmployeeType_MRT_SettingTypeId")]
public partial class OrganisationEmployeeTypeMrt
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    [Column("OrganisationMRTId")]
    public long OrganisationMrtid { get; set; }

    public bool IsRaw { get; set; }

    public bool IsBatch { get; set; }

    public bool IsManager { get; set; }

    [StringLength(1024)]
    public string? Description { get; set; }

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

    public long? SettingTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeMrts")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeMrts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationMrtid")]
    [InverseProperty("OrganisationEmployeeTypeMrts")]
    public virtual OrganisationMrt OrganisationMrt { get; set; } = null!;
}
