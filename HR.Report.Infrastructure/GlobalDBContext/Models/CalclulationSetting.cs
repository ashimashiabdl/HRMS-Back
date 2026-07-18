using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Calclulation_Setting", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Calclulation_Setting_OrganisationChartId", IsUnique = true)]
[Microsoft.EntityFrameworkCore.Index("RewardAndSanavatStoreTypeId", Name = "IX_Calclulation_Setting_RewardAndSanavatStoreTypeId")]
[Microsoft.EntityFrameworkCore.Index("RewardFormulaId", Name = "IX_Calclulation_Setting_RewardFormulaId")]
[Microsoft.EntityFrameworkCore.Index("SanavatFormulaId", Name = "IX_Calclulation_Setting_SanavatFormulaId")]
public partial class CalclulationSetting
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? RewardFormulaId { get; set; }

    public long? SanavatFormulaId { get; set; }

    public long? RewardAndSanavatStoreTypeId { get; set; }

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

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("CalclulationSetting")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("RewardFormulaId")]
    [InverseProperty("CalclulationSettingRewardFormulas")]
    public virtual OrganisationFormula? RewardFormula { get; set; }

    [ForeignKey("SanavatFormulaId")]
    [InverseProperty("CalclulationSettingSanavatFormulas")]
    public virtual OrganisationFormula? SanavatFormula { get; set; }
}
