using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bill_Detail_Exception", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BillDetailId", Name = "IX_Bill_Detail_Exception_BillDetailId")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Bill_Detail_Exception_CostCenterId")]
public partial class BillDetailException
{
    [Key]
    public long Id { get; set; }

    public long BillDetailId { get; set; }

    public long CostCenterId { get; set; }

    public int Value { get; set; }

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

    [ForeignKey("BillDetailId")]
    [InverseProperty("BillDetailExceptions")]
    public virtual BillDetail BillDetail { get; set; } = null!;

    [ForeignKey("CostCenterId")]
    [InverseProperty("BillDetailExceptions")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;
}
