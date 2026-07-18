using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class InsuranceDisketteItemDTO : BaseDTO
    {
        public long InsuranceDisketteId { get; set; }
        public  string? InsuranceDiskette { get; set; }
        public string? ActiveName { get; set; }
        public string? NationalNo { get; set; }
        public long EmployeeId { get; set; }
        public  string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? IdentityNo { get; set; }
        public long PersonnelFunctionId { get; set; }
        public  string? PersonnelFunction { get; set; }

    
        public long InterdictOrderId { get; set; }
        public  string? InterdictOrder { get; set; }

     
        public long FicheId { get; set; }
        public  string? Fiche { get; set; }

    
        public long CostCenterId { get; set; }
        public  string? CostCenter { get; set; }
        /// <summary>
        /// دستمزد روزانه
        /// </summary>
        public long? DailyPayment { get; set; }
        /// <summary>
        /// دستمزد ماهانه
        /// </summary>
        public long? MonthPayment { get; set; }
        /// <summary>
        /// مشمول پرداخت بیمه اما نه در ماه پرداختی جاری
        /// </summary>
        public long? PaymentInsuranceCoveredNotInMonthPayment { get; set; }
        /// <summary>
        /// مزایای ماهانه مشمول
        /// </summary>
        public long? PaymentInsuranceCovered { get; set; }
        /// <summary>
        /// جمع مبلغ بیمه پرداختی - جمع کل حقوق و مزایا مشمول
        /// </summary>
        public long? TotalInsurancePayment { get; set; }
        /// <summary>
        /// مبلغ حق بیمه شخص بیمه شده
        /// </summary>
        public long? PersonnelInsuranceAmount { get; set; }
        /// <summary>
        /// حق بیمه سهم کارفرما
        /// </summary>
        public long? EmployerInsuranceAmount { get; set; }
        /// <summary>
        /// حق بیمه بیکاری
        /// </summary>
        public long? UnEmployedInsuranceAmount { get; set; }
        // [ForeignKey("EmployeeStatus")]
        // public long EmployeeStatusId { get; set; }
        // public  EmployeeStatus? EmployeeStatus { get; set; }
       
        public string? InsuranceNo { get; set; }
      
        public string? InsuranceWorkShopDisc { get; set; }
    
        public string? PaymanRowDisc { get; set; }
        //public long? FinalFicheId { get; set; }
        //public long? InnerFicheId { get; set; }
        /// <summary>
        /// کد کارگاه
        /// </summary>
    
        public string? DSW_ID { get; set; }
        /// <summary>
        /// شماره فهرست
        /// </summary>
   
        public string? DSW_LISTNO { get; set; }
        /// <summary>
        /// شماره بیمه
        /// </summary>
   
        public string? DSW_ID1 { get; set; }
        /// <summary>
        /// نام
        /// </summary>
    
        public string? DSW_FNAME { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
    
        public string? DSW_LNAME { get; set; }
        /// <summary>
        /// نام پدر
        /// </summary>
    
        public string? DSW_DNAME { get; set; }
        /// <summary>
        /// محل صدور
        /// </summary>
   
        public string? DSW_IDPLC { get; set; }
        /// <summary>
        /// تاریخ صدور
        /// </summary>
     
        public string? DSW_IDATE { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
  
        public string? DSW_SEX { get; set; }
        /// <summary>
        /// شرح شغل
        /// </summary>
    
        public string? DSW_OCP { get; set; }
        /// <summary>
        /// تاریخ شروع به کار
        /// </summary>
   
        public string? DSW_SDATE { get; set; }
        /// <summary>
        /// تاریخ ترک کار
        /// </summary>
   
        public string? DSW_EDATE { get; set; }
        /// <summary>
        /// ملیت
        /// </summary>
   
        public string? DSW_NAT { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
    
        public string? DSW_BDATE { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>

        public string? DSW_IDNO { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
   
        public string? PER_NATCOD { get; set; }
        /// <summary>
        /// کد شغل
        /// </summary>
    
        public string? DSW_JOB { get; set; }
        /// <summary>
        /// سال عملکرد
        /// </summary>
        public long DSW_YY { get; set; }
        /// <summary>
        /// ماه عملکرد
        /// </summary>
        public long DSW_MM { get; set; }
        /// <summary>
        /// تعداد روز های کارکرد
        /// </summary>
        public long DSW_DD { get; set; }
        /// <summary>
        /// دستمزد روزانه
        /// </summary>
        public long DSW_ROOZ { get; set; }
        /// <summary>
        /// دستمزد ماهانه
        /// </summary>
        public long DSW_MAH { get; set; }
        /// <summary>
        /// مزایای ماهانه
        /// </summary>
        public long DSW_MAZ { get; set; }
        /// <summary>
        /// جمع دستمزد و مزایای ماهانه مشمول
        /// </summary>
        public long DSW_MASH { get; set; }
        /// <summary>
        /// جمع کل دستمزد و مزایای ماهانه
        /// </summary>
        public long DSW_TOTL { get; set; }
        /// <summary>
        /// حق بیمه سهم بیمه شده
        /// </summary>
        public long DSW_BIME { get; set; }
        /// <summary>
        /// نرخ پورسانتاژ
        /// </summary>
        public long DSW_PRATE { get; set; }
        /// <summary>
        /// پایه سنوات
        /// </summary>
        public long DSW_INC { get; set; }
        /// <summary>
        /// حق تاهل در فیش
        /// </summary>
        public long DSW_SPOUSE { get; set; }


    }
}
