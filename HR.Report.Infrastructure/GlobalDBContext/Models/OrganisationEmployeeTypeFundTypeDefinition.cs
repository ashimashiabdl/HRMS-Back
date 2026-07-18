using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Employee_Type_FundType_Definition", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeFormulaId", Name = "IX_Organisation_Employee_Type_FundType_Definition_EmployeeFormulaId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_Employee_Type_FundType_Definition_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeWageItemId", Name = "IX_Organisation_Employee_Type_FundType_Definition_EmployeeWageItemId")]
[Microsoft.EntityFrameworkCore.Index("EmployerFormulaId", Name = "IX_Organisation_Employee_Type_FundType_Definition_EmployerFormulaId")]
[Microsoft.EntityFrameworkCore.Index("EmployerWageItemId", Name = "IX_Organisation_Employee_Type_FundType_Definition_EmployerWageItemId")]
[Microsoft.EntityFrameworkCore.Index("FundTypeId", Name = "IX_Organisation_Employee_Type_FundType_Definition_FundTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Employee_Type_FundType_Definition_OrganisationChartId")]
public partial class OrganisationEmployeeTypeFundTypeDefinition
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public int EmployeePercent { get; set; }

    public long EmployeeWageItemId { get; set; }

    public long EmployerWageItemId { get; set; }

    public long? EmployeeFormulaId { get; set; }

    public long? EmployerFormulaId { get; set; }

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

    public long FundTypeId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeFormulaId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitionEmployeeFormulas")]
    public virtual OrganisationFormula? EmployeeFormula { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitions")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("EmployeeWageItemId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitionEmployeeWageItems")]
    public virtual WageItem EmployeeWageItem { get; set; } = null!;

    [ForeignKey("EmployerFormulaId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitionEmployerFormulas")]
    public virtual OrganisationFormula? EmployerFormula { get; set; }

    [ForeignKey("EmployerWageItemId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitionEmployerWageItems")]
    public virtual WageItem EmployerWageItem { get; set; } = null!;

    [ForeignKey("FundTypeId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitions")]
    public virtual FundType FundType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeFundTypeDefinitions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
