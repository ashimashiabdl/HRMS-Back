using HR.Order.Core.Data;
using HR.Payroll.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class TaxCalculationRequest : BaseFicheCalculationRequest
    {
        public TaxCalculationRequest(long EmployeeId, PaymentPeriod PaymentPeriod, RecruitOrder RecruitOrder, List<PaymentPeriod> LastPeriodsOfCurrentYear, PersonnelFunction PersonnelFunction, InterdictOrder InterdictOrder) : base(EmployeeId, LastPeriodsOfCurrentYear, PaymentPeriod, RecruitOrder, PersonnelFunction, InterdictOrder)
        {
            //public List<PaymentPeriod> LastPeriodsOfCurrentYear { get; set; }
        }
        public List<OrganisationEmployeeTypeFicheItemDTO>? OrganisationEmployeeTypeFicheItem { set; get; }

    }
}
