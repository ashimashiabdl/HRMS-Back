

namespace HR.SharedKernel.Share;

public class Enums
{
    public enum BankDiskItemFieldType
    {
        /// <summary>
        /// شماره حساب
        /// </summary>
        AcountNo = 21322,
        /// <summary>
        /// مبلغ حقوق
        /// </summary>
        Salary = 21323,
        /// <summary>
        /// کد شعبه
        /// </summary>
        BankBranchNo = 21324,
        /// <summary>
        /// فیلد یکتا ( بر اساس شبا ) کد بانک
        /// </summary>
        UniqueField = 21583,
        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        NameAndLastName = 21584,
        /// <summary>
        /// شرح واریزی
        /// </summary>
        DescriptionOfTheDeposit = 21585,
        /// <summary>
        /// مقدار ثابت
        /// </summary>
        StaticText = 21586,
        /// <summary>
        /// شماره شبا
        /// </summary>
        Sheba = 21588
    }


    /// <summary>
    /// نوع فایل های پیوست درخواست های گروهی
    /// </summary>
    public enum BatchRequestFileType
    {
        /// <summary>
        /// فایل PDF تجمعی آرشیو قالب دار
        /// </summary>
        PdfbyteArrayCombined = 11222,
        /// <summary>
        /// فایل PDF تجمعی آرشیو خام
        /// </summary>
        PdfrawByteArrayCombined = 11218,
        /// <summary>
        /// اکسل بخش استخدامی اختصاصی درخواست گروهی
        /// </summary>
        RecrutSectionExcel = 11221,
        /// <summary>
        /// اکسل ضرائب ( عوامل ) اختصاصی درخواست گروهی
        /// </summary>
        CoefficientsExcel = 11220,
        /// <summary>
        /// اکسل عوامل حقوقی اختصاصی درخواست گروهی
        /// </summary>
        WageItemsExcel = 11219,
        /// <summary>
        /// اکسل تاریخ اجرا و انقضای احکام درخواست گروهی
        /// </summary>
        OrderDatesExcel = 11223
    }

    public enum CheckFormulaType
    {
        /// <summary>
        /// هشدار دهنده
        /// </summary>
        Warning = 599,
        /// <summary>
        /// بازدارنده
        /// </summary>
        Deterrent = 598
    }
    public enum CheckingTime
    {
        /// <summary>
        /// قبل از محاسبه فرمول
        /// </summary>
        BeforeMainCalculation = 1164,
        /// <summary>
        /// بعد از محاسبه فرمول
        /// </summary>
        AfterMainCalculation = 1165
    }
    public enum SqlFunctionType
    {
        /// <summary>
        /// کوئری پویا
        /// </summary>
        DynamicQuery = 11210,
        /// <summary>
        /// تابع موجود در پایگاه داده ها
        /// </summary>
        SqlFunction = 11211
    }


    public enum TableType
    {
        /// <summary>
        /// بازه ای
        /// </summary>
        Continuous = 578,
        /// <summary>
        /// غیر بازه ای ( گسسته )
        /// </summary>
        Discrete = 577



    }
    /// <summary>
    /// روش محاسبه عوامل حقوقی و ضرائب حکم	
    /// </summary>
    public enum EnterTypeId
    {
        /// <summary>
        /// عدد ثابت
        /// </summary>
        fixValue = 414,
        /// <summary>
        /// منطقی 
        /// </summary>
        boolean = 415,
        /// <summary>
        /// کاهش مقداری
        /// </summary>
        AmountDecrease = 412,
        /// <summary>
        /// طبق فرمول
        /// </summary>
        UseFormula = 413,
        /// <summary>
        /// کاهش درصدی
        /// </summary>
        PercentDecrease = 410,
        /// <summary>
        /// افزایش مقداری
        /// </summary>
        AmountIncrease = 411,
        /// <summary>
        /// برابر با مقدار ورودی
        /// </summary>
        EqualToinput = 407,
        /// <summary>
        /// مطابق آخرین حکم
        /// </summary>
        EqualTolastRec = 408,
        /// <summary>
        /// افزایش درصدی
        /// </summary>
        PercentIncrease = 409
    }
    public enum PayRollEnterTypeId
    {
        /// <summary>
        /// طبق فرمول
        /// </summary>
        UseFormula = 11231,
        /// <summary>
        /// مطابق آخرین حکم
        /// </summary>
        EqualTolastRec = 11230,
        /// <summary>
        /// عدد ثابت
        /// </summary>
        fixValue = 11238,
    }
    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum OrderLevel
    {
        /// <summary>
        /// استخدامی
        /// </summary>
        Recruit = 11207,
        /// <summary>
        /// حقوقی
        /// </summary>
        interdict = 11208
    }

