using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bill_Instance", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BillId", Name = "IX_Bill_Instance_BillId")]
[Microsoft.EntityFrameworkCore.Index("BuyerCostCenterId", Name = "IX_Bill_Instance_BuyerCostCenterId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Bill_Instance_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("SellerCostCenterId", Name = "IX_Bill_Instance_SellerCostCenterId")]
public partial class BillInstance
{
    [Key]
    public long Id { get; set; }

    public long BillId { get; set; }

    [StringLength(128)]
    public string? SerialNo { get; set; }

    public long PaymentPeriodId { get; set; }

    public long SellerCostCenterId { get; set; }

    public long BuyerCostCenterId { get; set; }

    [StringLength(128)]
    public string? GeneratedBillKey { get; set; }

    public long AllAmount { get; set; }

    public long WageAmount { get; set; }

    public long TaxAmount { get; set; }

    public long PayableAmount { get; set; }

    public long? Count { get; set; }

    public long? TaxAmount3 { get; set; }

    public long? TaxAmount6 { get; set; }

    [StringLength(128)]
    public string? InvoiceNo { get; set; }

    [StringLength(128)]
    public string? ContractNo { get; set; }

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

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BillId")]
    [InverseProperty("BillInstances")]
    public virtual Bill Bill { get; set; } = null!;

    [ForeignKey("BuyerCostCenterId")]
    [InverseProperty("BillInstanceBuyerCostCenters")]
    public virtual OrganisationChart BuyerCostCenter { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("BillInstances")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [ForeignKey("SellerCostCenterId")]
    [InverseProperty("BillInstanceSellerCostCenters")]
    public virtual OrganisationChart SellerCostCenter { get; set; } = null!;
}
