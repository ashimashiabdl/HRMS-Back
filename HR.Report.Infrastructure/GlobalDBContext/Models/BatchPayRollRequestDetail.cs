using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_PayRoll_Request_Detail", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BatchPayRollRequestId", Name = "IX_Batch_PayRoll_Request_Detail_BatchPayRollRequestId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Batch_PayRoll_Request_Detail_EmployeeId")]
public partial class BatchPayRollRequestDetail
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long BatchPayRollRequestId { get; set; }

    public long? FicheId { get; set; }

    public string? FinalMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DoDatetime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastTryDateTime { get; set; }

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

    public double RunTimeinMilliseconds { get; set; }

    public long? BankDisketteItemId { get; set; }

    public long? InsuranceDisketteItemId { get; set; }

    [Column("ISMainTax")]
    public string? IsmainTax { get; set; }

    public double Value { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BatchPayRollRequestId")]
    [InverseProperty("BatchPayRollRequestDetails")]
    public virtual BatchPayRollRequest BatchPayRollRequest { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("BatchPayRollRequestDetails")]
    public virtual Employee Employee { get; set; } = null!;
}
