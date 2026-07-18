using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organ_Property", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organ_Property_OrganisationChartId")]
public partial class OrganProperty
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? InsuranceWorkShopName { get; set; }

    [StringLength(512)]
    public string? Address { get; set; }

    [StringLength(256)]
    public string? InsuranceWorkShopNo { get; set; }

    [StringLength(256)]
    public string? BankAccountNo { get; set; }

    [StringLength(256)]
    public string? TaxDocumentNo { get; set; }

    public int? TaxBranchCode { get; set; }

    [StringLength(256)]
    public string? TaxBranchName { get; set; }

    [StringLength(256)]
    public string? TaxEconomicNo { get; set; }

    [StringLength(256)]
    public string? TaxPostalCode { get; set; }

    [StringLength(256)]
    public string? TaxPhoneNo { get; set; }

    [StringLength(32)]
    public string? TaxFirstEmployerNationalNo { get; set; }

    [StringLength(128)]
    public string? TaxFirstEmployerFirstName { get; set; }

    [StringLength(128)]
    public string? TaxFirstEmployerLastName { get; set; }

    [StringLength(256)]
    public string? TaxFirstEmployerPostName { get; set; }

    [StringLength(32)]
    public string? TaxSecondEmployerNationalNo { get; set; }

    [StringLength(128)]
    public string? TaxSecondEmployerFirstName { get; set; }

    [StringLength(128)]
    public string? TaxSecondEmployerLastName { get; set; }

    [StringLength(128)]
    public string? TaxSecondEmployerPostName { get; set; }

    [StringLength(128)]
    public string? TaxAccountNo { get; set; }

    [StringLength(128)]
    public string? MailAddress { get; set; }

    [StringLength(16)]
    public string? PhoneNo { get; set; }

    public long? CurrentServiceLocationId { get; set; }

    public int? Type { get; set; }

    [StringLength(128)]
    public string? EmployerName { get; set; }

    public int? OrganLevel { get; set; }

    public long? FinanceId { get; set; }

    [StringLength(32)]
    public string? NationalNo { get; set; }

    [StringLength(32)]
    public string? PostalCode { get; set; }

    [StringLength(32)]
    public string? Fax { get; set; }

    [StringLength(64)]
    public string? ContractNo { get; set; }

    [StringLength(64)]
    public string? CurrentServiceTitle { get; set; }

    public int ServiceLocationGroupId { get; set; }

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

    public int? InsuranceRate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganProperties")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
