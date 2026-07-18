using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Insurance", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Insurance_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", "OrganisationChartId", "IsLast", "IsDeleted", "Id", Name = "IX_Insurance_EmployeeId_OrgChart_IsLast", IsDescending = new[] { false, false, false, false, true })]
[Microsoft.EntityFrameworkCore.Index("InsWorkShopTypeId", Name = "IX_Insurance_InsWorkShopTypeId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceSubmissionCityId", Name = "IX_Insurance_InsuranceSubmissionCityId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Insurance_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SupplementaryInsuranceTypeId", Name = "IX_Insurance_SupplementaryInsuranceTypeId")]
public partial class Insurance
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public int? AccDay { get; set; }

    public bool? IsLast { get; set; }

    public bool? IsComputable { get; set; }

    [StringLength(32)]
    public string InsuranceNumber { get; set; } = null!;

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

    public long? InsWorkShopTypeId { get; set; }

    public long? InsuranceTypeId { get; set; }

    public long? InsuranceBranchId { get; set; }

    public bool? HasSupplementaryInsurance { get; set; }

    public long? InsuranceSubmissionCityId { get; set; }

    public long? SupplementaryInsuranceTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Insurances")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("Insurance")]
    public virtual ICollection<InsuranceDetail> InsuranceDetails { get; set; } = new List<InsuranceDetail>();

    [ForeignKey("InsuranceSubmissionCityId")]
    [InverseProperty("Insurances")]
    public virtual Place? InsuranceSubmissionCity { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Insurances")]
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("SupplementaryInsuranceTypeId")]
    [InverseProperty("Insurances")]
    public virtual BaseTableValue? SupplementaryInsuranceType { get; set; }
}
