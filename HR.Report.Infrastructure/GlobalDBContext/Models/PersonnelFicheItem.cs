using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Personnel_FicheItem", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Personnel_FicheItem_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EnterTypeId", Name = "IX_Personnel_FicheItem_EnterTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Personnel_FicheItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationCheckFormulaId", Name = "IX_Personnel_FicheItem_OrganisationCheckFormulaId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationFormulaId", Name = "IX_Personnel_FicheItem_OrganisationFormulaId")]
[Microsoft.EntityFrameworkCore.Index("PaymentIntervalId", Name = "IX_Personnel_FicheItem_PaymentIntervalId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelPaymentId", Name = "IX_Personnel_FicheItem_PersonnelPaymentId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Personnel_FicheItem_WageItemId")]
public partial class PersonnelFicheItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long WageItemId { get; set; }

    public int? Value { get; set; }

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

    public bool DeductAtOnce { get; set; }

    public long PaymentIntervalId { get; set; }

    public long? PersonnelPaymentId { get; set; }

    public long? OrganisationCheckFormulaId { get; set; }

    public long? OrganisationFormulaId { get; set; }

    public long EnterTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("PersonnelFicheItems")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("EnterTypeId")]
    [InverseProperty("PersonnelFicheItems")]
    public virtual BaseTableValue EnterType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PersonnelFicheItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationCheckFormulaId")]
    [InverseProperty("PersonnelFicheItemOrganisationCheckFormulas")]
    public virtual OrganisationFormula? OrganisationCheckFormula { get; set; }

    [ForeignKey("OrganisationFormulaId")]
    [InverseProperty("PersonnelFicheItemOrganisationFormulas")]
    public virtual OrganisationFormula? OrganisationFormula { get; set; }

    [ForeignKey("PersonnelPaymentId")]
    [InverseProperty("PersonnelFicheItems")]
    public virtual PersonnelPayment? PersonnelPayment { get; set; }

    [InverseProperty("PersonnelFicheItem")]
    public virtual ICollection<PersonnelPayment> PersonnelPayments { get; set; } = new List<PersonnelPayment>();

    [ForeignKey("WageItemId")]
    [InverseProperty("PersonnelFicheItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
