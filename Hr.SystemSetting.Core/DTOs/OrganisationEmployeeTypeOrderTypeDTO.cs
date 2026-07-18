using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeOrderTypeDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderTypeTitle { get; set; }
        public bool? OrderCopyToMyCurrentWorkplace { get; set; }
        public bool? OrderCopyToMyNewWorkplace { get; set; }
        public bool? OrderCopyToMyCurrentCostCenter { get; set; }
        public bool? OrderCopyToMyNewCostCenter { get; set; }
        public bool? OrderCopyToMyCurrentOrganizationUnit { get; set; }
        public bool? OrderCopyToMyNewOrganizationUnit { get; set; }
        public long OrderLevelTypeId { get; set; }
        
        public string? OrderLevelType { get; set; }
        /// <summary>
        /// حتما پست خالی شود
        /// </summary>
        public bool? ForceSetNullForPost { get; set; }
        /// <summary>
        /// حتما شغل خالی شود
        /// </summary>
        public bool? ForceSetNullForJob { get; set; }
        /// <summary>
        /// True: پست از چارت سازمانی انتخاب شود؛ False: عنوان از جدول پست‌ها انتخاب شود
        /// </summary>
        public bool? SelectPostFromChart { get; set; }
        /// <summary>
        /// ضریب تجربه
        /// </summary>
        public int? ExperienceCoefficient { get; set; }
        /// <summary>
        /// ضریب بازنشستگی
        /// </summary>
        public int? RetiredCoefficient { get; set; }
        /// <summary>
        /// ضریب سنوات
        /// </summary>
        public int? YearCoefficient { get; set; }
        /// <summary>
        /// نیازمند تسویه حساب
        /// </summary>
        public bool? NeedSettlement { get; set; }

    }
}
