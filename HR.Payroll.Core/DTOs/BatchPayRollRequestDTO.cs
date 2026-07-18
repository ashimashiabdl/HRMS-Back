using HR.Identity.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs;

public class BatchPayRollRequestDTO : BaseDTO
{
    public long UserId { get; set; }
    


    public long? OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long? RequestStateId { get; set; }
    public string? RequestState { get; set; }
    public long? RequestTypeId { get; set; }
    public string? RequestType { get; set; }
    public long? PaymentPeriodId { get; set; }
    public string? PaymentPeriod { get; set; }
    [StringLength(256)]
    public string? Username { get; set; }
    [StringLength(4096)]
    public string? RequsetDescription { get; set; }
    public DateTime? LastPoolingTime { get; set; }
    public DateTime? FinishDateTime { get; set; }
    public bool? IsDone { get; set; }
    public int? EmployeeCount { get; set; }
    public int? SuccessCount { get; set; }
    public int? PoolingEmployeeId { get; set; }
}
