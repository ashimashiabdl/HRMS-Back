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

namespace Hr.SystemSetting.Core.Entities;

[Table("Organisation_EmployeeType", Schema = "Setting")]
public class OrganisationEmployeeType : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
{
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("EmployeeTypeGroup")]
    public long EmployeeTypeGroupId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeTypeGroup? EmployeeTypeGroup { get; set; }
    /// <summary>
    /// جدول 7 نوع استخدام مالیات
    /// </summary>
    public long? TaxBaseTable7Id { get; set; }

    /// <summary>
    /// کد تفضیل نوع استخدام
    /// </summary>
    [MaxLength(50)]
    public string? EmployeeTypeFinancialCode { get; set; }

    [NotMapped()]
    private new string title { get; set; }
}
