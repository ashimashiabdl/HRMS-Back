using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs;

public class AbsenceRecordDTO : BaseDTO
{
    public long? OrganisationChartId { get; set; }
    public long? EmployeeId { get; set; }
    public string? Employee { get; set; }
    public bool IsMission { get; set; }

    public bool IsDaily { get; set; }
    public bool IsIllness { get; set; }
    public bool FirstApprove { get; set; }
    public bool SecondApprove { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}