using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Type", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Employee_Type_title", IsUnique = true)]
public partial class EmployeeType
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

    [InverseProperty("EmployeeType")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlements { get; set; } = new List<EmployeeSettlement>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<Fiche> Fiches { get; set; } = new List<Fiche>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<FunctionExcelDefinition> FunctionExcelDefinitions { get; set; } = new List<FunctionExcelDefinition>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficientBonusWageItem> OrganisationEmployeeTypeCoefficientBonusWageItems { get; set; } = new List<OrganisationEmployeeTypeCoefficientBonusWageItem>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficient> OrganisationEmployeeTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeCoefficient>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItems { get; set; } = new List<OrganisationEmployeeTypeFicheItem>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitions { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeLeave> OrganisationEmployeeTypeLeaves { get; set; } = new List<OrganisationEmployeeTypeLeave>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeMrt> OrganisationEmployeeTypeMrts { get; set; } = new List<OrganisationEmployeeTypeMrt>();

    [InverseProperty("DefaultEmpType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChangeDefaultEmpTypes { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCanChange>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChangeEmployeeTypes { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCanChange>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeChecks { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCheck>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCoefficient>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeDescription> OrganisationEmployeeTypeOrderTypeDescriptions { get; set; } = new List<OrganisationEmployeeTypeOrderTypeDescription>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeSummaryCalc> OrganisationEmployeeTypeOrderTypeSummaryCalcs { get; set; } = new List<OrganisationEmployeeTypeOrderTypeSummaryCalc>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeOrderTypeWageItem>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeOrderType> OrganisationEmployeeTypeOrderTypes { get; set; } = new List<OrganisationEmployeeTypeOrderType>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; } = new List<OrganisationEmployeeTypeSettlementItem>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeTypeWageItem> OrganisationEmployeeTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeWageItem>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationEmployeeType> OrganisationEmployeeTypes { get; set; } = new List<OrganisationEmployeeType>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<OrganisationSetting> OrganisationSettings { get; set; } = new List<OrganisationSetting>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<PersonnelFunctionExcelFile> PersonnelFunctionExcelFiles { get; set; } = new List<PersonnelFunctionExcelFile>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<RecruitOrder> RecruitOrders { get; set; } = new List<RecruitOrder>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<StatusListItem> StatusListItems { get; set; } = new List<StatusListItem>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<Tax> Taxes { get; set; } = new List<Tax>();

    [InverseProperty("EmployeeType")]
    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
