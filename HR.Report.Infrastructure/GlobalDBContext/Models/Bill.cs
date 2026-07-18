using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Bill", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BillTypeId", Name = "IX_Bill_BillTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Bill_OrganisationChartId")]
public partial class Bill
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? BillTypeId { get; set; }

    [StringLength(128)]
    public string? BillAmountAccountNo { get; set; }

    [StringLength(128)]
    public string? BillTaxAccountNo { get; set; }

    [StringLength(128)]
    public string? BillAmountAccountInfo { get; set; }

    [StringLength(128)]
    public string? BillTaxAccountInfo { get; set; }

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

    [InverseProperty("Bill")]
    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    [InverseProperty("Bill")]
    public virtual ICollection<BillInstance> BillInstances { get; set; } = new List<BillInstance>();

    [InverseProperty("Bill")]
    public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Bills")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
