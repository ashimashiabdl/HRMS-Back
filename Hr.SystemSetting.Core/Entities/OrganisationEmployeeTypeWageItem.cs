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
    [Table("Organisation_EmployeeType_WageItem", Schema = "Setting")]
    public class OrganisationEmployeeTypeWageItem : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
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
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual WageItem? WageItem { get; set; }

        public int? Priority { get; set; }
        public bool? HideInOrder { get; set; }

        public bool IsDaily { get; set; }
        /// <summary>
        /// به عنوان دستمزد روزانه برای دیسکت بیمه ارسال می شود
        /// </summary>
        public bool IsDailyAndWage { get; set; }
        /// <summary>
        /// پایه سنوات در دیسکت بیمه است ؟
        /// </summary>
        public bool IsSanavatINC { get; set; }



        [NotMapped]
        private new string title { get; set; }
    }
}
