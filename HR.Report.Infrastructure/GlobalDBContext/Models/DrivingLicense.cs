using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Driving_License", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Driving_License_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Driving_License_OrganisationChartId")]
public partial class DrivingLicense
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long? LicenseTypeId { get; set; }

    public long? LicenseValidationPeriodId { get; set; }

    public long? DrivingConstraintId { get; set; }

    public long? PrimaryOrSecondaryId { get; set; }

    [StringLength(256)]
    public string? LicenseSerialNumber { get; set; }

    [StringLength(256)]
    public string? PreviousDerivingNumber { get; set; }

    [StringLength(256)]
    public string? Licencedescription { get; set; }

    public DateOnly? LicenseImplDate { get; set; }

    public DateOnly? LicensePrimaryImplDate { get; set; }

    public DateOnly? LicenseExpireDate { get; set; }

    public DateOnly? Previouslicenseimpldate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("DrivingLicenses")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("DrivingLicenses")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
