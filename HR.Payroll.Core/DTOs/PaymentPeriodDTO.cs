using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs;

public class PaymentPeriodDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public int ShamsiYear { get; set; }
    public int ShamsiMonth { get; set; }
    public int PeriodDays { get; set; }
    public bool IsClosed { get; set; }
    public bool UpdatedOnSite { get; set; }
}
