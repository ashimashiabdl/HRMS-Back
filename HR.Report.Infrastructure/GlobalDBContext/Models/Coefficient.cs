using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Coefficient", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Coefficient_title", IsUnique = true)]
public partial class Coefficient
{
    [Key]
    public long Id { get; set; }

    [StringLength(450)]
    public string? Description { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("Coefficient")]
    public virtual ICollection<InterdictOrderCoefficientItem> InterdictOrderCoefficientItems { get; set; } = new List<InterdictOrderCoefficientItem>();

    [InverseProperty("Coefficient")]
    public virtual ICollection<OrganisationCoefficient> OrganisationCoefficients { get; set; } = new List<OrganisationCoefficient>();

    [InverseProperty("Coefficient")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficientBonusWageItem> OrganisationEmployeeTypeCoefficientBonusWageItems { get; set; } = new List<OrganisationEmployeeTypeCoefficientBonusWageItem>();

    [InverseProperty("Coefficient")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficient> OrganisationEmployeeTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeCoefficient>();

    [InverseProperty("Coefficient")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCoefficient>();

    [InverseProperty("Coefficient")]
    public virtual ICollection<PaymentPeriodEmployeeBonu> PaymentPeriodEmployeeBonus { get; set; } = new List<PaymentPeriodEmployeeBonu>();
}
