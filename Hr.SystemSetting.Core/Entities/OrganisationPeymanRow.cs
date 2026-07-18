using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities;

[Table("Organisation_Peyman_Row", Schema = "Setting")]
public class OrganisationPeymanRow : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    /// <summary>
    /// کد ردیف پیمان
    /// </summary>
    [StringLength(32)]
    [Comment("کد ردیف پیمان")]
    public string? Code { get; set; }
}
