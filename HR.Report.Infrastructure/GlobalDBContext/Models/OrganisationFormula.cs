using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Formula", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("FormulaId", Name = "IX_Organisation_Formula_FormulaId")]
[Microsoft.EntityFrameworkCore.Index("FormulaUsageLocationId", Name = "IX_Organisation_Formula_FormulaUsageLocationId")]
[Microsoft.EntityFrameworkCore.Index("Id", "IsDeleted", Name = "IX_Organisation_Formula_Id_Active_DateRange")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Formula_OrganisationChartId")]
public partial class OrganisationFormula
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long FormulaId { get; set; }

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

    public bool IsCheckFormula { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    public long FormulaUsageLocationId { get; set; }

    [InverseProperty("RewardFormula")]
    public virtual ICollection<CalclulationSetting> CalclulationSettingRewardFormulas { get; set; } = new List<CalclulationSetting>();

    [InverseProperty("SanavatFormula")]
    public virtual ICollection<CalclulationSetting> CalclulationSettingSanavatFormulas { get; set; } = new List<CalclulationSetting>();

    [ForeignKey("FormulaId")]
    [InverseProperty("OrganisationFormulas")]
    public virtual Formula Formula { get; set; } = null!;

    [InverseProperty("IdNavigation")]
    public virtual FormulaDefinition? FormulaDefinition { get; set; }

    [ForeignKey("FormulaUsageLocationId")]
    [InverseProperty("OrganisationFormulas")]
    public virtual FormulaUsageLocation FormulaUsageLocation { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationFormulas")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("OrganisationCheckFormula")]
    public virtual ICollection<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItemOrganisationCheckFormulas { get; set; } = new List<OrganisationEmployeeTypeFicheItem>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItemOrganisationFormulas { get; set; } = new List<OrganisationEmployeeTypeFicheItem>();

    [InverseProperty("EmployeeFormula")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitionEmployeeFormulas { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("EmployerFormula")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitionEmployerFormulas { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeChecks { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCheck>();

    [InverseProperty("OrganisationCheckFormula")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficientOrganisationCheckFormulas { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCoefficient>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficientOrganisationFormulas { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCoefficient>();

    [InverseProperty("OrganisationCheckFormula")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItemOrganisationCheckFormulas { get; set; } = new List<OrganisationEmployeeTypeOrderTypeWageItem>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItemOrganisationFormulas { get; set; } = new List<OrganisationEmployeeTypeOrderTypeWageItem>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; } = new List<OrganisationEmployeeTypeSettlementItem>();

    [InverseProperty("OrganisationCheckFormula")]
    public virtual ICollection<OrganisationFicheItem> OrganisationFicheItemOrganisationCheckFormulas { get; set; } = new List<OrganisationFicheItem>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<OrganisationFicheItem> OrganisationFicheItemOrganisationFormulas { get; set; } = new List<OrganisationFicheItem>();

    [InverseProperty("OrganisationCheckFormula")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItemOrganisationCheckFormulas { get; set; } = new List<PersonnelFicheItem>();

    [InverseProperty("OrganisationFormula")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItemOrganisationFormulas { get; set; } = new List<PersonnelFicheItem>();
}
