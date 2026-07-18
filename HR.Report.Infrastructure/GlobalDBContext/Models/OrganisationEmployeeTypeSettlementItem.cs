using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_Settlement_Item", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_Settlement_Item_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_Settlement_Item_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationFormulaId", Name = "IX_Organisation_EmployeeType_Settlement_Item_OrganisationFormulaId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Organisation_EmployeeType_Settlement_Item_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("SettlementItemId", Name = "IX_Organisation_EmployeeType_Settlement_Item_SettlementItemId")]
public partial class OrganisationEmployeeTypeSettlementItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long SettlementItemId { get; set; }

    public long PaymentTypeId { get; set; }

    public long EnterTypeId { get; set; }

    public long? OrganisationFormulaId { get; set; }

    public long? MeasurementUnitId { get; set; }

    public int? FixValue { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? FinancialCode { get; set; }

    public bool? IsEditAble { get; set; }

    public bool? IsImport { get; set; }

    public int? Priority { get; set; }

    public bool? ShowLoan { get; set; }

    public bool? IsVirtual { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeSettlementItems")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeSettlementItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationFormulaId")]
    [InverseProperty("OrganisationEmployeeTypeSettlementItems")]
    public virtual OrganisationFormula? OrganisationFormula { get; set; }

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("OrganisationEmployeeTypeSettlementItems")]
    public virtual BaseTableValue PaymentType { get; set; } = null!;

    [ForeignKey("EnterTypeId")]
    public virtual BaseTableValue? EnterType { get; set; }

    [ForeignKey("SettlementItemId")]
    [InverseProperty("OrganisationEmployeeTypeSettlementItems")]
    public virtual SettlementItem SettlementItem { get; set; } = null!;
}
