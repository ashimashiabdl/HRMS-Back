using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Core.DTOs
{
    public class TaxDisketteWkDTO : BaseDTO
    {
        public long TaxDisketteId { get; set; }
        public string? PaymentType { get; set; }
        public long PaymentTypeId { get; set; }
        public long WorkplaceStatusId { get; set; }
        public long PurePaymentAmount { get; set; }
        public long ExceptionsSubjectToTheBudgetLawOf1404 { get; set; }
        public long CurrencyCode { get; set; }
        public long ExchangeRateOfCurrency { get; set; }
        public long GrossContinuousCashCurrentMonth { get; set; }
        public long ContinuousCashArearsNoTax { get; set; }
        public int HouseStatusId { get; set; }
        public long EmployeeHousingDeductionCurrentMonth { get; set; }
        public int CarStatusId { get; set; }
        public long EmployeeCarDeductionCurrentMonth { get; set; }
        public long ContinuousNonCashOtherBenefitsCost { get; set; }
        public long ContinuousNonCashArearsNoTax { get; set; }
        public long ConsultingFeesAndSimilar { get; set; }
        public long ResearchContracts { get; set; }
        public long Overtime { get; set; }
        public long TravelExpense { get; set; }
        public long MissionAllowance { get; set; }
        public long Karaneh { get; set; }
        public long BonusExceptYearEndServiceEndProductivity { get; set; }
        public long YearEndBonus { get; set; }
        public long AnnualEydi { get; set; }
        public long EndOfServiceBonus { get; set; }
        public long DismissalCompensation { get; set; }
        public long ServiceBuyback { get; set; }
        public long SeverancePay { get; set; }
        public long UnusedLeavePay { get; set; }
        public long NonContinuousCashCurrentMonth { get; set; }
        public long NonContinuousCashArearsNoTax { get; set; }
        public long NonContinuousNonCashCostCurrentMonth { get; set; }
        public long NonContinuousNonCashArearsNoTax { get; set; }
        public long MedicalInsuranceArticle137 { get; set; }
        public long LifeInsuranceArticle137 { get; set; }
        public long TeachingResearchFees { get; set; }
        public long OnCallPay { get; set; }
        public long WelfareMotivationProductivity { get; set; }
        public long WorkEffortExcludingWageSalaryBonus { get; set; }
    }
}


