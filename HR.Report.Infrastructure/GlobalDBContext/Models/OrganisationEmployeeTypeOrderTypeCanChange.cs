using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_OrderType_CanChange", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("DefaultEmpStatusId", Name = "IX_Organisation_EmployeeType_OrderType_CanChange_DefaultEmpStatusId")]
[Microsoft.EntityFrameworkCore.Index("DefaultEmpTypeId", Name = "IX_Organisation_EmployeeType_OrderType_CanChange_DefaultEmpTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_OrderType_CanChange_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_EmployeeType_OrderType_CanChange_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_OrderType_CanChange_OrganisationChartId")]
public partial class OrganisationEmployeeTypeOrderTypeCanChange
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long OrderTypeId { get; set; }

    public bool CanImplDate { get; set; }

    public bool CanExpireDate { get; set; }

    public bool CanPayLocationId { get; set; }

    public bool CanPositionId { get; set; }

    [Column("CanJobID")]
    public bool CanJobId { get; set; }

    public bool CanOrganizationUnitId { get; set; }

    public bool CanWorkPlaceId { get; set; }

    public bool CanEmployeeStatusId { get; set; }

    public bool CanEmployeeTypeId { get; set; }

    public bool CanCostCenterId { get; set; }

    public bool CanProjectId { get; set; }

    public long? DefaultEmpTypeId { get; set; }

    public long? DefaultEmpStatusId { get; set; }

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

    [ForeignKey("DefaultEmpStatusId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeCanChanges")]
    public virtual EmployeeStatus? DefaultEmpStatus { get; set; }

    [ForeignKey("DefaultEmpTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeCanChangeDefaultEmpTypes")]
    public virtual EmployeeType? DefaultEmpType { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeCanChangeEmployeeTypes")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeCanChanges")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeCanChanges")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
