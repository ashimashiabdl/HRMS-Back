using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Employee.Core.DTOs;

public class AttendanceDTO : BaseDTO
{
    public long? OrganisationChartId { get; set; }
    public long EmployeeId { get; set; }
    public string? Employee { get; set; }
    public int InOutType { get; set; }
    public string InOutTypeDesc
    {
        get
        {
            if (InOutType == 237)
            {
                return "ورود";
            }

            if (InOutType == 226)
            {
                return "خروج";
            }
            return "";
        }
    }
    [Column(TypeName = "datetime")]
    public DateTime? DateTime { get; set; }
    [StringLength(50)]
    public string? DeviceName { get; set; }
    [StringLength(20)]
    public string? InOutCard { get; set; }
}
