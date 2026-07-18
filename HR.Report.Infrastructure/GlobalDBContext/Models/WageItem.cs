using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Wage_Item", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Wage_Item_title", IsUnique = true)]
public partial class WageItem
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

    [InverseProperty("WageItem")]
    public virtual ICollection<ArearFicheItem> ArearFicheItems { get; set; } = new List<ArearFicheItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<ArearsChangedFicheItem> ArearsChangedFicheItems { get; set; } = new List<ArearsChangedFicheItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<CostCenterFicheItem> CostCenterFicheItems { get; set; } = new List<CostCenterFicheItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<DeductedArear> DeductedArears { get; set; } = new List<DeductedArear>();

    [InverseProperty("WageItem")]
    public virtual ICollection<DeductionType> DeductionTypes { get; set; } = new List<DeductionType>();

    [InverseProperty("WageItem")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<InterdictOrderWageItem> InterdictOrderWageItems { get; set; } = new List<InterdictOrderWageItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<LoanType> LoanTypes { get; set; } = new List<LoanType>();

    [InverseProperty("WageItem")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficientBonusWageItem> OrganisationEmployeeTypeCoefficientBonusWageItems { get; set; } = new List<OrganisationEmployeeTypeCoefficientBonusWageItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItems { get; set; } = new List<OrganisationEmployeeTypeFicheItem>();

    [InverseProperty("EmployeeWageItem")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitionEmployeeWageItems { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("EmployerWageItem")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitionEmployerWageItems { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("WageItem")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeOrderTypeWageItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<OrganisationEmployeeTypeWageItem> OrganisationEmployeeTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeWageItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<OrganisationFicheItem> OrganisationFicheItems { get; set; } = new List<OrganisationFicheItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<OrganisationWageItem> OrganisationWageItems { get; set; } = new List<OrganisationWageItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItems { get; set; } = new List<PersonnelFicheItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<StatusListItem> StatusListItems { get; set; } = new List<StatusListItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<TaxCoefficientItem> TaxCoefficientItems { get; set; } = new List<TaxCoefficientItem>();

    [InverseProperty("WageItem")]
    public virtual ICollection<TaxableIncome> TaxableIncomes { get; set; } = new List<TaxableIncome>();

    [InverseProperty("WageItem")]
    public virtual ICollection<Tax> Taxes { get; set; } = new List<Tax>();
}