    public enum IssueType
    {
        /// <summary>
        /// عادی
        /// </summary>
        Normal = 1206,
        /// <summary>
        /// اصلاحیه
        /// </summary>
        Correction = 1207,
        /// <summary>
        /// لغو
        /// </summary>
        Cancelation = 1208,
        /// <summary>
        /// عادی معوقه
        /// </summary>
        NormalOutDate = 11204
    }
    /// <summary>
    /// وضعیت حکم
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// پیش نویس 
        /// </summary>
        Draft = 3,
        /// <summary>
        /// تایید شده نهایی
        /// </summary>
        FinalAprove = 7,
        /// <summary>
        /// احکام ماقبل
        /// </summary>
        LastOrder = 8,
        /// <summary>
        /// آخرین حکم
        /// </summary>
        FinalOrder = 9,
        /// <summary>
        /// پیش نویس رد شده
        /// </summary>
        RejectedOrder = 10,
        /// <summary>
        /// درحال بررسی کارگزینی
        /// </summary>
        Pending = 11,
        /// <summary>
        /// حکم حذف شده
        /// </summary>
        LogicalDeleted = 12,
        /// <summary>
        /// دارای اصلاحیه
        /// </summary>
        CorrectedOrder = 13,
        /// <summary>
        /// آخرين در محل پرداخت قبل
        /// </summary>
        LastInPayLocation = 14,
        /// <summary>
        /// حکم لغو شده
        /// </summary>
        CanceledOrder = 15
    }
    public enum BatchRequestType
    {
        /// <summary>
        /// صدور حکم گروهی
        /// </summary>
        BatchOrderIssue = 931,
        /// <summary>
        /// چاپ گروهی
        /// </summary>
        BatchPrint = 933,
        /// <summary>
        /// ارسال به حقوقی
        /// </summary>
        SendToPayRoll = 934,
        /// <summary>
        /// ارسال به کارتابل گروهی
        /// </summary>
        SendToCartable = 935,
        /// <summary>
        /// اصلاحیه گروهی
        /// </summary>
        BatchCorrection = 936,
        /// <summary>
        /// تایید گروهی احکام پیش نویس
        /// </summary>
        ApproveDraftOrders = 932
    }

    /// <summary>
    /// وضعیت آرشیو درخواست گروهی
    /// </summary>
    public enum BatchRequestArchiveState
    {
        /// <summary>
        /// اتمام حلقه آرشیو	
        /// </summary>
        EndLoop = 11213,
        /// <summary>
        /// درخواست آرشیو مجدد
        /// </summary>
        ReArchive = 11214,
        /// <summary>
        /// آرشیو اولیه انجام شود	
        /// </summary>
        InitialArchive = 11215,
        /// <summary>
        /// درخواست نیاز به آرشیو ندارد
        /// </summary>
        IgnoreArchive = 11216,
        /// <summary>
        /// در حال آرشیو
        /// </summary>
        InProgress = 11217
    }

    public enum BatchRequestState
    {
        /// <summary>
        /// ایجاد اولیه
        /// </summary>
        Initial = 11212,
        /// <summary>
        /// پایان حلقه
        /// </summary>
        EndLoop = 887,
        /// <summary>
        /// اجرای مجدد
        /// </summary>
        Resume = 888,
        /// <summary>
        /// انصراف توسط کاربر
        /// </summary>
        CancelbyUser = 889,
        /// <summary>
        /// در حال اجرا
        /// </summary>
        Runing = 886

    }
    /// <summary>
    /// مورد استفاده در بررسی دسترسی ها اتریبیوت
    /// </summary>
    public enum HelperObjectType
    {
        /// <summary>
        /// مقدار ارسالی خود شناسه کارمندی است
        /// </summary>
        IsEmployeeId = 11226,
        /// <summary>
        /// شناسه کارمندی در شی ارسالی وجود دارد ( Body )
        /// </summary>
        ExtractFromBody = 11227,
        /// <summary>
        /// شناسه ارسالی جدول وابسه است و با استفاده از شما و نام جدول EmployeeId استخراج شود
        /// </summary>
        IsDependantTable = 11228,
        /// <summary>
        /// از QueryString خوانده شود
        /// </summary>
        ExtractFromQueryString = 11229
    }

    /// <summary>
    /// عملیات های گردش کار
    /// </summary>
    public enum WorkFlowActions
    {
        /// <summary>
        /// تایید
        /// </summary>
        Approve = 1,
        /// <summary>
        /// رد — بازگرداندن به مرحله قبلی گردش کار
        /// </summary>
        Reject = 2,
        /// <summary>
        /// امضا
        /// </summary>
        Sign = 3,
        /// <summary>
        /// رد کلی — پایان ناموفق چرخه گردش کار
        /// </summary>
        FullReject = 4,
        /// <summary>
        /// تایید از طرف
        /// </summary>
        ApproveOnBehalf = 5
    }
    /// <summary>
    /// وضعیت قلم حقوقی
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// کسورات
        /// </summary>
        Deduction = 11233,
        /// <summary>
        /// پرداختی
        /// </summary>
        Payment = 11232
    }
    /// <summary>
    /// روش محاسبه عیدی و سنوات ( 30248 )
    /// </summary>
    public enum EydiSanavatCalculationType
    {
        /// <summary>
        /// ماهانه
        /// </summary>
        Monthly = 11244,
        /// <summary>
        /// آخر سال کاری یا موقع تسویه حساب
        /// </summary>
        YearlYOrTermination = 11243
    }

    /// <summary>
    /// منشا قلم فیش حقوقی ( 40250 )
    /// </summary>
    public enum OriginOfFicheItem
    {
        /// <summary>
        /// مرکز هزینه
        /// </summary>
        CostCenter = 21251,
        /// <summary>
        /// عوامل اختصاصی فرد
        /// </summary>
        PersonnelFicheItem = 21250,
        /// <summary>
        /// نوع استخدام
        /// </summary>
        EmploymentType = 21249
    }
    /// <summary>
    /// وضعیت فیش حقوقی
    /// </summary>
    public enum FicheStatus
    {
        /// <summary>
        /// محاسبه اولیه
        /// </summary>
        Initial = 1,
        /// <summary>
        /// پرداخت شده
        /// </summary>
        Payed = 2,
    }
    /// <summary>
    /// نوع فیش
    /// </summary>
    public enum FicheType
    {
        /// <summary>
        /// عادی
        /// </summary>
        Normal = 1,
        /// <summary>
        /// معوقه
        /// </summary>
        Arears = 3
    }

    /// <summary>
    /// وضعیت رکورد درخواست صدور گروهی حقوق و دستمزد
    /// </summary>
    public enum BatchPayRollRequestState
    {
        /// <summary>
        /// ایجاد اولیه
        /// </summary>
        Initial = 21254,
        /// <summary>
        /// پایان حلقه
        /// </summary>
        EndLoop = 21255,
        /// <summary>
        /// تلاش مجدد
        /// </summary>
        TryAgain = 21256,
        /// <summary>
        /// انصراف کاربر
        /// </summary>
        CancelByUser = 21257,
        /// <summary>
        /// درحال اجرا
        /// </summary>
        Running = 21258,
        /// <summary>
        /// حذف شده
        /// </summary>
        Deleted = 21261
    }

    /// <summary>
    /// نوع درخواست صدور گروهی حقوق و دستمزد 
    /// </summary>
    public enum BatchPayRollRequestType
    {
        /// <summary>
        /// صدور فیش حقوقی عادی گروهی	
        /// </summary>
        NormalFicheCalculation = 21259,
        /// <summary>
        /// صدور معوقه گروهی
        /// </summary>
        ArearsFicheCalculation = 21260,
        /// <summary>
        /// تهیه دیسکت بانک گروهی
        /// </summary>
        BankDisketteCalculation = 21325,
        /// <summary>
        /// تهیه دیسکت بیمه
        /// </summary>
        InsuranceDisketteCalculation = 21326,
        /// <summary>
        /// تهیه دیسکت مالیات ( دارایی )
        /// </summary>
        TaxDisketteCalculation = 21327
    }

    /// <summary>
    /// وضعیت درخواست تسویه حساب گروهی
    /// </summary>
    public enum BatchSettlementRequestState
    {
        /// <summary>
        /// ایجاد اولیه
        /// </summary>
        Initial = 21350,
        /// <summary>
        /// پایان حلقه
        /// </summary>
        EndLoop = 21351,
        /// <summary>
        /// تلاش مجدد
        /// </summary>
        TryAgain = 21352,
        /// <summary>
        /// انصراف کاربر
        /// </summary>
        CancelByUser = 21353,
        /// <summary>
        /// درحال اجرا
        /// </summary>
        Running = 21354,
        /// <summary>
        /// حذف شده
        /// </summary>
        Deleted = 21355
    }

    /// <summary>
    /// نوع درخواست تسویه حساب گروهی
    /// </summary>
    public enum BatchSettlementRequestType
    {
        /// <summary>
        /// صدور تسویه حساب گروهی
        /// </summary>
        NormalSettlement = 21356
    }

    /// <summary>
    /// وضعیت دیسکت بانک
    /// </summary>
    public enum BankDisketteStatus
    {
        /// <summary>
        /// ایجاد اولیه
        /// </summary>
        Initial = 21328,
        /// <summary>
        /// اتمام محاسبه
        /// </summary>
        CalculationFinished = 21329,
        /// <summary>
        /// پرداخت شده
        /// </summary>
        Payed = 21330,
        /// <summary>
        /// حذف شده
        /// </summary>
        Deleted = 21331
    }

    /// <summary>
    /// وضعیت دیسکت ( فهرست ) بیمه
    /// </summary>
    public enum InsuranceDisketteStatus
    {
        /// <summary>
        /// ایجاد اولیه
        /// </summary>
        Initial = 21335,
        /// <summary>
        /// اتمام محاسبه
        /// </summary>
        CalculationFinished = 21336,
        /// <summary>
        /// حذف شده
        /// </summary>
        Deleted = 21337,
        /// <summary>
        /// فهرست گرفته و پرداخت شده	
        /// </summary>
        Payed = 21338,
        /// <summary>
        /// در حال اجرا
        /// </summary>
        Running = 21377
    }
    /// <summary>
    /// وضعیت دیسکت مالیات
    /// </summary>
    public enum TaxDisketteStatus
    {
        /// <summary>
        /// ایجاد اولیه
        /// </summary>
        Initial = 21366,
        /// <summary>
        /// اتمام محاسبه
        /// </summary>
        CalculationFinished = 21367,
        /// <summary>
        /// حذف شده
        /// </summary>
        Deleted = 21368,
        /// <summary>
        /// فهرست گرفته و پرداخت شده	
        /// </summary>
        Payed = 21369,
        /// <summary>
        /// در حال اجرا
        /// </summary>
        Running = 21376

    }



    /// <summary>
    /// نوع فایل دیسکت بیمه
    /// </summary>
    public enum InsuranceFileDiskType
    {
        /// <summary>
        /// دیسکت سر جمع کارگاه
        /// </summary>
        DSKWOR00 = 21340,
        /// <summary>
        /// دیسکت فایل ردیف جزئیات
        /// </summary>
        DSKKAR00 = 21339
    }


    /// <summary>
    /// نوع فایل دیسکت مالیات
    /// </summary>
    public enum TaxDisketteFileType
    {
        /// <summary>
        /// اطلاعات فردی کارمند تمامی افراد که در دیسکت هستند در این فایل می آید
        /// </summary>
        WP = 21373,
        /// <summary>
        /// اطلاعات ردیف به ردیف افراد در این فایل هست
        /// </summary>
        WH = 21374,
        /// <summary>
        /// سر جمع ردیف ها داخل این فایل می باشد	
        /// </summary>
        WK = 21375,
        /// <summary>
        /// فایل پاسخ محاسبه شده مالیات آپلود شده توسط کاربر
        /// </summary>
        TaxResponse = 21598
    }
    /// <summary>
    /// جدول شماره 9 سند مالیات
    /// </summary>
    public enum EmployeeStatusTable9
    {
        /// <summary>
        /// عادی
        /// </summary>
        Normal = 1,
        /// <summary>
        /// جانباز
        /// </summary>
        HandicappedInWar = 2,
        /// <summary>
        /// فرزند شهید
        /// </summary>
        MartyrsChild = 3,
        /// <summary>
        /// آزاده
        /// </summary>
        FormerPoW = 4,
        /// <summary>
        /// نیرو های مسلح
        /// </summary>
        ArmedForce = 5,
        /// <summary>
        /// سایر مشمولین بند 14 ماده 91
        /// </summary>
        OtherSubjectToClause14Art91 = 6,
        /// <summary>
        /// اتباع خارجی مشمول قانون اجتناب از اخذ مالیات مضاعف
        /// </summary>
        ForeignNationalsSubjectToDoubleTaxationAvoidance = 7
    }


    /// <summary>
    /// جدول شماره 15 سند مالیات نوع پرداخت
    /// </summary>
    public enum PaymentTypeTable15Tax
    {
        /// <summary>
        /// چک شخصی
        /// </summary>
        PersonalCheck = 1,
        /// <summary>
        /// پرداخت با کارت اعتباری
        /// </summary>
        POS_PaymentCard = 2,
        /// <summary>
        /// انتقال بانکی
        /// </summary>
        Bank_Transfer = 3,
        /// <summary>
        /// سفته
        /// </summary>
        Promissory_Note = 4,
        /// <summary>
        /// چک تضمین شده
        /// </summary>
        GauranteedCheck_CertifiedCheck = 5,
        /// <summary>
        /// واریز نقدی
        /// </summary>
        Cash_Deposit = 6,
        /// <summary>
        /// واریز خزانه
        /// </summary>
        Treasury = 7,
        /// <summary>
        /// ارائه فهرست بدون پرداخت مالیات
        /// </summary>
        NoPayment = 8
    }


    /// <summary>
    /// نوع مکان
    /// </summary>
    public enum PlaceType
    {
        /// <summary>
        /// کشور
        /// </summary>
        Country = 1167,
        /// <summary>
        /// استان
        /// </summary>
        State = 1168,
        /// <summary>
        /// شهر
        /// </summary>
        City = 1169,
        /// <summary>
        /// روستا
        /// </summary>
        Village = 1170
    }

    /// <summary>
    /// نوع گزارش دیسکت بیمه
    /// </summary>
    public enum InsuranceDisketteReportType
    {
        /// <summary>
        /// گزارش بر اساس مرکز هزینه	
        /// </summary>
        ByCostCenter = 21378,
        /// <summary>
        /// گزارش بر اساس ردیف پیمان
        /// </summary>
        ByProjectRow = 21379,
        /// <summary>
        /// گزارش بر اساس پروژه
        /// </summary>
        ByProject = 21380,
        /// <summary>
        /// گزارش بر اساس دوره محاسبه حقوق	
        /// </summary>
        ByPaymentPeriod = 21381
    }
    /// <summary>
    /// نوع رکورد لاگ گروهی
    /// </summary>
    public enum BatchLoggerRecordType
    {
        /// <summary>
        /// محاسبه معوقه حکمی	
        /// </summary>
        OrderArear = 21382,
        /// <summary>
        /// دیسکت بانک
        /// </summary>
        BankDiskette = 21383
    }

    /// <summary>
    /// وضعیت معوقه حکم
    /// </summary>
    public enum ArearsStatus
    {
        /// <summary>
        /// عادی
        /// </summary>
        Normal = 3,
        /// <summary>
        /// ایجاد معوقه
        /// </summary>
        NeedToCalculate = 4,
        /// <summary>
        /// انصراف از پرداخت
        /// </summary>
        CancelToPayment = 5,
        /// <summary>
        /// پرداخت شده
        /// </summary>
        Payed = 7,
        /// <summary>
        /// محاسبه شده
        /// </summary>
        Calculated = 8
    }



    /// <summary>
    /// نوع گردش کار سیستمی (FK به wf.WorkFlowType)
    /// </summary>
    public enum SystemWorkFlowType
    {
        /// <summary>
        /// احکام
        /// </summary>
        Order = 1,
        /// <summary>
        /// تسویه حساب‌ها
        /// </summary>
        Settlement = 2,
    }

    /// <summary>
    /// وضعیت تسویه حساب (FK به bas.Settlement_Status)
    /// </summary>
    public enum SettlementStatus
    {
        /// <summary>
        /// ایجاد اولیه (پیش نویس)
        /// </summary>
        Initial = 1,
        /// <summary>
        /// در انتظار بررسی
        /// </summary>
        PendingReview = 2,
        /// <summary>
        /// تایید و پرداخت
        /// </summary>
        ApprovedAndPaid = 3,
        /// <summary>
        /// تایید نشده
        /// </summary>
        NotApproved = 4,
    }

    public enum MRTtype
    {
        Order = 21252,
        Fiche = 21253,
        Settlement = 21739
    }
}
