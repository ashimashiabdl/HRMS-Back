using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bank_Diskette", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankDisketteStatusId", Name = "IX_Bank_Diskette_BankDisketteStatusId")]
[Microsoft.EntityFrameworkCore.Index("BatchPayRollRequestId", Name = "IX_Bank_Diskette_BatchPayRollRequestId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Bank_Diskette_OrganisationChartId")]
public partial class BankDiskette
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

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

    [StringLength(128)]
    public string? CodeList { get; set; }

    public long PaymentPeriodId { get; set; }

    public int AllPersonnelCount { get; set; }

    public long SumPaymentAmount { get; set; }

    public long BankDisketteStatusId { get; set; }

    public long? BatchPayRollRequestId { get; set; }

    /// <summary>
    /// ������ ��Ә� ���� ����� ��ǘ� ����� �� �� ���� ���� ��� ����� ����� ���
    /// </summary>
    public bool CalculateAllFichesInCurrentPeriod { get; set; }

    public string? DescriptionOfTheDeposit { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("BankDiskette")]
    public virtual ICollection<BankDisketteCostCenter> BankDisketteCostCenters { get; set; } = new List<BankDisketteCostCenter>();

    [InverseProperty("BankDiskette")]
    public virtual ICollection<BankDisketteGroupAndFile> BankDisketteGroupAndFiles { get; set; } = new List<BankDisketteGroupAndFile>();

    [InverseProperty("BankDiskette")]
    public virtual ICollection<BankDisketteItem> BankDisketteItems { get; set; } = new List<BankDisketteItem>();

    [ForeignKey("BatchPayRollRequestId")]
    [InverseProperty("BankDiskettes")]
    public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }

    [InverseProperty("BankDiskette")]
    public virtual ICollection<BatchPayRollRequest> BatchPayRollRequests { get; set; } = new List<BatchPayRollRequest>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("BankDiskettes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("BankDiskettes")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [InverseProperty("BankDiskette")]
    public virtual ICollection<StatusListItem> StatusListItems { get; set; } = new List<StatusListItem>();
}
