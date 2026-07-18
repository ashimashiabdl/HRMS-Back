using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("User_Default_Setting", Schema = "Identity")]
public class UserDefaultSetting : HR.SharedKernel.Data.BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("User")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }

    [ForeignKey("DefaultOrgan")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long? DefaultOrganId { get; set; }
    public virtual Organisation.Core.Entities.OrganisationChart? DefaultOrgan { get; set; }
    
    public long? DefaultWorkPlaceId { get; set; }
    public long? DefaultCostCenterId { get; set; }
    public long? DefaultOrganizationUnitId { get; set; }
    public long? DefaultPaymentPeriodId { get; set; }
    [NotMapped]
    private new string title { get; set; }

}
