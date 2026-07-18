using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Order_Type", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Order_Type_title", IsUnique = true)]
public partial class OrderType
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

    [InverseProperty("OrderType")]
    public virtual ICollection<BatchRequest> BatchRequests { get; set; } = new List<BatchRequest>();

    [InverseProperty("OrderType")]
    public virtual ICollection<InterdictOrder> InterdictOrders { get; set; } = new List<InterdictOrder>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChanges { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCanChange>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeChecks { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCheck>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCoefficient>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeDescription> OrganisationEmployeeTypeOrderTypeDescriptions { get; set; } = new List<OrganisationEmployeeTypeOrderTypeDescription>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeSummaryCalc> OrganisationEmployeeTypeOrderTypeSummaryCalcs { get; set; } = new List<OrganisationEmployeeTypeOrderTypeSummaryCalc>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeOrderTypeWageItem>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderType> OrganisationEmployeeTypeOrderTypes { get; set; } = new List<OrganisationEmployeeTypeOrderType>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationOrderTypeHistoryExclusion> OrganisationOrderTypeHistoryExclusions { get; set; } = new List<OrganisationOrderTypeHistoryExclusion>();

    [InverseProperty("OrderType")]
    public virtual ICollection<OrganisationOrderType> OrganisationOrderTypes { get; set; } = new List<OrganisationOrderType>();
}
