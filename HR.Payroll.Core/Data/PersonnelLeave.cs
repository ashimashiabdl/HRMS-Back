using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;
/// <summary>
/// اطلاعات این جدول ماهانه مثل کارکرد وارد می شود
/// </summary>

[Table("Personnel_Leave", Schema = "Payroll")]
public class PersonnelLeave : BaseEntity,IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("PaymentPeriod")]
    public long PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }

    [ForeignKey("LeaveType")]
    public long LeaveTypeId { get; set; }
    public virtual LeaveType? LeaveType { get; set; }
    [ForeignKey("PersonnelFunctionExcelFile")]
    public long? PersonnelFunctionExcelFileId { get; set; }
    public virtual PersonnelFunctionExcelFile? PersonnelFunctionExcelFile { get; set; }
    public decimal Day { get; set; }
    public decimal Hour { get; set; }
    public decimal Minute { get; set; }

    [MaxLength(150)]
    public string? Description { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
