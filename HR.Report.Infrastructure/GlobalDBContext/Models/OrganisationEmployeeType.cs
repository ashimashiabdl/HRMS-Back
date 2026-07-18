using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeGroupId", Name = "IX_Organisation_EmployeeType_EmployeeTypeGroupId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_OrganisationChartId")]
public partial class OrganisationEmployeeType
{
    [Key]
    public long Id { get; set; }

    public long EmployeeTypeId { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeGroupId { get; set; }

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

    public long? TaxBaseTable7Id { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(50)]
    public string? EmployeeTypeFinancialCode { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypes")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("EmployeeTypeGroupId")]
    [InverseProperty("OrganisationEmployeeTypes")]
    public virtual EmployeeTypeGroup EmployeeTypeGroup { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
