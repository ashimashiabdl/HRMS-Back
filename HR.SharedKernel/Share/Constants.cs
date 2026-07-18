using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Share
{
    public static class Constants
    {
        /// <summary>
        /// حداکثر حجم جهت آپلود فایل
        /// </summary>
        public const int defaultMaxUploadSize = 6000;
        /// <summary>
        /// تعداد پیش فرض مجاز جهت اشتباه وارد کردن کلمه عبور
        /// </summary>
        public const int defaultMaxWrongPassAttemptCount = 6;
        /// <summary>
        /// مدت زمان اعتبار نشست 
        /// </summary>
        public const int defaultJWTExpireTime = 15;
        /// <summary>
        /// بررسی توانایی های فردی در شرایط احراز شغل
        /// </summary>
        public const bool JobAbilityQualification = true;
        /// <summary>
        /// بررسی شایستگی های فردی در شرایط احراز شغل
        /// </summary>
        public const bool JobCompetencyQualification = true;
        /// <summary>
        /// بررسی رشته های تحصیلی در شرایط احراز شغل
        /// </summary>
        public const bool JobEducationFieldQualification = true;
        /// <summary>
        /// بررسی مقطع تحصیلی در شرایط احراز شغل
        /// </summary>
        public const bool JobEducationGradeQualification = true;
        /// <summary>
        /// بررسی زبان های خارجی در شرایط احراز شغل
        /// </summary>
        public const bool JobForeignLanguageQualification = true;
        /// <summary>
        /// بررسی دوره های آموزشی در شرایط احراز شغل
        /// </summary>
        public const bool JobInitialCourseQualification = true;
        /// <summary>
        /// بررسی ویژگی های شخصیتی در شرایط احراز شغل
        /// </summary>
        public const bool JobRequiredCharacterQualification = true;
        /// <summary>
        /// بررسی نرم افزار های مورد نیاز در شرایط احراز شغل
        /// </summary>
        public const bool JobRequiredSoftwaresQualification = true;
        /// <summary>
        /// ردیف آغاز کننده اطلاعات خالص اکسل کارکرد
        /// </summary>
        public const int FunctionExcelStartingRow = 2;
    }
}
