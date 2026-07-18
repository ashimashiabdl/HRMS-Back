using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class FicheDTO : BaseDTO
    {
        public long EmployeeId { get; set; }
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(10)]
        public string? NationalNo { get; set; }
        [StringLength(50)]
        public string? PersonelCode { get; set; }
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long? PeymanRowId { get; set; }
        public string? PeymanRow { get; set; }
        public string? Description { get; set; }
        public short? Serial { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public string? BankAccountNo { get; set; }
        public bool HasExistingFiche { get; set; }
        public long? ExsitingFicheId { get; set; }
        public long TotalAmount { get; set; }
        /// <summary>
        /// جمع کل دستمزد و مزایای ماهانه برای دیسکت بیمه (بدون اقلام حق اولاد)
        /// </summary>
        public long InsuranceTotal_DSW { get; set; }
        /// <summary>
        /// سهم اقلام فرعی از کسورات (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public long SecondaryDeductedAmount { get; set; }
        /// <summary>
        /// سهم اقلام فرعی از جمع پرداخت (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public long SecondaryTotalAmount { get; set; }
        /// <summary>
        /// سهم خالص اقلام فرعی (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public long SecondaryPurePaymentAmount { get; set; }

        public long FicheStatusId { get; set; }
        public string? FicheStatus { get; set; }
        public long FicheTypeId { get; set; }
        public string? FicheType { get; set; }
        public long PersonnelFunctionId { get; set; }
        public PersonnelFunctionDTO? PersonnelFunction { get; set; }
        public long DeductedAmount { get; set; }
        public long InterdictOrderId { get; set; }
        public string? InterdictOrder { get; set; }
        public string? InsuranceNo { get; set; }
        public int? InterdictOrderSerial { get; set; }
        
        public long PurePaymentAmount { get; set; }
        public long PaymentTax { get; set; }
        public long PaymentRetiredCovered { get; set; }
        public long? UnEmploymentAmount { get; set; }
        public bool? IsActiveInsurance { get; set; }
        public long MonthJobBenefit { get; set; }
        public long PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
        public long? DailyFunctionAmount { get; set; }
        public long? BillSumItemsAmount { get; set; }
        public long? BillItemsWage { get; set; }
        public long NotNetCurrentMonthOverTimePayment { get; set; }
        public long SumOfDelayedCountiniousPaymentInCurrentMonth { get; set; }
        public long HouseAmount { get; set; }
        public long CarAmount { get; set; }

        /// <summary>
        /// حق تاهل در فهرست بیمه
        /// </summary>
        public long SpouseAmount { get; set; }
        /// <summary>
        /// سنوات در فهرست بیمه
        /// </summary>
        public long IncAmount { get; set; }

        /// <summary>
        /// جمع کسورات با احتساب قلم‌های فرعی
        /// </summary>
        public long DeductionSumAmount { get; set; }
        /// <summary>
        /// سهم اقلام فرعی از جمع کسورات (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public long SecondaryDeductionSumAmount { get; set; }
        /// <summary>
        /// جمع کل پرداختی‌ها با احتساب قلم‌های فرعی
        /// </summary>
        public double TotalPaymentAmount { get; set; }
        /// <summary>
        /// جمع کل پرداختی ها برای دیسکت بیمه (بدون اقلام حق اولاد)
        /// </summary>
        public double InsuranceTotalPaymentAmount_DSW { get; set; }
        /// <summary>
        /// سهم اقلام فرعی از جمع پرداخت (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public double SecondaryTotalPaymentAmount { get; set; }
        /// <summary>
        /// خالص پرداختی با احتساب قلم‌های فرعی
        /// </summary>
        public double PayableAmount { get; set; }
        /// <summary>
        /// سهم خالص اقلام فرعی (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public double SecondaryPayableAmount { get; set; }
        /// <summary>
        /// جمع قلم های مشمول مالیات
        /// </summary>
        public double TaxCoveredSum { get; set; }
        public double CurrentTax { get; set; }
        public double PaymentInsuranceCovered { get; set; }
        public long RelatedTaxWageItemId { get; set; }
        /// <summary>
        /// خالص پرداختی به حروف
        /// </summary>
        public string? PayableAmountSTR { get; set; }
        public long? BillBazkharidSanavatAmount { get; set; }
        /// <summary>
        /// سهم اقلام فرعی از بازخرید سنوات (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public long? SecondaryBillBazkharidSanavatAmount { get; set; }
        public string? SanavatFormulaFriendlyText { get; set; }
        public Dictionary<string, string?>? SanavatVariableFriendlyList { get; set; }
        public string? SanavatFormulaText { get; set; }
        public FormulaExecutionTree? SanavatFormulaTreeParser { get; set; }
        public string? SanavatFormulaHelpDesc { get; set; }
        public string? Sanavatformularowmessage { get; set; }
        public bool SanavatIsRowSuccess { get; set; }
        public int SanavatSuccessRunTimeInmilliseconds { get; set; }
        public long? BillEydiOpadashAmount { get; set; }
        /// <summary>
        /// سهم اقلام فرعی از عیدی (محاسباتی — ذخیره نمی‌شود)
        /// </summary>
        public long? SecondaryBillEydiOpadashAmount { get; set; }
        public string? EydiFormulaFriendlyText { get; set; }
        public Dictionary<string, string?>? EydiVariableFriendlyList { get; set; }
        public string? EydiFormulaText { get; set; }
        public FormulaExecutionTree? EydiFormulaTreeParser { get; set; }
        public string? EydiFormulaHelpDesc { get; set; }
        public string? EydiFormula { get; set; }
        public string? SanavatFormula { get; set; }
        public bool EydiIsRowSuccess { get; set; }
        public int EydiSuccessRunTimeInmilliseconds { get; set; }
        public string? Eydiformularowmessage { get; set; }
        public List<OrganisationEmployeeTypeFicheItemDTO>? OrganisationEmployeeTypeFicheItem { set; get; }
        public List<FicheItemDTO>? DeductionFicheItems { set; get; }
        public List<FicheItemDTO>? PaymentFicheItems { set; get; }
        public List<PersonnelLoanPayment>? PersonnelLoanPayments { set; get; }
        public List<PersonnelLoanPaymentDTO>? PersonnelLoanPaymentDTOs { set; get; }


        public List<PersonnelPayment> PersonnelPayments = new List<PersonnelPayment>();
        public List<EmployeeDeductionPayment> EmployeeDeductionPayments = new List<EmployeeDeductionPayment>();
        public List<EmployeeDeductionPaymentDTO> EmployeeDeductionPaymentDTOs = new List<EmployeeDeductionPaymentDTO>();
        public List<FicheLeaveItemDTO> FicheLeaveItemDTOs { get; set; } = new List<FicheLeaveItemDTO>();

        /// <summary>
        /// جمع مزایای مستمر نقدی و مشمول مالیات
        /// </summary>
        public long? SumCashTaxCoveredAndCountinious { get; set; }
        /// <summary>
        /// جمع مزایای مستمر غیر نقدی و مشمول مالیات
        /// </summary>
        public long? SumNonCashTaxCoveredAndCountinious { get; set; }
        /// <summary>
        /// جمع مزایای غیر مستمر غیر نقدی و مشمول مالیات
        /// </summary>
        public long? SumNonCashTaxCoveredAndNotCountinious { get; set; }
        /// <summary>
        /// جمع مزایای غیر مستمر نقدی و مشمول مالیات
        /// </summary>
        public long? SumCashTaxCoveredAndNotCountinious { get; set; }
    }
}
