using HR.Order.Core.Data;
using HR.Payroll.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BaseFicheCalculationRequest
    {
        public BaseFicheCalculationRequest(long EmployeeId, List<PaymentPeriod> lastPeriodsOfCurrentYear, PaymentPeriod PaymentPeriod, RecruitOrder RecruitOrder, PersonnelFunction PersonnelFunction, InterdictOrder InterdictOrder)
        {
            _paymentPeriod = PaymentPeriod;
            _personnelFunction = PersonnelFunction;
            _interdictOrder = InterdictOrder;
            _recruitOrder = RecruitOrder;
            _employeeId = EmployeeId;
            LastPeriodsOfCurrentYear = lastPeriodsOfCurrentYear;
        }
        public PaymentPeriod _paymentPeriod { get; set; }
        public PersonnelFunction _personnelFunction { get; set; }
        public InterdictOrder _interdictOrder { get; set; }
        public RecruitOrder _recruitOrder { get; set; }
        public List<PaymentPeriod> LastPeriodsOfCurrentYear { get; set; }
        public long _employeeId { get; set; }
    }
}
