using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Insurance_Diskette_Item", Schema = "Payroll")]
    public class InsuranceDisketteItem : SharedKernel.Data.BaseEntity , IignoreDateRangeValidation
    {
        [ForeignKey("InsuranceDiskette")]
        public long InsuranceDisketteId { get; set; }
        public virtual InsuranceDiskette? InsuranceDiskette { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

        [ForeignKey("PersonnelFunction")]
        public long PersonnelFunctionId { get; set; }
        public virtual PersonnelFunction? PersonnelFunction { get; set; }

        [ForeignKey("InterdictOrder")]
        public long InterdictOrderId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }

        [ForeignKey("Fiche")]
        public long FicheId { get; set; }
        public virtual Fiche? Fiche { get; set; }

        [ForeignKey("CostCenter")]
        public long CostCenterId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrganisationChart? CostCenter { get; set; }
        /// <summary>
        /// دستمزد روزانه
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? DailyPayment { get; set; }
        /// <summary>
        /// دستمزد ماهانه
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? MonthPayment { get; set; }
        /// <summary>
        /// مشمول پرداخت بیمه اما نه در ماه پرداختی جاری
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? PaymentInsuranceCoveredNotInMonthPayment { get; set; }
        /// <summary>
        /// مزایای ماهانه مشمول
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? PaymentInsuranceCovered { get; set; }
        /// <summary>
        /// جمع مبلغ بیمه پرداختی - جمع کل حقوق و مزایا مشمول
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? TotalInsurancePayment { get; set; }
        /// <summary>
        /// مبلغ حق بیمه شخص بیمه شده
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? PersonnelInsuranceAmount { get; set; }
        /// <summary>
        /// حق بیمه سهم کارفرما
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? EmployerInsuranceAmount { get; set; }
        /// <summary>
        /// حق بیمه بیکاری
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long? UnEmployedInsuranceAmount { get; set; }
        // [ForeignKey("EmployeeStatus")]
        // public long EmployeeStatusId { get; set; }
        // public virtual EmployeeStatus? EmployeeStatus { get; set; }
        [StringLength(128)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? InsuranceNo { get; set; }
        [StringLength(128)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? InsuranceWorkShopDisc { get; set; }
        [StringLength(128)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? PaymanRowDisc { get; set; }
        //public long? FinalFicheId { get; set; }
        //public long? InnerFicheId { get; set; }
        /// <summary>
        /// کد کارگاه
        /// </summary>
        [StringLength(10)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_ID { get; set; }
        /// <summary>
        /// شماره فهرست
        /// </summary>
        [StringLength(12)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_LISTNO { get; set; }  
        /// <summary>
        /// شماره بیمه
        /// </summary>
        [StringLength(12)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_ID1 { get; set; } 
        /// <summary>
        /// نام
        /// </summary>
        [StringLength(100)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_FNAME { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [StringLength(100)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_LNAME { get; set; } 
        /// <summary>
        /// نام پدر
        /// </summary>
        [StringLength(100)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_DNAME { get; set; } 
        /// <summary>
        /// محل صدور
        /// </summary>
        [StringLength(100)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_IDPLC { get; set; } 
        /// <summary>
        /// تاریخ صدور
        /// </summary>
        [StringLength(8)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_IDATE { get; set; }  
        /// <summary>
        /// جنسیت
        /// </summary>
        [StringLength(3)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_SEX { get; set; } 
        /// <summary>
        /// شرح شغل
        /// </summary>
        [StringLength(100)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_OCP { get; set; }  
        /// <summary>
        /// تاریخ شروع به کار
        /// </summary>
        [StringLength(8)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_SDATE { get; set; } 
        /// <summary>
        /// تاریخ ترک کار
        /// </summary>
        [StringLength(8)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_EDATE { get; set; } 
        /// <summary>
        /// ملیت
        /// </summary>
        [StringLength(10)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_NAT { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        [StringLength(8)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_BDATE { get; set; }  
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        [StringLength(15)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_IDNO { get; set; }  
        /// <summary>
        /// کد ملی
        /// </summary>
        [StringLength(10)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? PER_NATCOD { get; set; }  
        /// <summary>
        /// کد شغل
        /// </summary>
        [StringLength(6)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? DSW_JOB { get; set; }
        /// <summary>
        /// سال عملکرد
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_YY { get; set; }
        /// <summary>
        /// ماه عملکرد
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_MM { get; set; }
        /// <summary>
        /// تعداد روز های کارکرد
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_DD { get; set; }
        /// <summary>
        /// دستمزد روزانه
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_ROOZ { get; set; }
        /// <summary>
        /// دستمزد ماهانه
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_MAH { get; set; }
        /// <summary>
        /// مزایای ماهانه
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_MAZ { get; set; }
        /// <summary>
        /// جمع دستمزد و مزایای ماهانه مشمول
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_MASH { get; set; }
        /// <summary>
        /// جمع کل دستمزد و مزایای ماهانه
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_TOTL { get; set; }
        /// <summary>
        /// حق بیمه سهم بیمه شده
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_BIME { get; set; }
        /// <summary>
        /// نرخ پورسانتاژ
        /// </summary>
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long DSW_PRATE { get; set; }
        /// <summary>
        /// پایه سنوات
        /// </summary>
        public long DSW_INC { get; set; }
        /// <summary>
        /// حق تاهل در فیش
        /// </summary>
        public long DSW_SPOUSE { get; set; }


        [NotMapped]
        private new string title { get; set; }

    }
}
