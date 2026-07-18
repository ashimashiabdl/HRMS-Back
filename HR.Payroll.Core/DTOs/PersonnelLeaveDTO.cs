using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.DTOs;

public class PersonnelLeaveDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long PaymentPeriodId { get; set; }
    public string? PaymentPeriod { get; set; }

    public long EmployeeId { get; set; }
    public string? Employee { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonelCode { get; set; }
    public string? NationalNo { get; set; }

    public long LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }
  
    public long? PersonnelFunctionExcelFileId { get; set; }
    public decimal Day { get; set; }
    public decimal Hour { get; set; }
    public decimal Minute { get; set; }

    [MaxLength(150)]
    public string? Description { get; set; }

}


