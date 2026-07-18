using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.Data;

[Table("Formula_Table", Schema = "For")]
public class FormulaTable : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("TableType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long TableTypeId { get; set; }
    public virtual BaseTableValue? TableType { get; set; }
    [StringLength(256)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? RelatedContextField { get; set; }
    public int Rank { get; set; }

    public bool SetZeroIfNotFound { get; set; }

    [StringLength(512)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }
}
