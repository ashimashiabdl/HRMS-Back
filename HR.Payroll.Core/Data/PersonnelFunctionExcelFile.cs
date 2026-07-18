using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("PersonnelFunction_Excel_File", Schema = "Payroll")]
public class PersonnelFunctionExcelFile : BaseEntity, IOrganisationChartId, IbaseFile
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("AspNetUsers")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long AspNetUsersId { get; set; }
    public virtual AspNetUsers? AspNetUsers { get; set; }
    /// <summary>
    /// نوع استخدام
    /// </summary>
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public virtual EmployeeType? EmployeeType { get; set; }
    [ForeignKey("PaymentPeriod")]
    public long PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }

    [StringLength(512)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    
    public string? Extension { get; set; }
    public Guid? UniqueId { get; set; }
    public long Size { get; set; }
    public byte[] Content { get; set; } = null!;
    [StringLength(512)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? MimeType { get; set; }
}
