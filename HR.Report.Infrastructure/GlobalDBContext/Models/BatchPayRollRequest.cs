using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_PayRoll_Request", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteId", Name = "IX_Batch_PayRoll_Request_BankDisketteId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceDisketteId", Name = "IX_Batch_PayRoll_Request_InsuranceDisketteId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Batch_PayRoll_Request_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Batch_PayRoll_Request_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("RequestStateId", Name = "IX_Batch_PayRoll_Request_RequestStateId")]
[Microsoft.EntityFrameworkCore.Index("RequestTypeId", Name = "IX_Batch_PayRoll_Request_RequestTypeId")]
[Microsoft.EntityFrameworkCore.Index("TaxDisketteId", Name = "IX_Batch_PayRoll_Request_TaxDisketteId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_Batch_PayRoll_Request_UserId")]
public partial class BatchPayRollRequest
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long RequestStateId { get; set; }

    public long RequestTypeId { get; set; }

    [StringLength(256)]
    public string? Username { get; set; }

    public string? RequsetDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastPoolingTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinishDateTime { get; set; }

    public bool IsDone { get; set; }

    public int EmployeeCount { get; set; }

    public int SuccessCount { get; set; }

    public long? PoolingEmployeeId { get; set; }

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

    public long? PaymentPeriodId { get; set; }

    public long? BankDisketteId { get; set; }

    public long? InsuranceDisketteId { get; set; }

    public long? TaxDisketteId { get; set; }

    public string? ExeptionMessage { get; set; }

    public long UserId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankDisketteId")]
    [InverseProperty("BatchPayRollRequests")]
    public virtual BankDiskette? BankDiskette { get; set; }

    [InverseProperty("BatchPayRollRequest")]
    public virtual ICollection<BankDiskette> BankDiskettes { get; set; } = new List<BankDiskette>();

    [InverseProperty("BatchPayRollRequest")]
    public virtual ICollection<BatchPayRollRequestDetail> BatchPayRollRequestDetails { get; set; } = new List<BatchPayRollRequestDetail>();

    [ForeignKey("InsuranceDisketteId")]
    [InverseProperty("BatchPayRollRequests")]
    public virtual InsuranceDiskette? InsuranceDiskette { get; set; }

    [InverseProperty("BatchPayRollRequest")]
    public virtual ICollection<InsuranceDiskette> InsuranceDiskettes { get; set; } = new List<InsuranceDiskette>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("BatchPayRollRequests")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("BatchPayRollRequests")]
    public virtual PaymentPeriod? PaymentPeriod { get; set; }

    [ForeignKey("TaxDisketteId")]
    [InverseProperty("BatchPayRollRequests")]
    public virtual TaxDiskette? TaxDiskette { get; set; }

    [InverseProperty("BatchPayRollRequest")]
    public virtual ICollection<TaxDiskette> TaxDiskettes { get; set; } = new List<TaxDiskette>();

    [ForeignKey("UserId")]
    [InverseProperty("BatchPayRollRequests")]
    public virtual AspNetUser User { get; set; } = null!;
}
