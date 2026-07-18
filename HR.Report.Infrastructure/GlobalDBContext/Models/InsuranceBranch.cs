using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Insurance_Branch", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("InsuranceTypeId", Name = "IX_Insurance_Branch_InsuranceTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Insurance_Branch_OrganisationChartId")]
public partial class InsuranceBranch
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long InsuranceTypeId { get; set; }

    [StringLength(512)]
    public string? WorkshopName { get; set; }

    [StringLength(128)]
    public string? WorkshopCode { get; set; }

    [StringLength(32)]
    public string? WorkshopPhone { get; set; }

    [StringLength(512)]
    public string? WorkshopAddress { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(512)]
    public string? EmployerName { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("InsuranceBranch")]
    public virtual ICollection<InsuranceDiskette> InsuranceDiskettes { get; set; } = new List<InsuranceDiskette>();

    [ForeignKey("InsuranceTypeId")]
    [InverseProperty("InsuranceBranches")]
    public virtual InsuranceType InsuranceType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("InsuranceBranches")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
