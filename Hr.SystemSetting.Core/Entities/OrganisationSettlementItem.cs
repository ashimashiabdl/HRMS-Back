using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hr.SystemSetting.Core.Entities;

[Table("Organisation_Settlement_Item", Schema = "Setting")]
public class OrganisationSettlementItem : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }


    [ForeignKey("SettlementItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long SettlementItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual SettlementItem? SettlementItem { get; set; }

}
