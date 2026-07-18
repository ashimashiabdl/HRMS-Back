using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Status_List", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Status_List_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("StatusListTypeId", Name = "IX_Status_List_StatusListTypeId")]
public partial class StatusList
{
    [Key]
    public long Id { get; set; }

    public long PaymentPeriodId { get; set; }

    public int PersonnelCount { get; set; }

    public long SumPaymentAmount { get; set; }

    public long SumDeductAmount { get; set; }

    public long SumEmployerAmount { get; set; }

    public long? StatusListTypeId { get; set; }

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

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("StatusLists")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [InverseProperty("StatusList")]
    public virtual ICollection<StatusListItem> StatusListItems { get; set; } = new List<StatusListItem>();
}
