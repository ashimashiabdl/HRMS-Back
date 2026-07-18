using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_EmployeeType_OrderType", Schema = "Setting")]
    public class OrganisationEmployeeTypeOrderType : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("EmployeeType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeType? EmployeeType { get; set; }
        public long OrderLevelTypeId { get; set; }
        [ForeignKey("OrderType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? OrderLevelType { get; set; }
        public bool OrderCopyToMyCurrentWorkplace { get; set; }
        public bool OrderCopyToMyNewWorkplace { get; set; }
        public bool OrderCopyToMyCurrentCostCenter { get; set; }
        public bool OrderCopyToMyNewCostCenter { get; set; }
        public bool OrderCopyToMyCurrentOrganizationUnit { get; set; }
        public bool OrderCopyToMyNewOrganizationUnit { get; set; }
        public bool ForceSetNullForPost { get; set; }
        public bool ForceSetNullForJob { get; set; }
        /// <summary>
        /// True: پست از چارت سازمانی انتخاب شود؛ False: عنوان از جدول پست‌ها انتخاب شود
        /// </summary>
        public bool SelectPostFromChart { get; set; } = true;
        public int? ExperienceCoefficient { get; set; }

        public int? RetiredCoefficient { get; set; }

        public int? YearCoefficient { get; set; }

        public bool? NeedSettlement { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
