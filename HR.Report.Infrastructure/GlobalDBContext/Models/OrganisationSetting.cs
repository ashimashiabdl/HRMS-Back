using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Setting", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_Setting_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Setting_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SettingId", Name = "IX_Organisation_Setting_SettingId")]
public partial class OrganisationSetting
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long SettingId { get; set; }

    public bool IsActive { get; set; }

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

    public int Code { get; set; }

    public long? EmployeeTypeId { get; set; }

    [StringLength(256)]
    public string? Hourparameters { get; set; }

    [StringLength(128)]
    public string? Value { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationSettings")]
    public virtual EmployeeType? EmployeeType { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationSettings")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("SettingId")]
    [InverseProperty("OrganisationSettings")]
    public virtual Setting Setting { get; set; } = null!;
}
